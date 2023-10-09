using PlanePal.Enums;
using PlanePal.Model.UserModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanePal.Model.FlightModel
{
    public class BookedFlight
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserEmail { get; set; }

        public User User { get; set; }
        public DateTime DepartureDate { get; set; }
        public FlightStatus FlightStatus { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public string FlightIcao { get; set; }
        public int TicketQuantity { get; set; }
        public DateTime ArrivalDate { get; set; }
        public bool IsCanceled { get; set; }

        public BookedFlight()
        {
        }

        public BookedFlight(User user, ScheduledFlight flight, int ticketQuantity)
        {
            User = user;
            UserEmail = user.Email;
            DepartureDate = flight.Departure.Scheduled;
            FlightStatus = flight.Flight_status;
            DepartureAirport = flight.Departure.Airport;
            ArrivalAirport = flight.Arrival.Airport;
            FlightIcao = flight.Flight.Icao;
            ArrivalDate = flight.Arrival.Scheduled;
            TicketQuantity = ticketQuantity;
            IsCanceled = false;
        }
    }
}