using System;
using System.Collections.Generic;

namespace SciFi_IMDB;

public partial class Name
{
    public string NameId { get; set; } = null!;

    public string? PrimaryName { get; set; }

    public int? BirthYear { get; set; }

    public int? DeathYear { get; set; }

    public string? PrimaryProfession { get; set; }
    public List<string>? KnownForTitles { get; set; }
    public string KnownForDisplay => KnownForTitles != null ? string.Join(", ", KnownForTitles) : "None";

    public virtual ICollection<Principal> Principals { get; set; } = new List<Principal>();
}
