using System.ComponentModel.DataAnnotations;

namespace Student_API.DTOs
{
    public class StudentDocumentConfigDTO
    {
        public int Student_Document_id { get; set; }
        public string Student_Document_Name { get; set; }
        //public DateTime en_date { get; set; }
        public int Institute_id {  get; set; }  
    }
}
