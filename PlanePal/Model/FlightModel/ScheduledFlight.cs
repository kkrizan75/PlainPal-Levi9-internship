using PlanePal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PlanePal.Model.FlightModel
{
    public class ScheduledFlight
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }

        public DateTime Flight_date { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FlightStatus Flight_status { get; set; }

        [ForeignKey("Departure")]
        public int DepartureId { get; set; }

        public Departure Departure { get; set; }

        [ForeignKey("Arrival")]
        public int ArrivalId { get; set; }

        public Arrival Arrival { get; set; }

        [ForeignKey("Airline")]
        public int AirlineId { get; set; }

        public FlightAirline Airline { get; set; }

        [ForeignKey("Flight")]
        public int FlightId { get; set; }

        public Flight Flight { get; set; }
    }
}