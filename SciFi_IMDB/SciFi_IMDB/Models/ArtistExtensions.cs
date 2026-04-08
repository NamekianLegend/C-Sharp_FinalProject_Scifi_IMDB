using System;
using System.Collections.Generic;
using System.Text;

namespace A2_Chinook_EFandLINQ
{
    public partial class Artist
    {
        public int TrackCount => Albums?.Sum(a => a.Tracks?.Count ?? 0) ?? 0;
    }
}