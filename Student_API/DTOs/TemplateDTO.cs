﻿namespace Student_API.DTOs
{
    public class TemplateDTO
    {
        public int Template_Type_Id { get; set; }
        public string Template_Name { get; set; }
        public int UserId { get; set; }
        
    }
    public class TemplateResponseDTO
    {
        public int Template_Type_Id { get; set; }
        public string Template_Name { get; set; }
        public int UserId { get; set; }
        public string CreatedDate { get; set; }

    }
}
