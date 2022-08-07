

namespace Manager
{
    class Meeting
    {
        public Meeting(string name, string description, string respPerson, string category, string type, string startDate, string endDate, IList<string> members)
        {
            Name = name;
            Description = description;
            RespPerson = respPerson;
            Category = category;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            Members = members;
        }


        public string Name{
            get; set; 
        }
        public string Description { 
            get; set;
        }
        public string RespPerson
        {
            get; set;
        }

        public string Category
        {
            get; set;
        }

        public string Type
        {
            get; set;
        }

        public string StartDate
        {
            get; set;
        }

        public string EndDate
        {
            get; set;
        }

        public IList<string> Members { get; set; }
    }
}
