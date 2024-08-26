namespace Student_API.DTOs
{
    public class IMEIRegistrationDto
    {
        public int IMEIRegistration_Id { get; set; }
        public string Employee_Id { get; set; }
        public string Employee_Name { get; set; }
        public string Mobile_Number { get; set; }
        public string IMEI_Number { get; set; }
        public bool IsReset { get; set; }
    }

}
