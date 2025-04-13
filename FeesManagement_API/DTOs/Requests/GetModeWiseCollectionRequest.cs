namespace FeesManagement_API.DTOs.Requests
{
    public class GetModeWiseCollectionRequest
    {
        public int InstituteID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
