namespace Institute_API.DTOs
{
    public class AcaConfigSubStudentRequest
    {
        public int InstituteId { get; set; }
        public int StudentId { get; set; }
        public List<SubStudentMappingReq>? SubStudentMappingReqs { get; set; }
    }
    public class SubStudentMappingReq
    {
        public int SSMappingId { get; set; }
        //  public int InstituteId { get; set; }
        //  public int StudentId { get; set; }
        public int SubjectId { get; set; }
    }
    public class SubjectList
    {
        public int SubjectId { get; set; }
        public int InstituteId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public int subject_type_id { get; set; }
        public string SubjectTypeName { get; set; } = string.Empty;
    }
    public class StudentListRequest
    {
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public int Institute_id { get; set; }
    }
    public class StudentListResponse
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public string AdmissionNumber { get; set; } = string.Empty;
    }
    public class MappingListRequest
    {
        public int InstituteId { get; set; }
        public int SubjectTypeId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
    }
    public class AcaConfigSubStudentResponse
    {
        public int InstituteId { get; set; }
        public List<StudentSubjectResponse>? StudentSubjectResponses {  get; set; }
    }
    public class StudentSubjectResponse
    {
        public int subjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<Students>? Students {  get; set; }
    }
    public class Students
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
    }
}