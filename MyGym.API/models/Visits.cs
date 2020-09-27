using System.Collections.Generic;

namespace MyGym.API.models
{
    public class Visits
    {
        public int TotalVisits { get; set; }
        public List<VisitClubDetails> VisitClubDetails { get; set; }
        public List<Visit> Visit { get; set; } 
    }
}