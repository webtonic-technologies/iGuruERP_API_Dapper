namespace Student_API.Models
{
    public class Timetable
    {
        public int Timetable_id { get; set; }
        public int TimetableGroup_id { get; set; }
        public int? PeriodBreak_id { get; set; }
        public int? Period_id { get; set; }
        public int? Subject_id { get; set; }
        public int? Employee_id { get; set; }
        public bool IsBreak { get; set; }
        public int AcademicYear { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
        public int InstituteId { get; set; }
    }
}
