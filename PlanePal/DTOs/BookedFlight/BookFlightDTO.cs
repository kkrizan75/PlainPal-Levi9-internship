using System.ComponentModel.DataAnnotations;

namespace PlanePal.DTOs.BookedFlight
{
    public class BookFlightDTO
    {
        /// <summary>
        /// The ID of the scheduled flight you want to book.
        /// </summary>
        [Required]
        public int ScheduledFlightId { get; set; }

        /// <summary>
        /// The number of tickets you want to book. The default value is 1.
        /// </summary>
        /// <example>1</example>
        public int TicketQuantity { get; set; } = 1;
    }
}