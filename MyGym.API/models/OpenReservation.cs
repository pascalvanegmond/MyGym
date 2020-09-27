using System.Collections.Generic;

namespace MyGym.API.models
{
    public class OpenReservation
    {
        public List<DoorPolicyData> Data { get; set; }
        public string Duration { get; set; }
    }
    
}