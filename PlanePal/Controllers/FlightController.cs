using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanePal.Services.Interfaces;
using Serilog;

namespace PlanePal.Controllers
{
    [Route("api/flights")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        /// <summary>
        /// Retrieves a list of all scheduled flights.
        /// </summary>
        /// <returns>
        /// If successful, returns a collection of scheduled flight information.
        /// </returns>
        /// <response code="200">Returns the list of scheduled flights.</response>
        /// <response code="404">If no scheduled flights are found.</response>

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            await _flightService.LoadFlights();
            var response = await _flightService.GetAll();
            if (!response.Any())
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Retrieves flight information by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the flight to retrieve.</param>
        /// <returns>
        /// If successful, returns the flight information associated with the provided ID.
        /// </returns>
        /// <response code="200">Returns the flight information.</response>
        /// <response code="404">If no flight with the specified ID is found.</response>
        [AllowAnonymous]
        [HttpGet("flight-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFlightById(int id)
        {
            var response = await _flightService.GetFlightById(id);
            if (!response.Success)
            {
                Log.Error("An error has occured while fetching flight data, error message: {@ErrorMessage}", response.Message);
                return NotFound(response.Message);
            }

            return Ok(response.Data);
        }

        /// <summary>
        /// Retrieves a list of all airlines.
        /// </summary>
        /// <returns>
        /// If successful, returns a collection of airline information.
        /// </returns>
        /// <response code="200">Returns the list of airlines.</response>
        /// <response code="404">If no airlines are found.</response>
        [AllowAnonymous]
        [HttpGet("airlines")]
        public async Task<IActionResult> GetAllAirlines()
        {
            var response = await _flightService.GetAllAirlines();
            if (!response.Data.Any())
                return NotFound(response.Message);

            return Ok(response.Data);
        }

        /// <summary>
        /// Retrieves a list of all airports.
        /// </summary>
        /// <returns>
        /// If successful, returns a collection of airport information.
        /// </returns>
        /// <response code="200">Returns the list of airports.</response>
        /// <response code="404">If no airports are found.</response>
        [AllowAnonymous]
        [HttpGet("airports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAirports()
        {
            var response = await _flightService.GetAllAirports();
            if (!response.Data.Any())
                return NotFound(response.Message);

            return Ok(response.Data);
        }
    }
}