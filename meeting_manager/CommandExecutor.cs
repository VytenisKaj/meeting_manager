/* 
    Class gets input command from user, processes it, makes changes accordingly
    Returns ReturnCode.Success if user command was executed(even if it failed later due to wrong parameters etc.)
    Returns ReturnCode.NotFound if user input was not a supported command
    Returns ReturnCode.Exit if user entered exit command to stop execution of program
*/
namespace Manager
{
    public class CommandExecutor
    {

        private readonly string[] commands = {
            "help",
            "?",
            "create <Name> | <RespPerson> | <Desc> | <Category{CodeMonkey, Hub, Short, TeamBuilding}> | <Type{Live, InPerson}> | <StartDate yyyy:MM:dd HH:mm> | <EndDate yyyy:MM:dd HH:mm>",
            "exit",
            "login <username> <password>",
            "new_user <username> <password>",
            "del <Name>",
            "add <user> <meeting>",
            "rm <user> <meeting>",
            "ls <filter1>(<filterData>) | <filter2>(<filterData>)"
        };
        private readonly string[] definitions = {
            "Shows all commands",
            "Shows all commands",
            "Creates a new meeting(arguments seperated by '|', \"Name\" argument has to be a single string)",
            "Exits the program",
            "Logins to user specified",
            "Creates new user with specified credentials",
            "Deletes meeting with specified name",
            "Adds user to specified meeting",
            "Removes a user from specified meeting",
            "Lists all meetings that pass filters(arguments seperated by '|'. If no filter is given, all meetings are shown.\n" +
                "filters: -d = description, -r = responsible person, -c = category, -t = type, -s = start date, -e = end date, -n = number of atendees(use with <, > and =))"
        };

        

        public CommandExecutor()
        {
            DataDirName = "data";
            CurrentUser = "";
            MeetingsDirName = "meetings";
            UserDataFileName = "user_data.dat";
            DataDir = CreateDataDir();
            UserDataPath = Path.Combine(DataDir.FullName, UserDataFileName);
            CreateUserDataFile(UserDataPath);
            MeetingsDir = CreateMeetingsDir();
            NewUser = new();
            LoginCommand = new();
            CreateCommand = new(MeetingsDir.FullName);
            DeleteCommand = new(MeetingsDir.FullName);
            ListCommand = new(MeetingsDir.FullName);
            AddCommand = new(MeetingsDir.FullName);
            RemoveCommand = new(MeetingsDir.FullName);
        }

        private string UserDataPath
        {
            get;
        }

        private string UserDataFileName
        {
            get;
        }

        private DirectoryInfo DataDir
        {
            get;
        }

        private DirectoryInfo MeetingsDir
        {
            get;
        }

        private string MeetingsDirName
        {
            get;
        }

        public string CurrentUser
        {
            get;
            set;
        }

        private string DataDirName
        {
            get;
        }

        public string GetCommandByIndex(int index)
        {
            return commands[index];
        }

        public string GetDefinitionByIndex(int index)
        {
            return definitions[index];
        }

        private NewUserCommand NewUser
        {
            get;
        }

        private LoginCommand LoginCommand 
        {
            get;
        }

        private CreateCommand CreateCommand 
        {
            get;
        }

        private DeleteCommand DeleteCommand
        {
            get;
        }

        private ListCommand ListCommand
        {
            get;
        }

        private AddCommand AddCommand
        {
            get;
        }

        private RemoveCommand RemoveCommand
        {
            get;
        }

        /*
            Creates "data" directory in project directory if it does not exist and returns path to it, otherwise just returns path to it 
         */
        private DirectoryInfo CreateDataDir()
        {
            var dataDir = Directory.CreateDirectory(DataDirName);
            if (!dataDir.Exists)
            {
                throw new Exception("Error: failed to create data directory");
            }
            return dataDir;
        }


        /*
            Creates "user_data.dat" file if it does not exists. Opens and closes it to make sure it is working
        */
        private void CreateUserDataFile(string userDataPath)
        {
            if (!File.Exists(userDataPath))
            {
                Console.WriteLine($"{UserDataFileName} does not exist, creating new one");
                FileStream userDataStream = File.Create(userDataPath);
                if (userDataStream == null)
                {
                    throw new Exception("Error: failed to create user_data file");
                }
                userDataStream.Close();
            }
            else
            {
                FileStream userDataStream = File.Open(userDataPath, FileMode.Open, FileAccess.ReadWrite);
                if (userDataStream == null)
                {
                    throw new Exception("Error: failed to open user_data file");
                }
                userDataStream.Close();
            }
        }

        /*
            Creates "meeting" directory in "data" directory if it does not exist and returns path to it, otherwise just returns path to it 
         */
        private DirectoryInfo CreateMeetingsDir()
        {
            
            DirectoryInfo meetingDir = Directory.CreateDirectory(Path.Combine(DataDir.FullName, MeetingsDirName));
            if (!meetingDir.Exists)
            {
                throw new Exception("Error: failed to create meetings directory");
            }
            return meetingDir;
            
        }


