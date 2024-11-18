namespace Transport_API.DTOs.Responses
{
    public class GetStudentsForRouteMappingResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public string AdmissionNumber { get; set; } 
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }
     
}
