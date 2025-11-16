using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventEaseAppPOE.Models;

public partial class Venue
{
    public int VenueId { get; set; }

    public string VenueName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int Capacity { get; set; }

   // [Required(ErrorMessage = "Please select an image.")]
    public String? VenueImage { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
