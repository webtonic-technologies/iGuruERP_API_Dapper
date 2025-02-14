using StudentManagement_API.DTOs.Responses;

namespace StudentManagement_API.DTOs.Response.StudentManagement
{
    public class DownloadStudentImportTemplateResponse
    {
        public List<ClassResponse> Classes { get; set; } = new List<ClassResponse>();
        public List<SectionResponse> Sections { get; set; } = new List<SectionResponse>();
        public List<GenericResponse> Genders { get; set; } = new List<GenericResponse>();
        public List<GenericResponse> Religions { get; set; } = new List<GenericResponse>();
        public List<GenericResponse> Nationalities { get; set; } = new List<GenericResponse>();
        public List<GenericResponse> MotherTongues { get; set; } = new List<GenericResponse>();
        public List<GenericResponse> BloodGroups { get; set; } = new List<GenericResponse>();
        public List<GenericResponse> Castes { get; set; } = new List<GenericResponse>();
        public List<GenericResponse> InstituteHouses { get; set; } = new List<GenericResponse>();
        public List<GenericResponse> StudentTypes { get; set; } = new List<GenericResponse>();
        public List<GenericResponse> ParentTypes { get; set; } = new List<GenericResponse>();

    }
}
