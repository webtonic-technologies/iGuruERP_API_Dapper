namespace StudentManagement_API.DTOs.Responses
{
    public class GetCertificateStudentTagsResponse
    {
        public string ColumnDisplayName { get; set; }
        public string ColumnFieldName { get; set; }
        public string Value { get; set; }
    }

    public class CertificateStudentTagDto
    {
        public string Group { get; set; }
        public string ColumnDisplayName { get; set; }
        public string ColumnFieldName { get; set; }
        public string Value { get; set; }
    }
}
