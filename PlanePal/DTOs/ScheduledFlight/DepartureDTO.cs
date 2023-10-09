using Newtonsoft.Json;

namespace PlanePal.DTOs.ScheduledFlight
{
    public class DepartureDTO
    {
        public string Airport { get; set; }
        public string Timezone { get; set; }
        public string Iata { get; set; }
        public string Terminal { get; set; }
        public string Gate { get; set; }

        [JsonProperty("scheduled")]
        public DateTime Scheduled { get; set; }
    }
}