﻿namespace HostelManagement_API.DTOs.Requests
{
    public class GetAllCautionDepositRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
