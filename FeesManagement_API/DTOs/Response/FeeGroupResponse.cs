namespace FeesManagement_API.DTOs.Response
{
    public class FeeGroupResponse
    {
        public int FeeGroupID { get; set; }
        public string GroupName { get; set; }
        public int FeeHeadID { get; set; }
        public string FeeHead { get; set; }
        public int FeeTenurityID { get; set; }
        public string FeeTenurityType { get; set; }
        public string ClassSection { get; set; }
    }
}
