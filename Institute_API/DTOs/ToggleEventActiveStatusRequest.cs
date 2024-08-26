namespace Institute_API.DTOs
{
    public class ToggleEventActiveStatusRequest
    {
        public int EventId {  get; set; }       
        public int Status { get; set; }
        public int UserId { get; set; }
    }
}
