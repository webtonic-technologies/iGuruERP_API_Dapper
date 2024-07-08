namespace Transport_API.DTOs.Requests
{
    public class GetAllExpenseRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteID { get; set; }
    }

    public class VehicleExpenseRequest
    {
        public int VehicleExpenseID { get; set; }
        public int ExpenseTypeID { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Cost { get; set; }
        public string Remarks { get; set; }
        public int VehicleID { get; set; }
        public int InstituteID { get; set; }
        public List<VehicleExpenseDocumentRequest> VehicleExpenseDocuments { get; set; }

    }

    public class VehicleExpenseDocumentRequest
    {
        public int VehicleExpenseDocumentID { get; set; }
        public string VehicleExpenseDocument { get; set; }
        public int VehicleExpenseID { get; set; }
    }
}
