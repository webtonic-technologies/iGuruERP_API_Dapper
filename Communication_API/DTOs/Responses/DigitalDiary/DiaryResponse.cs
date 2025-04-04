﻿namespace Communication_API.DTOs.Responses.DigitalDiary
{
    public class DiaryResponse
    {
        public int DiaryID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string ClassSection { get; set; }
        public string Subject { get; set; }
        public string DiaryRemarks { get; set; }
        public string ShareOn { get; set; }
        public string GivenBy { get; set; }
    }

}
