
namespace Manager
{
    class AddCommand
    {

        public AddCommand(string meetingsDir)
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

        public string Execute(string username, string meetingName)
        {
            try
            {
                Meeting? meeting = FindMeetingByName(meetingName, MeetingsDir);
                if (meeting == null)
                {
                    return $"Meeting \"{meetingName}\" not found or error opening or reading file {meetingName}.json";
                }

                IList<string>? allMembersInMeeting = meeting.Members;

                if (allMembersInMeeting == null)
                {
                    throw new Exception("Failed to load meeting members");
                }

                foreach (string member in allMembersInMeeting)
                {
                    if (member == username)
                    {
                        return $"\"{username}\" is already in the meeting";
                    }
                }

                allMembersInMeeting.Add(username);
                meeting.Members = allMembersInMeeting;
                Comm.MeetingToFile(meeting);
                return $"User \"{username}\" has been added to meeting \"{meetingName}\" at {DateTime.Now}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        /*
            Returns Meeting object with the name given. Returns null if otherwise
         */
        private Meeting? FindMeetingByName(string meetingName, string meetingsDir)
        {
            string[] allFiles = Directory.GetFiles(meetingsDir);
            foreach (string file in allFiles)
            {
                if (Path.GetFileNameWithoutExtension(file) == meetingName)
                {
                    return Comm.FromFileToMeeting(file);
                }
            }
            return null;

        }

    }
}
