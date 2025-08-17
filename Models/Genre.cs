using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Movie_Management_API.Models;

public partial class Genre
{
    public int GenreId { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
