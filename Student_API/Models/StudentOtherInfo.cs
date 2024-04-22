namespace Student_API.Models
{
    public class StudentOtherInfo
    {
        public int Student_Other_Info_id { get; set; }
        public int? student_id { get; set; }
        public int? StudentType_id { get; set; }
        public string email_id { get; set; }
        public string Hall_Ticket_Number { get; set; }
        public int? Exam_Board_id { get; set; }
        public string Identification_Mark_1 { get; set; }
        public string Identification_Mark_2 { get; set; }
        public DateTime? Admission_Date { get; set; }
        public int? Student_Group_id { get; set; }
        public DateTime? Register_Date { get; set; }
        public string Register_Number { get; set; }
        public string samagra_ID { get; set; }
        public string Place_of_Birth { get; set; }
        public string comments { get; set; }
        public string language_known { get; set; }
    }
}
