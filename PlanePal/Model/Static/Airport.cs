using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanePal.Model.Static
{
    public class Airport
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }

        public string Name { get; set; }
        public string TimeZone { get; set; }
        public string Icao { get; set; }
        public string Country { get; set; }
    }
}