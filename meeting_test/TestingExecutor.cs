using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Manager
{
    [TestClass]
    public class TestingExecutor
    {

        private MeetingManager Manager
        {
            get;
        }

        public TestingExecutor()
        {
            // Uncaught exceptions will stop tests(intended, they should not run if anything bad happens here)
            Manager = new MeetingManager();
            CopyDirectory(@"..\..\..\test_data\data", "data", true);
        }

        [TestMethod]
        public void TestBadInput()
        {

            var res = Manager.ExecuteCommand("");
            Assert.AreEqual((int)ReturnCode.Not_found, res.Item1);
            Assert.AreEqual("", res.Item2);

            res = Manager.ExecuteCommand("NotACommand");
            Assert.AreEqual((int)ReturnCode.Not_found, res.Item1);
            Assert.AreEqual("", res.Item2);
            
        }

        [TestMethod]
        public void TestExitCommand()
        {
            // Normal input
            var res = Manager.ExecuteCommand("exit");
            Assert.AreEqual( (int)ReturnCode.Exit, res.Item1);
            Assert.AreEqual("", res.Item2);

            // Capitalized input, should not do anything
            res = Manager.ExecuteCommand("EXIT");
            Assert.AreEqual((int)ReturnCode.Exit, res.Item1);
            Assert.AreEqual("", res.Item2);

            // Not a full command name, should not be executed
            res = Manager.ExecuteCommand("exi");
            Assert.AreEqual((int)ReturnCode.Not_found, res.Item1);
            Assert.AreEqual("", res.Item2);

            // Should still work, anything past a valid command is ignored
            res = Manager.ExecuteCommand("exit and some other things after that"); 
            Assert.AreEqual((int)ReturnCode.Exit, res.Item1);
            Assert.AreEqual("", res.Item2);
        }

        [TestMethod]
        public void TestCreateMethod()
        {

            // No arguments
            var res = Manager.ExecuteCommand("create");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // Not a full command name
            res = Manager.ExecuteCommand("creat");
            Assert.AreEqual((int)ReturnCode.Not_found, res.Item1);
            Assert.AreEqual("", res.Item2);

            // Wrong command syntax, arguments that are not seperated by '|' are ignored
            res = Manager.ExecuteCommand("create TestMeeting1 User Some Description Hub Live 2022-08-04 10:00 2022-08-04 11:00"); 
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // Category is not one of the following CodeMonkey, Hub, Short, TeamBuilding (must be exact match)
            res = Manager.ExecuteCommand("create TestMeeting1|User| Some Description |hub |Live| 2022-08-04 10:00| 2022-08-04 11:00");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Category has to be one of the following values: CodeMonkey, Hub, Short, TeamBuilding", res.Item2);

            // Type is not one of the following Live, InPerson (must be exact match)
            res = Manager.ExecuteCommand("cReate TestMeeting1|User| Some Description |Hub |live| 2022-08-04 10:00| 2022-08-04 11:00");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Type has to be one of the following values: Live, InPerson", res.Item2);

            // Start date is later than end date
            res = Manager.ExecuteCommand("CREATE TestMeeting1|User| Some Description |Hub |Live| 2022-08-04 10:00| 2022-08-04 09:00");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Start date cannot be later than end date", res.Item2);

            // Bad start date format
            res = Manager.ExecuteCommand("create TestMeeting1|User| Some Description |Hub |Live| 2022-08-35 10:00| 2022-08-04 11:00");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("String '2022-08-35 10:00' was not recognized as a valid DateTime.", res.Item2);

            // Bad end date format
            res = Manager.ExecuteCommand("create TestMeeting1|User| Some Description |Hub |Live| 2022-08-04 10:00| 2022-13-35 11:00");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("String '2022-13-35 11:00' was not recognized as a valid DateTime.", res.Item2);

            // End date is missing
            res = Manager.ExecuteCommand("create TestMeeting1|User| Some Description |Hub |Live| 2022-08-04 10:00");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // Correct command, but meeting exists
            res = Manager.ExecuteCommand("cReate TestMeeting|User| Some Description |Hub |Live| 2022-08-04 10:00| 2022-08-04 11:00");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Meeting with this name already exists", res.Item2);

            // Correct command, creates new meeting, will be deleted later
            res = Manager.ExecuteCommand("create DeleteMe|User| Some Description |Hub |Live| 2022-08-04 10:00| 2022-08-04 11:00");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Meeting DeleteMe has been created and is stored in DeleteMe.json file", res.Item2);
        }

        [TestMethod]
        public void TestNewUserCommand()
        {
            // No arguments
            var res = Manager.ExecuteCommand("new_user");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // Only name given
            res = Manager.ExecuteCommand("new_USER user");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // Normal command
            res = Manager.ExecuteCommand("neW_user user user");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Username already exists", res.Item2);

            // Too many arguments, should work normally
            res = Manager.ExecuteCommand("NEW_USER user user user");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Username already exists", res.Item2);
        }

        [TestMethod]
        public void TestLoginCommand()
        {
            // No arguments
            var res = Manager.ExecuteCommand("login");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // No password
            res = Manager.ExecuteCommand("login user");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // Normal command
            Manager.ExecuteCommand("new_user user user");
            res = Manager.ExecuteCommand("LOGIN user user");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Logging on as user", res.Item2);

            // Wrong password given
            res = Manager.ExecuteCommand("loGin user WrongPassWord");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Password is incorrect", res.Item2);

            // User with a name does not exist
            res = Manager.ExecuteCommand("loGin UserDoesntExist user");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("User with this username does not exist", res.Item2);
        }

        [TestMethod]
        public void TestAddCommand()
        {
            // No arguments
            var res = Manager.ExecuteCommand("add");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // No meeting
            res = Manager.ExecuteCommand("add user");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // Meeting does not exist
            res = Manager.ExecuteCommand("add user NotExistingMeeting");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Meeting \"NotExistingMeeting\" not found or error opening or reading file NotExistingMeeting.json", res.Item2);

            // User is already in a meeting
            res = Manager.ExecuteCommand("add user TestMeeting");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("\"user\" is already in the meeting", res.Item2);

        }

        [TestMethod]
        public void TestDeleteCommand()
        {
            // No meeting
            var res = Manager.ExecuteCommand("del");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // Correct command, but meeting does not exist
            res = Manager.ExecuteCommand("del DoesNotExist");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Metting with name \"DoesNotExist\" does not exist", res.Item2);

            // Correct command, but not logged on as responsible person, should NOT delete meeting and inform user
            res = Manager.ExecuteCommand("DEL DeleteMe");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Only responsible person can delete this meeting", res.Item2);

            // Correct command, logged on as repsonsible person, should delete meeting
            Manager.ExecuteCommand("new_user User User");
            Manager.ExecuteCommand("login User User"); 
            res = Manager.ExecuteCommand("DEL DeleteMe");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Meeting with name \"DeleteMe\" deleted", res.Item2);

        }

        [TestMethod]
        public void TestRemoveCommand()
        {
            // No arguments
            var res = Manager.ExecuteCommand("rm");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // No meeting to remove user form
            res = Manager.ExecuteCommand("rm deleteUser");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Not enough arguments", res.Item2);

            // No meeting with this name
            res = Manager.ExecuteCommand("rm deleteUser NoMeetingWithThisName");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Error opening or reading file NoMeetingWithThisName.json (meeting with this name may not exist)", res.Item2);

            // User does not exist in meeting
            res = Manager.ExecuteCommand("rm deleteUser TestMeeting");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("User \"deleteUser\" was not found in meeting \"TestMeeting\"", res.Item2);

            // Correct command
            Manager.ExecuteCommand("add deleteUser TestMeeting"); // adding user before removing it
            res = Manager.ExecuteCommand("rm deleteUser TestMeeting");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("User \"deleteUser\" has been removed from meeting \"TestMeeting\"", res.Item2);

            // Correct command, but responsible person should not be removed from meeting
            res = Manager.ExecuteCommand("rm User TestMeeting");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("Responsible person cannot be removed from meeting", res.Item2);
        }

        [TestMethod]

        public void TestListCommand()
        {

            // Outputs of meeting that will be used for testing
            string testMeeting1 = "\nName: TestMeeting\nDescription: Some Description\nResponsible person: User\nCategory: Hub\nType: Live\nStart date: 2022-08-04 10:00\nEnd date: 2022-08-04 11:00\nMembers: User user\n";
            string testMeeting2 = "\nName: TestMeeting2\nDescription: Another meeting for ls command\nResponsible person: user2\nCategory: Hub\nType: Live\nStart date: 2022-06-22 10:00\nEnd date: 2022-06-23 23:00\nMembers: User user user2 user3 user4 user5\n";
            string testMeeting3 = "\nName: TestMeeting3\nDescription: Third meeting\nResponsible person: user2\nCategory: CodeMonkey\nType: Live\nStart date: 2022-07-04 10:00\nEnd date: 2022-07-04 11:00\nMembers: User user user2 user3\n";
            string testMeeting4 = "\nName: TestMeeting4\nDescription: 4th meeting\nResponsible person: user3\nCategory: TeamBuilding\nType: InPerson\nStart date: 2022-08-04 19:00\nEnd date: 2022-08-04 20:00\nMembers: user3 user\n";
            string testMeeting5 = "\nName: TestMeeting5\nDescription: This is a 5th meeting for tests\nResponsible person: user3\nCategory: Short\nType: Live\nStart date: 2022-08-04 10:00\nEnd date: 2022-08-04 10:15\nMembers: user3 user1\n";
            string testMeeting6 = "\nName: TestMeeting6\nDescription: Some Description\nResponsible person: User\nCategory: Hub\nType: InPerson\nStart date: 2022-09-04 10:00\nEnd date: 2022-09-04 11:00\nMembers: User user user2 user3\n";
            
            // Correct command, lists all meetings
            var res = Manager.ExecuteCommand("ls");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting1}{testMeeting2}{testMeeting3}{testMeeting4}{testMeeting5}{testMeeting6}", res.Item2);

            // Bad syntax, missing '-'
            res = Manager.ExecuteCommand("ls d(Some description)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("filter does not match requirements, correct syntax: <filter>(<filterData>) note: filter MUST start with a '-'", res.Item2);

            // Bad syntax, missing '(' and ')'
            res = Manager.ExecuteCommand("ls -d Some description");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("filter -d Some description does not match requirements, correct syntax: -d(my description here)", res.Item2);

            // Bad syntax, ' ' between filter and opening '(' of its argument
            res = Manager.ExecuteCommand("ls -d (Some description)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("filter -d (Some description) does not match requirements, correct syntax: -d(my description here)", res.Item2);

            // Bad syntax, missing '('
            res = Manager.ExecuteCommand("ls -dSome description)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("filter -dSome description) does not match requirements, correct syntax: -d(my description here)", res.Item2);

            // Bad syntax, missing ')'
            res = Manager.ExecuteCommand("ls -d(Some description");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("filter -d(Some description does not match requirements, correct syntax: -d(my description here)", res.Item2);

            // All filters syntax is checked in the same way, not going to test syntax for other types of filter

            res = Manager.ExecuteCommand("ls -d(No meeting meets requirements)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual("No meeting meets the requirements", res.Item2);

            // Correct description filter
            res = Manager.ExecuteCommand("ls -d(meeting)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting2}{testMeeting3}{testMeeting4}{testMeeting5}", res.Item2);

            // Correct responsible person filter
            res = Manager.ExecuteCommand("ls -r(user2)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting2}{testMeeting3}", res.Item2);

            // Correct category filter
            res = Manager.ExecuteCommand("ls -c(Hub)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting1}{testMeeting2}{testMeeting6}", res.Item2);

            // Correct type filter
            res = Manager.ExecuteCommand("ls -t(InPerson)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting4}{testMeeting6}", res.Item2);


            // Combining several filters, correct syntax
            res = Manager.ExecuteCommand("ls -s(>2022-07-04 10:00) | -s(<2022-08-04 19:00)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting1}{testMeeting5}", res.Item2);

            // In case of use of '=', '<', '>' and using several filters, everything is implemented the same way, testing it only once.

            // Testing '='
            res = Manager.ExecuteCommand("ls -n(=2)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting1}{testMeeting4}{testMeeting5}", res.Item2);

            // Testing default sign (it is same as '=')
            res = Manager.ExecuteCommand("ls -n(2)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting1}{testMeeting4}{testMeeting5}", res.Item2);

            // Testing '>'
            res = Manager.ExecuteCommand("ls -n(>2)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting2}{testMeeting3}{testMeeting6}", res.Item2);

            // Testing '<'
            res = Manager.ExecuteCommand("ls -n(<3)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting1}{testMeeting4}{testMeeting5}", res.Item2);

            // Testing 2 filters
            res = Manager.ExecuteCommand("ls -n(>2)|-n(<5)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"{testMeeting3}{testMeeting6}", res.Item2);

            // Not an integer provided
            res = Manager.ExecuteCommand("ls -n(<3.141592)");
            Assert.AreEqual((int)ReturnCode.Success, res.Item1);
            Assert.AreEqual($"Input string was not in a correct format.", res.Item2);
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

    }
}