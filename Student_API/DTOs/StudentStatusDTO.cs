namespace Student_API.DTOs
{
    public class StudentStatusDTO
    {
        public int StudentID { get; set; }
        public string InactiveReason { get; set; } // Nullable string for InactiveReason
        public int ActivityStatusID { get; set; }
        public int UserID { get; set; }
        public int InstituteID { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}
