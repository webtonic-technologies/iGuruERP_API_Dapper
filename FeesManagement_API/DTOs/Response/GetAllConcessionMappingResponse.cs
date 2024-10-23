namespace FeesManagement_API.DTOs.Responses
{
    public class GetAllConcessionMappingResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string AdmissionNumber { get; set; }
        public int ConcessionGroupID { get; set; }
        public string ConcessionGroupType { get; set; }
    }
}
