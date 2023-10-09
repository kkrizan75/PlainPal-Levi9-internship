using PlanePal.Model.FlightModel;

namespace PlanePal.Repositories.Interfaces
{
    public interface IBookedFlightRepository : IBaseRepository<BookedFlight, int>
    {
        Task<IEnumerable<BookedFlight>> GetBookedFlightsByUserEmailIcaoAndDepartureDate
            (string email, string icao, DateTime departureDate);

        Task<IEnumerable<BookedFlight>> GetFutureBookedFlightsByUserEmail(string email);
    }
}