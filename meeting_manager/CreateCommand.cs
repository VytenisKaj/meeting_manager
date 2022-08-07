

namespace Manager
{
    public class CreateCommand
    {

        public CreateCommand(string meetingsDir)
        {
            MeetingsDir = meetingsDir;
            Comm = new(MeetingsDir);
        }


        private readonly string[] categories = {
            "CodeMonkey",
            "Hub",
            "Short",
            "TeamBuilding"
        };

        private readonly string[] types = {
            "Live",
            "InPerson"
        };

        private string MeetingsDir
        {
            get;
        }

        private FileMeetingCommunicator Comm
        {
            get;
        }
        public string Execute(string name, string respPerson, string desc, string category, string type, string startDate, string endDate)
        {
            try
            {
                if (!ConfirmName(name.Trim(), MeetingsDir))
                {
                    return "Meeting with this name already exists";
                }
                if (!ConfirmCategory(category))
                {
                    return "Category has to be one of the following values: CodeMonkey, Hub, Short, TeamBuilding";
                }
                if (!ConfirmType(type))
                {
                    return "Type has to be one of the following values: Live, InPerson";
                }
                if (!ConfirmDates(startDate, endDate))
                {
                    return "Start date cannot be later than end date";
                }
                Meeting m = new(name.Trim(), desc, respPerson, category, type, startDate, endDate, new List<string>() { respPerson });
                Comm.MeetingToFile(m);
                return $"Meeting {name} has been created and is stored in {name}.json file";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /*
            Checks if meeting with this name exists, returns true if name is available for use, false otherwise
         */
        private bool ConfirmName(string name, string meetingsDir)
        {
            string[] allFiles = Directory.GetFiles(meetingsDir);
            if (allFiles.Length == 0)
            {
                return true;
            }
            foreach (string file in allFiles)
            {
                if (Path.GetFileNameWithoutExtension(file) == name)
                {
                    return false;
                }
            }
            return true;
        }

        /*
            Checks if category is valid. Returns true if valid, otherwise returns false
         */
        private bool ConfirmCategory(string category)
        {

            foreach (string cat in categories)
            {
                if (cat == category)
                {
                    return true;
                }
            }

            return false;

        }

        /*
            Checks if type is valid. Returns true if valid, otherwise returns false
         */
        private bool ConfirmType(string type)
        {
            foreach (string t in types)
            {
                if (t == type)
                {
                    return true;
                }
            }

            return false;
        }

        /*
            Validates input for both dates, checks if startDate is "smaller" than endDate.
            Returns true if both cases are correct, returns false if startDate is "bigger or equal" than endDate
            throws exception if date format is incorrect (gets caught in previous function)
         */
        private bool ConfirmDates(string startDate, string endDate)
        {
            System.Globalization.CultureInfo cultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture("lt-LT");
            var startDateVal = DateTime.Parse(startDate, cultureInfo);
            var endDateVal = DateTime.Parse(endDate, cultureInfo);
            return startDateVal < endDateVal;
        }

    }

}
