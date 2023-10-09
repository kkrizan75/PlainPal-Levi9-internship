using PlanePal.Model.FlightModel;

namespace PlanePal.Repositories.Interfaces
{
    public interface IScheduledFlightRepository : IBaseRepository<ScheduledFlight, int>
    {
        Task<ScheduledFlight> GetFlightDetailsByFlightId(int id);

        Task<IEnumerable<ScheduledFlight>> GetAllFlights();
    }
}