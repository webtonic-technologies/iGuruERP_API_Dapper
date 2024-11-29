namespace Transport_API.DTOs.Requests
{
    public class GetAllExpenseRequest
    {
        public string StartDate { get; set; } // Keep the date as string to handle DD-MM-YYYY format
        public string EndDate { get; set; }   // Keep the date as string to handle DD-MM-YYYY format
        public int? VehicleID { get; set; }
        public int? ExpenseTypeID { get; set; }
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    namespace Transport_API.DTOs.Requests
    {
        public class VehicleExpenseRequest
        {
            public int VehicleExpenseID { get; set; }
            public int VehicleID { get; set; }
            public int VehicleExpenseTypeID { get; set; }
            public string ExpenseDate { get; set; } // Formatted as DD-MM-YYYY
            public decimal Cost { get; set; }
            public string Remarks { get; set; }
            public int InstituteID { get; set; }
            public bool IsActive { get; set; }
            public List<VehicleExpenseDocumentRequest> Attachments { get; set; } // Attachment list for documents
        }

        public class VehicleExpenseDocumentRequest
        {
            public int VehicleExpenseDocumentID { get; set; }
            public string Attachment { get; set; } // This is the base64 encoded document
            public int VehicleExpenseID { get; set; } 
        }
    }

}
