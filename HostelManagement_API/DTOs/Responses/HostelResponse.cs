using HostelManagement_API.DTOs.Requests;
using System.Collections.Generic;

namespace HostelManagement_API.DTOs.Responses
{
    public class HostelResponse
    {
        public int HostelID { get; set; }
        public string HostelName { get; set; }
        public int HostelTypeID { get; set; }
        public string HostelType { get; set; } // HostelType from tblHostelType
        public string Address { get; set; }   
        public string PhoneNo { get; set; }
        public string HostelWarden { get; set; } 
        public string Block { get; set; }
        public string Building { get; set; }
        public string Floors { get; set; }
        public List<HostelDocs>? HostelDocs { get; set; }
    }
}
