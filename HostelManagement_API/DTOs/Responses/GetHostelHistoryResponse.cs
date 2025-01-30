namespace HostelManagement_API.DTOs.Responses
{
    public class GetHostelHistoryResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string ClassSection { get; set; }
        public string RollNumber { get; set; }
        public string AdmissionNumber { get; set; } 
        public List<StudentHostelHistory> StudentHostels { get; set; }
    }

    public class StudentHostelHistory
    {
        public int HostelID { get; set; }
        public string HostelName { get; set; }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string? AllocateDate { get; set; }
        public string? VacateDate { get; set; }
        public string AllotterName { get; set; }
    }
}
