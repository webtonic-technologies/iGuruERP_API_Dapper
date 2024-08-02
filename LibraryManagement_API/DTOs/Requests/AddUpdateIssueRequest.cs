namespace LibraryManagement_API.DTOs.Requests
{
    public class AddUpdateIssueRequest
    {
        public int IssueID { get; set; }
        public int InstituteID { get; set; }
        public int CatalogueID { get; set; }
        public int BorrowerID { get; set; }
        public int BorrowerTypeID { get; set; }  // 1 = Student, 2 = Employee, 3 = Guest
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string GuestName { get; set; }
        public string GuestType { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
    }
}
