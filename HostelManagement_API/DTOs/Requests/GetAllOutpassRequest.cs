﻿namespace HostelManagement_API.DTOs.Requests
{
    public class GetAllOutpassRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
