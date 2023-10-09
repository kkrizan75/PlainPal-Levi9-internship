using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanePal.Model.Static
{
    public class Airline
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Iata { get; set; }
        public string Country { get; set; }
    }
}