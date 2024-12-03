namespace Student_API.DTOs.RequestDTO
{
    public class GetStudentRequestModel : PagedListModel
    {
        public int Institute_id {  get; set; }
        public int class_id { get; set; } = 0;
        public int section_id { get; set; } = 0;
        public string AcademicYearCode { get; set; }
        public int StudentType_id { get; set; } = 0;
        //public bool isActive { get; set; } = true;
        public string Keyword { get; set; }
        public int StudentStatus { get; set; } = 3; // 1 = Active, 2 = Inactive, 3 = All

    }
    public class GetStudentProfileRequestModel : PagedListModel
    {
        public int Institute_id { get; set; }
        public int class_id { get; set; } = 0;
        public int section_id { get; set; } = 0;
        public string AcademicYearCode { get; set; }
        public int StudentType_id { get; set; } = 0;
        public bool isActive { get; set; } = true;
        public string Keyword { get; set; } 
    }

    public class UpdateProfile
    {
        public int requestId { get; set; }
        public int newStatus { get; set; }
    }
    public class AddProfileUpdate
    {
        public int studentId { get; set; }
        public int status { get; set; }
    }
}
