﻿namespace Student_API.DTOs
{
    public class StudentDocumentsDTO
    {
        public int Student_id { get; set; }
        public List<IFormFile> formFiles { get; set; }  
        //public List<StudentDocumentListDTO> studentDocumentListDTO { get; set; }  

    }
    public class StudentDocumentListDTO
    {
		public int Student_id { get; set; }
		public int Student_Documents_id { get; set; }
        public string Document_Name { get; set; }
        public string File_Name { get; set; }
        public string File_Path { get; set; }
        public IFormFile formFile { get; set; }
    }
}