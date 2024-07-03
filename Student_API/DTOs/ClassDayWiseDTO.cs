namespace Student_API.DTOs
{
    public class ClassDayWiseDTO
    {
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string AcademicYear { get; set; }
        public int SessionsPerWeek { get; set; }
        public List<SubjectCountDTO> SubjectsPerWeek { get; set; }
        public List<ClassDayWiseDetailDTO> DayWiseDetails { get; set; }
    }

    public class ClassDayWiseDetailDTO
    {
        public int DayId { get; set; }
        public string DayName { get; set; }
        public int PeriodId { get; set; }
        public string PeriodName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }
    public class SubjectCountDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int Count { get; set; }
    }
}
