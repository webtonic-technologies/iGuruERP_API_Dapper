﻿namespace FeesManagement_API.DTOs.Requests
{
    public class GetRefundRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string Search { get; set; }
    }
}