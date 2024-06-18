namespace Student_API.DTOs
{
    public class TemplateDTO
    {
        public int Template_Type_Id { get; set; }
        public string Template_Name { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
