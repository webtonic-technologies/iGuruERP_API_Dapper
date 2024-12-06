namespace Communication_API.DTOs.Responses.PushNotification
{
    public class PushNotificationStudentsResponse
    {
        public int GroupID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public string AdmissionNumber { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public bool IsActive { get; set; }
    }
}
