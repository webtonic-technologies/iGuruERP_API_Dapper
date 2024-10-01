namespace TimeTable_API.Models
{
    public class TimeTable
    {
        public int TimeTableID { get; set; }
        public string AcademicYearCode { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int DayID { get; set; }
        public bool IsBreak { get; set; }
        public int SessionID { get; set; }
        public int BreaksID { get; set; }
        public int SubjectID { get; set; }
        public int EmployeeID { get; set; }
        public int InstituteID { get; set; }
    }
}
