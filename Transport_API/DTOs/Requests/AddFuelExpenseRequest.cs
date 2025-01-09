namespace Transport_API.DTOs.Requests
{
    public class AddFuelExpenseRequest
    {
        public int VehicleFuelExpenseID { get; set; }
        public int VehicleID { get; set; }
        public decimal CurrentReadingInKM { get; set; }
        public decimal PreviousReading { get; set; }
        public decimal DistanceTravelledInKM { get; set; }
        public decimal PreviousFuelAddedInLitre { get; set; }
        public decimal FuelAddedInLitre { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string ExpenseDate { get; set; }
        public string Remarks { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<AttachmentRequest> Attachments { get; set; }
    }

    public class AttachmentRequest
    {
        public int VehicleExpenseDocumentID { get; set; }
        public string Attachment { get; set; }
        public int VehicleFuelExpenseID { get; set; }
    }
}
