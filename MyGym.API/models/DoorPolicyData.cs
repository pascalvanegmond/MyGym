using Newtonsoft.Json;

namespace MyGym.API.models
{
    public class DoorPolicyData
    {
        [JsonProperty("doorPolicyPeopleId")]
        public string DoorPolicyPeopleId { get; set; } 
        
        [JsonProperty("clubId")]
        public string ClubId { get; set; } 
    }
}