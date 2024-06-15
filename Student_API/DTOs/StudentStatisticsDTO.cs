namespace Student_API.DTOs
{
    public class StudentStatisticsDTO
    {
        public List<GenderWiseStudentCountDTO> GenderCounts { get; set; }
        public List<StatusWiseStudentCountDTO> StatusCounts { get; set; }
        public List<StudentTypeWiseCountDTO> StudentTypeCounts { get; set; }
    }
    public class GenderWiseStudentCountDTO
    {
        public string Gender { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class StatusWiseStudentCountDTO
    {
        public string Status { get; set; } // "Active" or "Inactive"
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class StudentTypeWiseCountDTO
    {
        public string StudentType { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
