﻿namespace Attendance_API.DTOs
{
    public class IMEIRegistrationModel
    {
        public int IMEIRegistration_Id { get; set; }
        public string Employee_Id { get; set; }
        public string IMEI_Number { get; set; }
        public bool IsReset { get; set; }
    }
}
