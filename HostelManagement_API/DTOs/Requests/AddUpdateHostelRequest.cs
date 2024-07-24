using System.Collections.Generic;

namespace HostelManagement_API.DTOs.Requests
{
    public class AddUpdateHostelRequest
    {
        public int? HostelID { get; set; }
        public string HostelName { get; set; }
        public int HostelTypeID { get; set; }
        public string HostelPhoneNo { get; set; }
        public string HostelWarden { get; set; }
        public string Address { get; set; } // Added Address field
        public List<int> BlockIDs { get; set; }
        public List<int> BuildingIDs { get; set; }
        public List<int> FloorIDs { get; set; }
        public string Attachments { get; set; } // Path to the uploaded file
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
