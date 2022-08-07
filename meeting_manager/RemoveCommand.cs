

namespace Manager
{
    class RemoveCommand
    {
        public RemoveCommand(string meetingsDir)
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
                Meeting? meeting = Comm.FindMeetingByName(meetingName);
                if (meeting == null)
                {
                    throw new Exception($"Error opening or reading file {meetingName}.json (meeting with this name may not exist)");
                }
                if (username == meeting.RespPerson)
                {
                    return "Responsible person cannot be removed from meeting";
                }

                IList<string>? allMembersInMeeting = meeting.Members;

                if (allMembersInMeeting == null)
                {
                    throw new Exception("Failed to load meeting members");
                }

                if (allMembersInMeeting.Remove(username))
                {
                    meeting.Members = allMembersInMeeting;
                    Comm.MeetingToFile(meeting);
                    return $"User \"{username}\" has been removed from meeting \"{meetingName}\"";
                }
                return $"User \"{username}\" was not found in meeting \"{meetingName}\"";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }

    
}
