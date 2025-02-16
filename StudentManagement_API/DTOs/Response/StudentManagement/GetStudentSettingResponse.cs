namespace StudentManagement_API.DTOs.Responses
{
    public class GetStudentSettingResponse
    {
        public int StudentColumnID { get; set; }
        public string ScreenFieldName { get; set; }
        public string Status { get; set; } // "1" if the mapping exists; "0" otherwise.
    }
}
