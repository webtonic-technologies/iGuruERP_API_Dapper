namespace Student_API.DTOs
{
    public class StudentDocumentInfo
    {
        public int Student_id { get; set; }           
        public string StudentName { get; set; }      
        public string Admission_Number { get; set; }   
        public string Class_Name { get; set; }        
        public string Section_Name { get; set; }      

 
        public Dictionary<string, DocumentStatusInfo> DocumentStatus { get; set; } = new Dictionary<string, DocumentStatusInfo>();

        public StudentDocumentInfo()
        {
            DocumentStatus = new Dictionary<string, DocumentStatusInfo>();
        }
    }

    public class DocumentStatusInfo
    {
        
        public int Student_Document_id { get; set; }            
        public bool IsSubmitted { get; set; }         
    }
}
