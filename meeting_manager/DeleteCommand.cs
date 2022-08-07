
namespace Manager
{
    public class DeleteCommand
    {
        public DeleteCommand(string meetingsDir)
        {
            MeetingsDir = meetingsDir;
            Comm = new(MeetingsDir);
        }
        private string MeetingsDir
        {
            get;
        }

        private FileMeetingCommunicator Comm
        {
            get;
        }
        public string Execute(string name, string currentUser)
        {
            try
            {
                string[] allFiles = Directory.GetFiles(MeetingsDir);
                foreach (string file in allFiles)
                {
                    if (Path.GetFileNameWithoutExtension(file) == name)
                    {

                        if (CanDeleteMeeting(file, currentUser))
                        {
                            File.Delete(file);
                            return $"Meeting with name \"{name}\" deleted";
                        }
                        else
                        {
                            return $"Only responsible person can delete this meeting";
                        }

                    }
                }
                return $"Metting with name \"{name}\" does not exist";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /*
            Checks if current user is "responsible person" and is allowed to delete meeting. Returns true if it is allowwd, false otherwise
         */
        private bool CanDeleteMeeting(string fileName, string currentUser)
        {
            return Comm.FromFileToMeeting(fileName).RespPerson == currentUser;
        }

        /*
            Creates a Meeting object from a file stored in json file. Return a Meeting object. Throws exception othwerwise
         */
        
    }
}
