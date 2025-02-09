namespace Communication_API.DTOs.Requests
{
    public class GetStudentListRequest
    {
        public int GroupID { get; set; }
        /// <summary>
        /// 1: Active, 2: Inactive, 3: Both
        /// </summary>
        public int Status { get; set; }
        public int InstituteID { get; set; }
    }
}
