using Newtonsoft.Json;

namespace MyGym.API.models
{
    public class CancelReservationBody
    {
        [JsonProperty("doorPolicyPeopleId")]
        public string DoorPolicyPeopleId { get; set; } 
    }
}