namespace PlanePal.DTOs.BookedFlight
{
    public class UpdateBookedFlightDTO
    {
        /// <summary>
        /// The unique identifier of the booked flight to be updated.
        /// </summary>
        /// <example>123</example>
        public int Id { get; set; }

        /// <summary>
        /// The new quantity of tickets for the booked flight.
        /// </summary>
        /// <example>2</example>
        public int TicketQuantity { get; set; }
    }
}