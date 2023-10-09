using Newtonsoft.Json;
using PlanePal.Enums;
using System.Text.Json.Serialization;

namespace PlanePal.DTOs.ScheduledFlight
{
    public class ScheduledFlightDTO
    {
        [JsonProperty("flight_date")]
        public DateTime Flight_date { get; set; }

        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonProperty("flight_status")]
        public FlightStatus Flight_status { get; set; }

        [JsonProperty("departure")]
        public DepartureDTO Departure { get; set; }

        [JsonProperty("arrival")]
        public ArrivalDTO Arrival { get; set; }

        [JsonProperty("airline")]
        public AirLineDTO Airline { get; set; }

        [JsonProperty("flight")]
        public FlightDTO Flight { get; set; }
    }
}