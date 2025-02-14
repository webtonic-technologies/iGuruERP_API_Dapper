namespace StudentManagement_API.DTOs.Requests
{
    public class SetStudentStatusActivityRequest
    {
        public int StudentID { get; set; }
        public string InactiveReason { get; set; }
        public int ActivityStatusID { get; set; }
        public int UserID { get; set; }
        public int InstituteID { get; set; }
        public string ActivityDate { get; set; } // Expected format: "DD-MM-YYYY"
    }
}
