namespace Communication_API.DTOs.Requests.Email
{
    public class UpdateEmailStudentStatusRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public int StudentID { get; set; }
        public int EmailStatusID { get; set; }  // Status ID: 0 for Pending, 1 for Delivered, 2 for Failed
    }
}
