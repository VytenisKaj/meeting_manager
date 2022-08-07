
namespace Manager
{
    public class LoginCommand
    {
        public string Execute(string username, string password, string userDataPath, ref string currentUser)
        {
            if (username.Length < 1 || password.Length < 1)
            {
                return "Username or password is empty";
            }
            try
            {
                string[]? users = File.ReadAllText(userDataPath).Split('\n');
                foreach (string user in users)
                {
                    string[] userData = user.Split(' ');
                    if (userData[0] == username)
                    {
                        if (userData[1] == password)
                        {
                            currentUser = username;
                            return $"Logging on as {username}";
                        }
                        else
                        {
                            return "Password is incorrect";
                        }
                    }
                }
                return "User with this username does not exist";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
