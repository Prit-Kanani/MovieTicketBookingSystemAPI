using System;
using System.Collections.Generic;

namespace Movie_Management_API.Models;

public partial class Theatre
{
    public int TheatreId { get; set; }

    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? Address { get; set; }

    public int UserId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Screen> Screens { get; set; } = new List<Screen>();

    public virtual User User { get; set; } = null!;
}
