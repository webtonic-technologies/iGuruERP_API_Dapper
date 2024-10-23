namespace Attendance_SE_API.DTOs.Requests
{
    public class GetAllShiftsRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
