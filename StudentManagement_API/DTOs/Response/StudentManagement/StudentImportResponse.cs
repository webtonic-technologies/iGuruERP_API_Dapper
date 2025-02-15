namespace StudentManagement_API.DTOs.Response.StudentManagement
{
    public class StudentImportResponse
    {
         
        public bool Success { get; set; } 
        public string Message { get; set; } 
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
