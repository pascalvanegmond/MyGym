using Newtonsoft.Json;

namespace MyGym.API.models
{
    public class BookReservationClubDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("country")]
        public string Country { get; set; }
        
        [JsonProperty("bookable")]
        public bool Bookable { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("debt_check")]
        public bool DebtCheck { get; set; }
        
        [JsonProperty("label_name")]
        public string LabelName { get; set; }
    }
}