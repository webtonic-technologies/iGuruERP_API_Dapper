namespace Lesson_API.DTOs.Responses
{
    public class GetHomeworkHistoryResponse
    {
        public int TotalStudents { get; set; }
        public int Submitted { get; set; }
        public int NotSubmitted { get; set; }
        public int Checked { get; set; }
        public int NotChecked { get; set; }
        public List<StudentHomeworkStatus> Students { get; set; }
    }

    
    public class StudentHomeworkStatus
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }


        public int SeenStatus { get; set; }  // Keep this as int for comparison
        public string SeenStatusDisplay { get; set; }  // This is where "Seen" or "Unseen" will be stored
        public DateTime? SeenDate { get; set; }
        public string SeenDateTime { get; set; }  // This is where formatted date will be stored



        public int SubmittedStatus { get; set; }  // Keep this as int for comparison
        public string SubmittedStatusDisplay { get; set; }  // This is where "Submitted" or "Not Submitted" will be stored
        public DateTime? SubmittedDate { get; set; }
        public string SubmittedDateTime { get; set; }  // This is where formatted date will be stored
         

        public int CheckedStatus { get; set; }  // Keep this as int for comparison
        public string CheckedStatusDisplay { get; set; }  // This is where "Checked" or "Not Checked" will be stored
        public DateTime? CheckedDate { get; set; }  
        public string CheckedDateTime { get; set; }  // This is where formatted date will be stored
    } 





}
