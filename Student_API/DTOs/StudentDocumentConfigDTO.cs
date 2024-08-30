using Student_API.Helper;
using System.ComponentModel.DataAnnotations;

namespace Student_API.DTOs
{
    public class StudentDocumentConfigDTO
    {
        public int Student_Document_id { get; set; }
        public string Student_Document_Name { get; set; }
        
        public string en_date { get; set; }
        public int Institute_id {  get; set; }  
    }

    public class StudentDocumentConfig
    {
        public int Student_Document_id { get; set; }
        public string Student_Document_Name { get; set; }

        public int Institute_id { get; set; }
    }
}
