using System;
using System.Collections.Generic;

namespace Movie_Management_API.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Name { get; set; } = null!;

    public string Language { get; set; }

    public int Duration { get; set; }

    public string Poster { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
