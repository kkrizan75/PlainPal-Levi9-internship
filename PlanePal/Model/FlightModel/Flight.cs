using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanePal.Model.FlightModel
{
    public class Flight
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }

        public string Number { get; set; }
        public string Iata { get; set; }
        public string Icao { get; set; }
    }
}