namespace FeesManagement_API.DTOs.Requests
{
    public class AddUpdateConcessionMappingRequest
    {
        public int InstituteID { get; set; }
        public int StudentID { get; set; }
        public int ConcessionGroupID { get; set; }
    }
}
