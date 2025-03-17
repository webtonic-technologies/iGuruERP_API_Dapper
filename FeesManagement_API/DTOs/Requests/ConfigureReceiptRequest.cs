using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Requests
{
    public class ConfigureReceiptRequest
    {
        public int LayoutID { get; set; }
        public int ReceiptTypeID { get; set; } 
        public string ReceiptLogo { get; set; }
        public int InstituteID { get; set; } 
        public List<int> Components { get; set; } 
        public List<int> Properties { get; set; }
    }
}
