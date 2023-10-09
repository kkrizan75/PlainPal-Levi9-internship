using PlanePal.Enums;

namespace PlanePal.DTOs.BookedFlight
{
    public class BookedFlightInfoDTO
    {
        /// <summary>
        /// The unique identifier for the booked flight.
        /// </summary>
        /// <example>123</example>
        public int Id { get; set; }

        /// <summary>
        /// The date and time of the booked flight.
        /// </summary>
        /// <example>2023-10-04T09:00:00</example>
        public DateTime FlightDate { get; set; }

        /// <summary>
        /// The status of the booked flight.
        /// </summary>
        /// <example>Scheduled</example>
        public FlightStatus FlightStatus { get; set; }

        /// <summary>
        /// Full name for the departure airport.
        /// </summary>
        /// <example>Wellington International</example>
        public string DepartureAirport { get; set; }

        /// <summary>
        /// Full name for the arrival airport.
        /// </summary>
        /// <example>Sydney Kingsford Smith Airport</example>
        public string ArrivalAirport { get; set; }

        /// <summary>
        /// The ICAO code of the booked flight.
        /// </summary>
        /// <example>ABC123</example>
        public string FlightIcao { get; set; }

        /// <summary>
        /// The number of tickets booked for the flight.
        /// </summary>
        /// <example>2</example>
        public int TicketQuantity { get; set; }

        /// <summary>
        /// A value indicating whether the booking is canceled.
        /// </summary>
        /// <example>false</example>
        public bool IsCanceled { get; set; }
    }
}