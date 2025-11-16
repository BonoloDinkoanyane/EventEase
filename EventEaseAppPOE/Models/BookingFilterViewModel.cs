namespace EventEaseAppPOE.Models
{
    public class BookingFilterViewModel
    {
        public string? SelectedEventType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? VenueAvailable { get; set; }

        public List<string> EventTypes { get; set; } = new();
        public List<Booking> FilteredBookings { get; set; } = new();
    }
}
