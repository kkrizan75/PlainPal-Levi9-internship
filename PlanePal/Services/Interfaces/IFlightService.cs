using PlanePal.DTOs.ScheduledFlight;
using PlanePal.Model.FlightModel;
using PlanePal.Model.Shared;

namespace PlanePal.Services.Interfaces
{
    public interface IFlightService
    {
        Task<ServiceResponse<FlightDetailsDTO>> GetFlightById(int id);

        Task LoadFlights();

        Task<ServiceResponse<IEnumerable<AirlineDetailsDTO>>> GetAllAirlines();

        Task<ServiceResponse<IEnumerable<AirportDetailsDTO>>> GetAllAirports();

        Task LoadData();

        Task<IEnumerable<ScheduledFlight>> GetAll();
    }
}