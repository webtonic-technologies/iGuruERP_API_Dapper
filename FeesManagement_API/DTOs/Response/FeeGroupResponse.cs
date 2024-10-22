namespace FeesManagement_API.DTOs.Response
{
    public class FeeGroupResponse
    {
        public int FeeGroupID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int FeeHeadID { get; set; }
        public string FeeHead { get; set; } = string.Empty;
        public int FeeTenurityID { get; set; }
        public string FeeTenurityType { get; set; } = string.Empty;
        public List<ClassSectionResponse> ClassSections { get; set; } = new List<ClassSectionResponse>();
    }

    public class ClassSectionResponse
    {
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
    }
}
