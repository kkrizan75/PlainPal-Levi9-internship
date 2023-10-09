using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanePal.Model.FlightModel
{
    public class Arrival
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }

        public string Airport { get; set; }
        public string Timezone { get; set; }
        public string Iata { get; set; }
        public string Terminal { get; set; }
        public string Gate { get; set; }
        public DateTime Scheduled { get; set; }
    }
}