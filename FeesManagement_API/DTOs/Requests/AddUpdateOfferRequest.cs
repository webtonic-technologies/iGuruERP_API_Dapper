﻿namespace Configuration.DTOs.Requests
{
    public class AddUpdateOfferRequest
    {
        public int OfferID { get; set; }
        public string OfferName { get; set; }
        public string AcademicYear { get; set; }
        public string OpeningDate { get; set; } // Change to string to handle 'DD-MM-YYYY' format
        public string ClosingDate { get; set; } // Change to string to handle 'DD-MM-YYYY' format
        public int StudentTypeID { get; set; }
        public bool isAmount { get; set; }
        public bool isPercentage { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
        public List<OfferFeeHeadMappingRequest> OfferFeeHeadMappings { get; set; }
        public List<OfferFeeTenureMappingRequest> OfferFeeTenureMappings { get; set; }
        public List<OfferClassSectionMappingRequest> OfferClassSectionMappings { get; set; }
    }

    public class OfferFeeHeadMappingRequest
    {
        public int FeeHeadID { get; set; }
    }

    public class OfferFeeTenureMappingRequest
    {
        public int FeeTenurityID { get; set; }
        public int? STMTenurityID { get; set; } // Nullable to handle optional values
        public int? FeeCollectionID { get; set; } // Nullable to handle optional values
    }

    public class OfferClassSectionMappingRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
}
