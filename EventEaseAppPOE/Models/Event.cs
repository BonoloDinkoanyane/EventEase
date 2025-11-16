using System;
using System.Collections.Generic;

namespace EventEaseAppPOE.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly EventDate { get; set; }

    public int? EventTypeId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual EventType? EventType { get; set; }
}
