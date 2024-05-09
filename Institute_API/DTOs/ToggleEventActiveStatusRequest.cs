namespace Institute_API.DTOs
{
    public class ToggleEventActiveStatusRequest
    {
        public int EventId {  get; set; }       
        public bool IsActive { get; set; }
        public int UserId { get; set; }
    }
}
