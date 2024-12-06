namespace Communication_API.DTOs.Requests.Email
{
    public class UpdateEmailEmployeeStatusRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public int EmployeeID { get; set; }
        public int EmailStatusID { get; set; }  // Status ID: 0 for Pending, 1 for Delivered, 2 for Failed
    }
}
