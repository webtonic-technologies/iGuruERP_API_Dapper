namespace Student_API.DTOs.RequestDTO
{
    public class GetStudentRequestModel : PagedListModel
    {
        public int Institute_id {  get; set; }
        public int class_id { get; set; } = 0;
        public int section_id { get; set; } = 0;
        public int Academic_year_id { get; set; } = 0;
        public int StudentType_id { get; set; } = 0;
        public bool isActive { get; set; } = true;
    }
    public class GetStudentProfileRequestModel : PagedListModel
    {
        public int Institute_id { get; set; }
        public int class_id { get; set; } = 0;
        public int section_id { get; set; } = 0;
        public int Academic_year_id { get; set; } = 0;
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
