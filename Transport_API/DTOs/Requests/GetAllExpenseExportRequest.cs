namespace Transport_API.DTOs.Requests
{
    public class GetAllExpenseExportRequest
    {
        public string StartDate { get; set; }  // Format 'DD-MM-YYYY'
        public string EndDate { get; set; }    // Format 'DD-MM-YYYY'
        public int VehicleID { get; set; }
        public int ExpenseTypeID { get; set; }
        public int InstituteID { get; set; }
        public int ExportType { get; set; }
    }
}
