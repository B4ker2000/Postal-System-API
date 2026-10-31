namespace PostalSystem.Api.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string EventTypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EventCategory Category { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}