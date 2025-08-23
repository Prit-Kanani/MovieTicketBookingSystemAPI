using System;
using System.Collections.Generic;

namespace Movie_Management_API.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int ShowId { get; set; }

    public string BookingType { get; set; } = null!;

    public DateTime? DateTime { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<SeatsBooked> SeatsBookeds { get; set; } = new List<SeatsBooked>();

    public virtual ShowTime Show { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
