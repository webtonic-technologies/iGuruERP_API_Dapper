﻿namespace StudentManagement_API.DTOs.Requests
{
    public class GetCertificateTemplateRequest
    {
        public int InstituteID { get; set; }
        public string Search { get; set; } 
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
