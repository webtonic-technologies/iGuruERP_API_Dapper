namespace TimeTable_API.DTOs.Responses
{
    public class EmployeeSubstitutionResponse
    {
        public int SubjectID { get; set; }
        public string Subject { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string ClassSession { get; set; } // Class - Section Name
        public string SessionTiming { get; set; } // Session Timing in (08:00 AM - 08:40 AM) format
        public string Substitution { get; set; }
    
    }

}
