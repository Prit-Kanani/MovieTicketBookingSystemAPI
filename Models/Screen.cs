using System;
using System.Collections.Generic;

namespace Movie_Management_API.Models;

public partial class Screen
{
    public int ScreenId { get; set; }

    public int TheatreId { get; set; }

    public int ScreenNo { get; set; }

    public int TotalSeats { get; set; }

    public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

    public virtual Theatre Theatre { get; set; } = null!;
}
