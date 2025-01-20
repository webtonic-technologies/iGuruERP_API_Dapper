namespace HostelManagement_API.DTOs.Requests
{
    public class SetMealAttendanceRequest
    {
        public int StudentID { get; set; }
        public int MealTypeID { get; set; }
        public int HostelID { get; set; }
        public string AttendanceDate { get; set; }  // Date in DD-MM-YYYY format
        public int StatusID { get; set; }
        public string Remarks { get; set; }
        public int InstituteID { get; set; }
    }
}
