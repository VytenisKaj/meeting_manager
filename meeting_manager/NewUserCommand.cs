
namespace Manager
{
    class NewUserCommand
    {
        public string Execute(string username, string password, string userDataPath, ref string currentUser)
        {
            if(username.Length < 1 || password.Length < 1)
            {
                return "Username or password is empty";
            }
            try
            {
                string[]? userData = File.ReadAllText(userDataPath).Split('\n');
                if(!IsNameAvailable(username, userData))
                {
                    return "Username already exists";
                }
                if (userData[0] == "" || userData[0] == "\n") // First time setup, clears everything, adds new user
                {
                    userData = new string[]{ $"{username} {password}" };
                }
                else // Adds new user
                { 
                    userData = new List<string>(userData) { $"{username} {password}" }.ToArray(); 
                }
                File.WriteAllText(userDataPath, String.Join("\n", userData));
                currentUser = username; // Set current user to new user
                return $"User with name {username} added";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        // Checks if name is available for use
        private bool IsNameAvailable(string name, string[] userData)
        {

            foreach (string user in userData)
            {
                if (name == user.Split(' ')[0]) // Only need to check username, thats why [0] is used
                {
                    return false;
                }
            }
            return true;
        }
    }


}
