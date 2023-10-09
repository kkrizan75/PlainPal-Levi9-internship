using PlanePal.Enums;

namespace PlanePal.DTOs.ScheduledFlight
{
    /// <summary>
    /// Represents detailed information about a flight, including its status, departure, arrival, airline, and flight details.
    /// </summary>
    public class FlightDetailsDTO
    {
        /// <summary>
        /// Gets or sets the status of the flight.
        /// </summary>
        /// <example>
        /// Example FlightStatus object
        /// </example>
        public FlightStatus Flight_Status { get; set; }

        /// <summary>
        /// Gets or sets the departure details of the flight.
        /// </summary>
        /// <example>
        /// Example DepartureDTO object
        /// </example>
        public DepartureDTO Departure { get; set; }

        /// <summary>
        /// Gets or sets the arrival details of the flight.
        /// </summary>
        /// <example>
        /// Example ArrivalDTO object
        /// </example>
        public ArrivalDTO Arrival { get; set; }

        /// <summary>
        /// Gets or sets the airline details of the flight.
        /// </summary>
        /// <example>
        /// Example AirLineDTO object
        /// </example>
        public AirLineDTO Airline { get; set; }

        /// <summary>
        /// Gets or sets the flight details.
        /// </summary>
        /// <example>
        /// Example FlightDTO object
        /// </example>
        public FlightDTO Flight { get; set; }
    }
}