

namespace Manager
{
    class FileMeetingCommunicator
    {
        public FileMeetingCommunicator(string meetingsDir)
        {
            MeetingsDir = meetingsDir;
        }
        private string MeetingsDir
        {
            get;
        }

        /*
            Returns Meeting object with the name given. Returns null if otherwise
         */
        public Meeting? FindMeetingByName(string meetingName)
        {
            string[] allFiles = Directory.GetFiles(MeetingsDir);
            foreach (string file in allFiles)
            {
                if (Path.GetFileNameWithoutExtension(file) == meetingName)
                {
                    return FromFileToMeeting(file);
                }
            }
            return null;

        }

        /*
            Stores Meeting object in json format and writes it to a file with a name of a meeting
         */
        public void MeetingToFile(Meeting meeting)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(meeting);
            File.WriteAllText(Path.Combine(MeetingsDir, $"{meeting.Name}.json"), json);
        }

        /*
            Creates a Meeting object from a file stored in json file. Return a Meeting object. Throws exception othwerwise
         */
        public Meeting FromFileToMeeting(string filePathWithName)
        {
            Meeting? meeting = System.Text.Json.JsonSerializer.Deserialize<Meeting>(File.ReadAllText(filePathWithName));
            if (meeting == null)
            {
                throw new Exception($"Error: failed to convert from file {filePathWithName}");
            }
            return meeting;
        }

        /*
            Gets all available meeting, stores them as Meeting objects in a list
         */
        public List<Meeting> GetMeetings(string meetingDir)
        {
            List<Meeting> meetings = new();
            string[] allMeetingFiles = Directory.GetFiles(meetingDir);
            foreach (string file in allMeetingFiles)
            {
                meetings.Add(FromFileToMeeting(file));
            }
            return meetings;
        }
    }

}

