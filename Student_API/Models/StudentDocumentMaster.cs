namespace Student_API.Models
{
    public class StudentDocumentMaster
    {
        public int Student_Document_id { get; set; }
        public string Student_Document_Name { get; set; }
        public DateTime en_date { get; set; }
    }

    public class StudentDocuments
    {
        public int Student_Documents_id { get; set; }
        public int Student_id { get; set; }
        public string Document_Name { get; set; }
        public string File_Name { get; set; }
        public string File_Path { get; set; }
    }
}
