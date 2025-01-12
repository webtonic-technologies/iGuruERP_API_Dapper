namespace Attendance_SE_API.DTOs.Requests
{
    public class GetEmployeesArrivalStatsRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Format: "DD-MM-YYYY"
        public string EndDate { get; set; }    // Format: "DD-MM-YYYY"
    }
}
