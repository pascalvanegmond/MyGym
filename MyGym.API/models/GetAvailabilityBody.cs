using Newtonsoft.Json;

namespace MyGym.API.models
{
    public class GetAvailabilityBody
    {
        [JsonProperty("clubId")]
        public string ClubId { get; set; }
        
        [JsonProperty("dateTime")]
        public string DateTime { get; set; }
    }
}