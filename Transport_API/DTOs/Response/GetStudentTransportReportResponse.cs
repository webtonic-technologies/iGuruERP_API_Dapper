namespace Transport_API.DTOs.Response
{
    public class GetStudentTransportReportResponse
    { 
            public int VehicleID { get; set; }
            public string VehicleNo { get; set; }
            public string VehicleType { get; set; }
            public string CoordinatorName { get; set; }
            public string DriverName { get; set; }
            public string DriverNumber { get; set; }
            public int TotalCount { get; set; }
            public List<StudentDetails> Students { get; set; } 
    }
    public class StudentDetails
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string ClassSection { get; set; }
        public string RollNumber { get; set; }
        public string FatherName { get; set; }
        public string MobileNumber { get; set; }
        public string StopName { get; set; }
        public string TransportFee { get; set; }

        
    }
    public class VehicleDetails
    {
        public int VehicleID { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleType { get; set; }
        public string CoordinatorName { get; set; }
        public string DriverName { get; set; }
        public string DriverNumber { get; set; }
    }

}
