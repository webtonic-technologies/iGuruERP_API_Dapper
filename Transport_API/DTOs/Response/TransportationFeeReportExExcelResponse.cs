namespace Transport_API.DTOs.Responses
{
    public class TransportationFeeReportExExcelResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string ClassSection { get; set; }
        public string RollNumber { get; set; }
        public string FatherName { get; set; }
        public string Mobile { get; set; }
        public string StopName { get; set; }
        public List<FeeDetail1> Fees { get; set; }
    }

    public class FeeDetail1
    {
        public string Head { get; set; }
        public string VehicleType { get; set; }
        public string TenureType { get; set; }
        public decimal FeesAmount { get; set; }
    }
}
