﻿namespace FeesManagement_API.DTOs.Requests
{
    public class GetWalletRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
}
