namespace Student_API.DTOs
{
    public class ProfileUpdateRequestDTO
    {
        public int Id { get; set; }
        public int Student_Id { get; set; }
        public string Student_Name { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string Roll_Number { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string Religion { get; set; }
        public string Father_Name { get; set; }
        public int Status { get; set; }  // 0: Pending, 1: Approved, 2: Rejected
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }

}
