using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanePal.DTOs.BookedFlight;
using PlanePal.Services.Interfaces;
using Serilog;
using System.Security.Claims;

namespace PlanePal.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Books a flight for an authenticated client user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows an authenticated client user to book a flight by providing the necessary flight details in the request body.
        /// </remarks>
        /// <param name="dto">The data transfer object containing flight booking information.</param>
        /// <returns>A response indicating the result of the flight booking operation.</returns>
        /// <response code="200">If the flight is successfully booked.</response>
        /// <response code="400">If there is an error or validation issue, along with an error message..</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not authorized.</response>
        [Authorize(Roles = "CLIENT")]
        [HttpPost("book-flight")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> BookFlight([FromBody] BookFlightDTO dto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return BadRequest("Cannot book flight: User doesn't exist");
            }
            var result = await _bookingService.BookFlight(dto, email);
            if (!result.Success)
            {
                Log.Error("An error has occured while booking the flight, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            Log.Information("Flight successfully booked");
            return Ok(result.Message);
        }

        /// <summary>
        /// Retrieves a list of booked flights for the authenticated user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows an authenticated user to retrieve a list of flights that they have booked.
        /// </remarks>
        /// <returns>A response containing a list of booked flight information if successful; otherwise, an error message.</returns>
        /// <response code="200">Returns a list of booked flight information.</response>
        /// <response code="400">If there is an error or no booked flights are found.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [Authorize(Roles = "CLIENT")]
        [HttpGet("booked-flights")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookedFlightInfoDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetBookedFlights()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return BadRequest("Cannot show flights: User doesn't exist");
            }
            var result = await _bookingService.GetBookedFlights(email);

            if (!result.Success)
            {
                Log.Error("An error has occured while fetching the booked flights for the user, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Deletes a booked flight for the authenticated user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows an authenticated user to delete a booked flight by providing its unique identifier.
        /// </remarks>
        /// <param name="id">The unique identifier of the booked flight to be deleted.</param>
        /// <returns>A response indicating the result of the flight booking deletion operation.</returns>
        /// <response code="200">If the booking is successfully deleted.</response>
        /// <response code="400">If there is an error during deletion.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not authorized.</response>
        [Authorize(Roles = "CLIENT")]
        [HttpDelete("book-flight/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteBooking(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return BadRequest("Cannot delete booking: User doesn't exist");
            }
            var result = await _bookingService.DeleteBooking(id, email);

            if (!result.Success)
            {
                Log.Error("An error has occured while deleting the booked flight, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            Log.Information("Flight successfully deleted");
            return Ok(result.Message);
        }

        /// <summary>
        /// Updates the details of a booked flight for the authenticated user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows an authenticated user to update the details of a previously booked flight by providing the updated information in the request body.
        /// </remarks>
        /// <param name="updateBookedFlightDTO">The data transfer object containing updated flight booking information.</param>
        /// <returns>A response indicating the result of the flight booking update operation.</returns>
        /// <response code="200">If the booking details are successfully updated.</response>
        /// <response code="400">If there is an error during the update.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not authorized.</response>
        [Authorize(Roles = "CLIENT")]
        [HttpPut("update-booked-flight")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateBookedFlight([FromBody] UpdateBookedFlightDTO updateBookedFlightDTO)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return BadRequest("Cannot update booked flight: User doesn't exist");
            }
            var result = await _bookingService.UpdateBookedFlight(updateBookedFlightDTO, email);
            if (!result.Success)
            {
                Log.Error("An error has occured while updating the booked flight, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            Log.Information("Flight successfully updated");
            return Ok(result.Message);
        }
    }
}