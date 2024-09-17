namespace Student_API.DTOs
{
    public class StudentCredentialsResponse
    {
        public string AdmissionNumber { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string LoginId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string LastActionTaken { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;
    }
    public class StudentsNonAppUsersResponse
    {
        public string AdmissionNumber { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
    }
    public class StudentActivityResponse
    {
        public string AdmissionNumber { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string LastActionTaken { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;
    }
}
