/*
    Main program class, execution starts here
*/
namespace Manager
{
    public class MeetingManager
    {
        private bool SetupCompleted
        {
            get; set; 
        }
        public CommandExecutor Executor
        {
            get; private set;
        }


#pragma warning disable CS8618 // Executor cannot be null, because if anything happens, exception will be thrown in CommandExecutor constructor
        // it will be caught in MeetingManager constructor and program will know setup failed and will abort further execution.
        public MeetingManager()
#pragma warning restore CS8618
        {
            try {

                Executor = new();
                SetupCompleted = true;
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                SetupCompleted = false;
            }

            
        }

        public Tuple<int, string> ExecuteCommand(string command)
        {
            return Executor.ExecuteCommand(command);
        }

        public void ShowStartMessage()
        {
            Console.WriteLine($"Use \"{Executor.GetCommandByIndex((int)Command.Login)}\" to login to existing user\n" +
                $"Use \"{Executor.GetCommandByIndex((int)Command.New_user)}\" to create new user\n" +
                $"Use \"{Executor.GetCommandByIndex((int)Command.Q_mark)}\" to get list of all commands\n"
                );
        }
        
        public static void Main() {   
            MeetingManager meetingManager = new();
            meetingManager.ShowStartMessage();

            string? input;
            if (meetingManager.SetupCompleted)
            {
                // main event loop
                while (true) {
                    Console.Write($"\n{((meetingManager.Executor.CurrentUser == "") ? "" : $"Logged on as {meetingManager.Executor.CurrentUser}\n")}Your command: ");
                    input = Console.ReadLine();
                    Tuple<int, string> result = meetingManager.ExecuteCommand(input ?? "");
                    if(result.Item1 == (int)ReturnCode.Exit)
                    {
                        break;
                    }
                    if(result.Item1 == (int)ReturnCode.Not_found)
                    {
                        Console.WriteLine("Command not found");
                    }
                    if(result.Item1 == (int)ReturnCode.Success)
                    {
                        Console.WriteLine(result.Item2);
                    }
                }
            }
            
        }
    }
}