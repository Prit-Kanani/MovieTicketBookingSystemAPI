using System;
using System.Collections.Generic;

namespace Movie_Management_API.Models;

public partial class SeatsBooked
{
    public int BookingId { get; set; }

    public string SeatNo { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;
}
