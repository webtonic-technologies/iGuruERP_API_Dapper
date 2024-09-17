namespace Institute_API.DTOs
{
    public class SubjectRequest
    {

        public int InstituteId { get; set; }
        public List<Subjects>? Subjects { get; set; }
        public List<SubjectSectionMappingRequest>? SubjectSectionMappingRequests { get; set; }
    }
    public class Subjects
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public int subject_type_id { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class SubjectSectionMappingRequest
    {

        public int CSSMappingId { get; set; }
        public int SubjectId { get; set; }
        public int class_id { get; set; }
        public string section_id { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
    public class SubjectSectionMappingResponse
    {

        public int CSSMappingId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int class_id { get; set; }
        public string className { get; set; } = string.Empty;
        public List<Sections>? Section { get; set; }
    }

    public class Sections
    {
        public int section_id { get; set; }
        public string sectionName { get; set; } = string.Empty;
    }
    public class SubjectResponse
    {
        public int SubjectId { get; set; }
        public int InstituteId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public int subject_type_id { get; set; }
        public string subject_type_name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public List<SubjectSectionMappingResponse>? SubjectSectionMappings { get; set; }
    }
    public class GetAllSubjectRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Institute_id { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public int ClassId {  get; set; }
        public int SectionId {  get; set; }
    }
    public class SubjectType
    {
        public int subject_type_id { get; set; }
        public string subject_type { get; set; } = string.Empty;
    }
    public class ExcelDownloadRequest
    {
        public int InstituteId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
    }
    public class ExcelDownloadSubStudentMappingRequest
    {
        public int InstituteId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int SubjectTypeId {  get; set; }
    }
}