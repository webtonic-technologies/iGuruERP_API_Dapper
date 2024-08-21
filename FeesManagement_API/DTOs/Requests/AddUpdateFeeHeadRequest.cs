using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Requests
{
    public class AddUpdateFeeHeadRequest
    {
        public List<FeeHeadRequest> FeeHeads { get; set; } = new List<FeeHeadRequest>();
    }

    public class FeeHeadRequest
    {
        public int FeeHeadID { get; set; }
        public string FeeHeadName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public int RegTypeID { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
    }
}
