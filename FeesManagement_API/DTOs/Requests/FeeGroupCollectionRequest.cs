namespace FeesManagement_API.DTOs.Requests
{
    public class FeeGroupCollectionRequest
    {
        public int FeeCollectionID { get; set; }
        public int FeeTenurityID { get; set; }

        public TenuritySingleRequest? TenuritySingle { get; set; }
        public TenurityTermRequest? TenurityTerm { get; set; }
        public TenurityMonthlyRequest? TenurityMonthly { get; set; }
    }
}
