using System;
using System.Collections.Generic;

namespace EventEaseAppPOE.Models;

public partial class BookingView
{
    public int BookingId { get; set; }

    public DateOnly BookingDate { get; set; }

    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public DateOnly EventDate { get; set; }

    public string EventDescription { get; set; } = null!;

    public int VenueId { get; set; }

    public string VenueName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int Capacity { get; set; }

    public string VenueImage { get; set; } = null!;
}
