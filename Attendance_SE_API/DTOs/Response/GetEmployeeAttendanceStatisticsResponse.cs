namespace Attendance_SE_API.DTOs.Responses
{
    public class GetEmployeeAttendanceStatisticsResponse
    {
        public int TotalEmployeeCount { get; set; }
        public int Present { get; set; }
        public int OnLeave { get; set; }
        public int NotInYet { get; set; }
    }
}
