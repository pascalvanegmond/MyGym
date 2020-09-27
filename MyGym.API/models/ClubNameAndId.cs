using Newtonsoft.Json;

namespace MyGym.API.models
{
    public class ClubNameAndId
    {
        [JsonProperty("name")]
        public string ClubName { get; set; }
        
        [JsonProperty("id")]
        public string ClubId { get; set; }
    }
}