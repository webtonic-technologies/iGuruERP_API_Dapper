namespace StudentManagement_API.DTOs.Responses
{
    public class GetStudentStatusStatisticsResponse
    {
        public StatusStatistics Active { get; set; }
        public StatusStatistics Inactive { get; set; }
    }

    public class StatusStatistics
    {
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
