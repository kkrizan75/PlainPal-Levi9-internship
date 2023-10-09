using AutoMapper;
using Newtonsoft.Json;
using PlanePal.AviationStackAPI;
using PlanePal.DTOs.APIResponse;
using PlanePal.DTOs.ScheduledFlight;
using PlanePal.Model.FlightModel;
using PlanePal.Model.Shared;
using PlanePal.Model.Static;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Interfaces;
using Serilog;

namespace PlanePal.Services.Implementation
{
    public class FlightService : IFlightService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FlightService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task LoadData()
        {
            await LoadAirlines();
            await LoadAirports();
            await LoadFlights();
        }

        public async Task LoadAirports()
        {
            var apiResponse = await FlightProcessor.LoadInformation("airports");
            if (apiResponse.Success)
            {
                Log.Information("Airport data successfully fetched from the external system");
                var data = JsonConvert.DeserializeObject<APIResponseDTO<AirportDetailsDTO>>(apiResponse.Data);
                var airports = _mapper.Map<IEnumerable<Airport>>(data.Data);
                foreach (Airport a in airports)
                {
                    await _unitOfWork.AirportRepository.Create(a);
                }

                Log.Information("Airport data successfully saved to the database");
            }
            else
            {
                Log.Error("An error has occured while fetching airport data from te external system: {@ErrorMessage}", apiResponse.Message);
            }
        }

        private async Task LoadAirlines()
        {
            var apiResponse = await FlightProcessor.LoadInformation("airlines");
            if (apiResponse.Success)
            {
                Log.Information("Airline data successfully fetched from the external system");
                var data = JsonConvert.DeserializeObject<APIResponseDTO<AirlineDetailsDTO>>(apiResponse.Data);
                var airlines = _mapper.Map<IEnumerable<Airline>>(data.Data);
                foreach (Airline a in airlines)
                {
                    await _unitOfWork.AirlineRepository.Create(a);
                }
                Log.Information("Airline data successfully saved to the database");
            }
            else
            {
                Log.Error("An error has occured while fetching airline data from te external system: {@ErrorMessage}", apiResponse.Message);
            }
        }

        public async Task LoadFlights()
        {
            var apiResponse = await FlightProcessor.LoadInformation("flights");
            if (apiResponse.Success)
            {
                Log.Information("Flight data successfully fetched from the external system");
                var data = JsonConvert.DeserializeObject<APIResponseDTO<ScheduledFlightDTO>>(apiResponse.Data);
                foreach (ScheduledFlightDTO scheduledFlightDTO in data.Data)
                {
                    await _unitOfWork.ScheduledFlightRepository.Create(_mapper.Map<ScheduledFlight>(scheduledFlightDTO));
                }
                Log.Information("Flight data successfully saved to the database");
            }
            else
            {
                Log.Error("An error has occured while fetching flight data from te external system: {@ErrorMessage}", apiResponse.Message);
            }
        }

        public async Task<ServiceResponse<IEnumerable<AirlineDetailsDTO>>> GetAllAirlines()
        {
            return new ServiceResponse<IEnumerable<AirlineDetailsDTO>>(_mapper.Map<IEnumerable<AirlineDetailsDTO>>(await _unitOfWork.AirlineRepository.GetAll()), true);
        }

        public async Task<IEnumerable<ScheduledFlight>> GetAll()
        {
            return _mapper.Map<IEnumerable<ScheduledFlight>>(await _unitOfWork.ScheduledFlightRepository.GetAllFlights());
        }

        public async Task<ServiceResponse<FlightDetailsDTO>> GetFlightById(int id)
        {
            var flightDetails = await _unitOfWork.ScheduledFlightRepository.GetFlightDetailsByFlightId(id);
            if (flightDetails == null)
            {
                Log.Error("Flight with an id {@id} not found", id);
                return new ServiceResponse<FlightDetailsDTO>(null, false, $"Flight with an id {id} not found");
            }
            return new ServiceResponse<FlightDetailsDTO>(_mapper.Map<FlightDetailsDTO>(flightDetails), true, "Flight found successfully");
        }

        public async Task<ServiceResponse<IEnumerable<AirportDetailsDTO>>> GetAllAirports()
        {
            return new ServiceResponse<IEnumerable<AirportDetailsDTO>>(_mapper.Map<IEnumerable<AirportDetailsDTO>>(await _unitOfWork.AirportRepository.GetAll()), true);
        }
    }
}