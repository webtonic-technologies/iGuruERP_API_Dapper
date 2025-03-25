namespace StudentManagement_API.DTOs.Responses
{
    public class GetCertificateTagValueResponse
    {
        // For single-record tags (IsMultiRecord = 0)
        public string TagValue { get; set; }

        // For multi-record tags (IsMultiRecord = 1)
        public List<TagValueItem> TagValues { get; set; }
    }

    // Helper class to wrap each item for multi-record tags
    public class TagValueItem
    {
        public string TagValue { get; set; }
    }
}





//namespace StudentManagement_API.DTOs.Responses
//{
//    public class GetCertificateTagValueResponse
//    {
//        public string TagValue { get; set; }
//    }
//}
