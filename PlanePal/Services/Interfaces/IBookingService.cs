using PlanePal.DTOs.BookedFlight;
using PlanePal.Model.Shared;

namespace PlanePal.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ServiceResponse<string>> BookFlight(BookFlightDTO dto, string email);

        Task<ServiceResponse<IEnumerable<BookedFlightInfoDTO>>> GetBookedFlights(string email);

        Task<ServiceResponse<string>> UpdateBookedFlight(UpdateBookedFlightDTO updateBookedFlightDTO, string email);

        Task<ServiceResponse<string>> DeleteBooking(int id, string email);
    }
}