using Newtonsoft.Json;

namespace MyGym.API.models
{
    public class DoorPolicy
    {
        [JsonProperty("doorPolicyId")]
        public string DoorPolicyId { get; set; }
        
        [JsonProperty("startDateTime")]
        public string StartDateTime { get; set; }
        
        [JsonProperty("openForReservation")]
        public bool OpenForReservation { get; set; }
    }
}