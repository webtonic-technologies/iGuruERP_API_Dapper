﻿namespace HostelManagement_API.DTOs.Requests
{
    public class AddUpdateCautionDepositRequest
    {
        public int? CautionFeeID { get; set; }
        public string AcademicYear { get; set; }
        public string FeeMasterName { get; set; }
        public decimal HostelCautionFee { get; set; }
        public int FeeFrequencyID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
