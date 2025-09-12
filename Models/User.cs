using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie_Management_API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    [Column("IsActive")]
    public bool IsActive { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Theatre> Theatres { get; set; } = new List<Theatre>();
}
