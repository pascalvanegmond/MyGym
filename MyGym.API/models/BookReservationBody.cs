using Newtonsoft.Json;

namespace MyGym.API.models
{
    public class BookReservationBody
    {
        [JsonProperty("doorPolicy")]
        public DoorPolicy DoorPolicy { get; set; }
        
        [JsonProperty("duration")]
        public string Duration { get; set; }
        
        [JsonProperty("clubOfChoice")]
        public BookReservationClubDetails ClubOfChoice { get; set; }
    }
}