        /*
            Shows all command and their definitions to user 
         */
        public string ShowCommands()
        {
            try
            {
                // Each command must have a definition
                if (commands.Length != definitions.Length)
                {
                    throw new Exception("Error: Commands do not match definitions");
                }
                string res = "";
                for (int i = 0; i < commands.Length; i++)
                {
                    res += $"{GetCommandByIndex(i)} -> {GetDefinitionByIndex(i)}\n";
                }
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string ExecuteHelp() => ShowCommands();


        /*
            Creates new user using provided username and password
         */
        private string ExecuteNewUser(string username, string password)
        {
            string tempCurrentUser = CurrentUser;
            string res = NewUser.Execute(username, password, UserDataPath, ref tempCurrentUser);
            CurrentUser = tempCurrentUser;
            return res;
        }

        /*
            Logs on user to an account to grant access to commands, that require you to be "responsible person"
         */
        private string ExecuteLogin(string username, string password)
        {
            string tempCurrentUser = CurrentUser;
            string res = LoginCommand.Execute(username, password, UserDataPath, ref tempCurrentUser);
            CurrentUser = tempCurrentUser;
            return res;
        }


        /*
            Creates a new meeting if command is correct
         */
        private string ExecuteCreate(string name, string respPerson, string desc, string category, string type, string startDate, string endDate)
        {
            return CreateCommand.Execute(name, respPerson, desc, category, type, startDate, endDate); 
        }

        /*
            Deletes a meeting if current user is "responsible person"
         */
        private string ExecuteDelete(string name)
        {
            return DeleteCommand.Execute(name, CurrentUser);
        }


        /*
            Adds user to a meeting. Does not allow to add a user multiple times. Notifies when user gets added and shows when it did.
         */
        private string ExecuteAdd(string username, string meetingName)
        {
            return AddCommand.Execute(username, meetingName);
        }

        /*
            Removes user from a meeting. Does not allow to remove Responsible person
         */
        private string ExecuteRemove(string username, string meetingName)
        {
            return RemoveCommand.Execute(username, meetingName);
        }


        /*
            Lists all meeting that meet ALL filters
         */
        private string ExecuteLs(string allFilters)
        {
            return ListCommand.Execute(allFilters);
        }

        
        /* 
            Executes commands. 
            Returns "ReturnCode" values
         */
        public Tuple<int, string> ExecuteCommand(string command)
        {
            if(command == "")
            {
                return new((int)ReturnCode.Not_found, "");
            }
            // Splits command into tokens
            string[] commandParams = command.Trim().Split(' ');
            
            // "exit" command
            if(commandParams[0].ToLower() == commands[(int)Command.Exit].Split(" ")[0])
            {
                return new((int)ReturnCode.Exit, "");
            }
            // "?" or "help" command
            if(commandParams[0].ToLower() == commands[(int)Command.Q_mark].Split(" ")[0] || commandParams[0].ToLower() == commands[(int)Command.Help].Split(" ")[0])
            {
                return new((int)ReturnCode.Success, ExecuteHelp());
            }

            // "new_user" command
            if(commandParams[0].ToLower() == commands[(int)Command.New_user].Split(" ")[0]) 
            {
                if(commandParams.Length < 3)
                {
                    return new((int)ReturnCode.Success, "Not enough arguments");
                }
                else
                {
                    return new((int)ReturnCode.Success, ExecuteNewUser(commandParams[1].Trim(), commandParams[2].Trim()));
                }
            }

            // "login" command
            if (commandParams[0].ToLower() == commands[(int)Command.Login].Split(" ")[0])
            {
                if (commandParams.Length < 3)
                {
                    return new((int)ReturnCode.Success, "Not enough arguments");
                }
                else
                {
                    return new((int)ReturnCode.Success, ExecuteLogin(commandParams[1].Trim(), commandParams[2].Trim()));
                }
            }

            // "create" command
            if(commandParams[0].ToLower() == commands[(int)Command.Create].Split(" ")[0])
            {
                
                commandParams = command.Trim().Split('|');
                if (commandParams.Length < 7)
                {
                    return new((int)ReturnCode.Success, "Not enough arguments");
                }
                else
                {
                    return new((int)ReturnCode.Success, ExecuteCreate(commandParams[0].Split(' ')[1].Trim(), commandParams[1].Trim(), commandParams[2].Trim(), commandParams[3].Trim(), commandParams[4].Trim(), commandParams[5].Trim(), commandParams[6].Trim()));
                }
            }

            // "del" command
            if(commandParams[0].ToLower() == commands[(int)Command.Del].Split(" ")[0])
            {
                if(commandParams.Length < 2)
                {
                    return new((int)ReturnCode.Success, "Not enough arguments");
                }
                else
                {
                    return new((int)ReturnCode.Success, ExecuteDelete(commandParams[1]));
                }
            }

            // "add" command
            if(commandParams[0].ToLower() == commands[(int)Command.Add].Split(" ")[0])
            {
                if(commandParams.Length < 3)
                {
                    return new((int)ReturnCode.Success, "Not enough arguments");
                }
                else
                {
                    return new((int)ReturnCode.Success, ExecuteAdd(commandParams[1], commandParams[2]));
                }
            }

            // "rm" command
            if(commandParams[0].ToLower() == commands[(int)Command.Rm].Split(" ")[0])
            {
                if(commandParams.Length < 3)
                {
                    return new((int)ReturnCode.Success, "Not enough arguments");
                }
                else
                {
                    return new((int)ReturnCode.Success, ExecuteRemove(commandParams[1], commandParams[2]));
                }
            }

            // "ls" command
            if(commandParams[0].ToLower() == commands[(int)Command.Ls].Split(" ")[0])
            {
                if(commandParams.Length == 1)
                {
                    return new((int)ReturnCode.Success, ExecuteLs(""));
                }
                else
                {
                    return new((int)ReturnCode.Success, ExecuteLs(command.Substring(command.IndexOf(' '))));
                }
            }

            return new((int)ReturnCode.Not_found, ""); ;
        }
    }
}
