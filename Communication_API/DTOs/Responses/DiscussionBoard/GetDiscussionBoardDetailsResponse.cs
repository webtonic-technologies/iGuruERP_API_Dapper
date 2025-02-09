namespace Communication_API.DTOs.Responses.DiscussionBoard
{
    public class GetDiscussionBoardDetailsResponse
    {
        // Formatted as "26-04-2024 to 28-04-2024"
        public string Date { get; set; }
        // Comma‑separated list of student mappings, e.g. "ClassA-SectionA, ClassA-SectionB"
        public string Student { get; set; }
        // Comma‑separated list of employee mappings, e.g. "Department1-Designation1, Department2-Designation2"
        public string Employee { get; set; }
        // The full name of the employee who created the discussion board
        public string CreatedBy { get; set; }
        // Formatted as "26th Apr 2024, 07:00 PM"
        public string CreatedOn { get; set; }
    }
}
