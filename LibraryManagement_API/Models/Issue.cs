namespace LibraryManagement_API.Models
{
    public class Issue
    {
        public int IssueID { get; set; }
        public int InstituteID { get; set; }
        public int CatalogueID { get; set; }
        public int EmployeeID { get; set; }  // or StudentID or GuestID based on the issue type
        public DateTime DueDate { get; set; }
        public string BorrowerType { get; set; }  // Employee, Student, Guest
        public bool? IsActive { get; set; }
    }
}
