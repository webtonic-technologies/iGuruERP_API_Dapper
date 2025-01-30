namespace HostelManagement_API.DTOs.Requests
{
    public class GetMealAttendanceRequest
    {
        public string AttendanceDate { get; set; }  // Date in DD-MM-YYYY format
        public int HostelID { get; set; }
        public int MealTypeID { get; set; }
        public int InstituteID { get; set; }
        public string Search { get; set; }
    }
}
