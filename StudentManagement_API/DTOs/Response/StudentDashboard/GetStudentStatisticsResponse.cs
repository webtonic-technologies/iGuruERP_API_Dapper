namespace StudentManagement_API.DTOs.Responses
{
    public class GetStudentStatisticsResponse
    {
        public int TotalStudents { get; set; }
        public GenderStatistics Boys { get; set; }
        public GenderStatistics Girls { get; set; }
    }

    public class GenderStatistics
    {
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
