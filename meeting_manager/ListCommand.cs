

namespace Manager
{
    class ListCommand
    {

        public ListCommand(string meetingsDir)
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

        public string Execute(string allFilters)
        {
            try
            {
                List<Meeting> meetings = Comm.GetMeetings(MeetingsDir); // Get all meetings
                if (allFilters == "") // No filters
                { 
                    return ShowMeetings(meetings);
                }
                else
                {
                    bool filterApplied = false;
                    string[] filters = allFilters.Split('|', StringSplitOptions.TrimEntries);
                    foreach (string filter in filters)
                    {
                        if (filter.StartsWith("-d")) // Description filter
                        {
                            if (ValidateFilter(filter[2..]))
                            {
                                DescFilter(filter[2..], meetings);
                                filterApplied = true;
                            }
                            else
                            {
                                return $"filter {filter} does not match requirements, correct syntax: -d(my description here)";
                            }
                        }
                        if (filter.StartsWith("-r")) // Responsible person filter
                        {
                            if (ValidateFilter(filter[2..]))
                            {
                                RespFilter(filter[2..], meetings);
                                filterApplied = true;
                            }
                            else
                            {
                                return $"filter {filter} does not match requirements, correct syntax: -r(my responsible person)";
                            }
                        }
                        if (filter.StartsWith("-c")) // Category filter
                        {
                            if (ValidateFilter(filter[2..]))
                            {
                                CatFilter(filter[2..], meetings);
                                filterApplied = true;
                            }
                            else
                            {
                                return $"filter {filter} does not match requirements, correct syntax: -c(category)";
                            }
                        }
                        if (filter.StartsWith("-t")) // Type filter
                        {
                            if (ValidateFilter(filter[2..]))
                            {
                                TypeFilter(filter[2..], meetings);
                                filterApplied = true;
                            }
                            else
                            {
                                return $"filter {filter} does not match requirements, correct syntax: -t(type)";
                            }
                        }
                        if (filter.StartsWith("-s")) // Start date filter
                        {
                            if (ValidateFilter(filter[2..]))
                            {
                                StartDateFilter(filter[2..], meetings);
                                filterApplied = true;
                            }
                            else
                            {
                                return $"filter {filter} does not match requirements, correct syntax: -s(start date)";
                            }
                        }
                        if (filter.StartsWith("-e")) // End date filter
                        {
                            if (ValidateFilter(filter[2..]))
                            {
                                EndDateFilter(filter[2..], meetings);
                                filterApplied = true;
                            }
                            else
                            {
                                return $"filter {filter} does not match requirements, correct syntax: -e(end date)";
                            }
                        }
                        if (filter.StartsWith("-n")) // Number of atendees filter
                        {
                            if (ValidateFilter(filter[2..]))
                            {
                                NumberFilter(filter[2..], meetings);
                                filterApplied = true;
                            }
                            else
                            {
                                return $"filter {filter} does not match requirements, correct syntax: -n(=10)";
                            }
                        }
                        if (!filterApplied)
                        {
                            return "filter does not match requirements, correct syntax: <filter>(<filterData>) note: filter MUST start with a '-'";
                        }
                    }
                    return ShowMeetings(meetings);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /*
            Shows all meeting that meet requirements or tells, that none did
         */
        private string ShowMeetings(List<Meeting> meetings)
        {
            if (meetings.Count == 0)
            {
                return "No meeting meets the requirements";
            }
            string res = "";
            foreach (Meeting meeting in meetings)
            {
                if (meeting != null)
                {
                    res += ShowMeeting(meeting);
                }
            }
            return res;
        }

        /*
            Validates filter syntax
         */
        private bool ValidateFilter(string filter)
        {
            return filter.StartsWith('(') ? filter.EndsWith(')') : false;
        }

        /*
            Filters out all meeting that do not contain filter string in their description 
         */
        private void DescFilter(string filter, List<Meeting> meetings)
        {
            filter = filter[1..^1];
            meetings.RemoveAll(meeting => !meeting.Description.Contains(filter));
        }

        /*
            Filters out all meeting that do not have responsible person that was provided in filter
         */
        private void RespFilter(string filter, List<Meeting> meetings)
        {
            filter = filter[1..^1];
            meetings.RemoveAll(meeting => meeting.RespPerson != filter);
        }

        /*
            Filters out all meeting that do not have category that was provided in filter
         */
        private void CatFilter(string filter, List<Meeting> meetings)
        {
            filter = filter[1..^1];
            meetings.RemoveAll(meeting => meeting.Category != filter);
        }

        /*
            Filters out all meeting that do not have type that was provided in filter
         */
        private void TypeFilter(string filter, List<Meeting> meetings)
        {
            filter = filter[1..^1];
            meetings.RemoveAll(meeting => meeting.Type != filter);
        }

        /*
            Filters out all meeting that do not meet start date filter. Works with '>', '<' and '=' values. If none given, '=' used as default
         */
        private void StartDateFilter(string filter, List<Meeting> meetings)
        {
            var cultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture("lt-LT");
            filter = filter[1..^1].Trim();
            if (filter.Contains('>'))
            {
                filter = filter[1..].Trim();
                meetings.RemoveAll(meeting => DateTime.Parse(meeting.StartDate, cultureInfo) <= DateTime.Parse(filter, cultureInfo));
                return;
            }
            if (filter.Contains('<'))
            {
                filter = filter[1..].Trim();
                meetings.RemoveAll(meeting => DateTime.Parse(meeting.StartDate, cultureInfo) >= DateTime.Parse(filter, cultureInfo));
                return;
            }
            if (filter.Contains('='))
            {
                filter = filter[1..].Trim();
            }
            meetings.RemoveAll(meeting => DateTime.Parse(meeting.StartDate, cultureInfo) != DateTime.Parse(filter, cultureInfo));
        }

        /*
            Filters out all meeting that do not meet end date filter. Works with '>', '<' and '=' values. If none given, '=' used as default
         */
        private void EndDateFilter(string filter, List<Meeting> meetings)
        {
            var cultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture("lt-LT");
            filter = filter[1..^1].Trim();
            if (filter.Contains('>'))
            {
                filter = filter[1..].Trim();
                meetings.RemoveAll(meeting => DateTime.Parse(meeting.EndDate, cultureInfo) <= DateTime.Parse(filter, cultureInfo));
                return;
            }
            if (filter.Contains('<'))
            {
                filter = filter[1..].Trim();
                meetings.RemoveAll(meeting => DateTime.Parse(meeting.EndDate, cultureInfo) >= DateTime.Parse(filter, cultureInfo));
                return;
            }
            if (filter.Contains('='))
            {
                filter = filter[1..].Trim();
            }
            meetings.RemoveAll(meeting => DateTime.Parse(meeting.EndDate, cultureInfo) != DateTime.Parse(filter, cultureInfo));
        }

        /*
            Filters out all meeting that do not meet number of atendees filter. Works with '>', '<' and '=' values. If none given, '=' used as default
         */
        private void NumberFilter(string filter, List<Meeting> meetings)
        {
            filter = filter[1..^1].Trim();
            if (filter.Contains('>'))
            {
                filter = filter[1..].Trim();
                meetings.RemoveAll(meeting => meeting.Members.Count <= int.Parse(filter));
                return;
            }
            if (filter.Contains('<'))
            {
                filter = filter[1..].Trim();
                meetings.RemoveAll(meeting => meeting.Members.Count >= int.Parse(filter));
                return;
            }
            if (filter.Contains('='))
            {
                filter = filter[1..].Trim();
            }
            meetings.RemoveAll(meeting => meeting.Members.Count != int.Parse(filter));
        }

        /*
            Shows meeting to a user in specified format
         */
        private string ShowMeeting(Meeting meeting)
        {
            return $"\nName: {meeting.Name}\nDescription: {meeting.Description}\nResponsible person: {meeting.RespPerson}\n" +
                              $"Category: {meeting.Category}\nType: {meeting.Type}\nStart date: {meeting.StartDate}\nEnd date: {meeting.EndDate}\n" +
                              $"Members: {string.Join(" ", meeting.Members)}\n";

        }
    }
}
