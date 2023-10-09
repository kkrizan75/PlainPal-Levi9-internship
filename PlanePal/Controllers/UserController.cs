using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanePal.DTOs.User;
using PlanePal.Services.Interfaces;
using Serilog;
using System.Security.Claims;

namespace PlanePal.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a list of all users by processing a GET request.
        /// </summary>
        /// <returns>
        /// An HTTP response containing a list of user data in the form of <see cref="ReadUserDTO"/> objects.
        /// </returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="404">If no users found.</response>
        /// <response code="401">If the user is not authorized to access this endpoint.</response>
        /// <response code="403">If the user is not admin.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAll();
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Creates a new user by processing a POST request with user data provided in the request body.
        /// </summary>
        /// <param name="userDTO">The data transfer object containing user information.</param>
        /// <returns>
        /// An HTTP response indicating the result of the user creation process, including success status and message.
        /// </returns>
        /// <response code="200">Returns the creation user success message.</response>
        /// <response code="400">If creation was unsuccessful.</response>
        [AllowAnonymous]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO userDTO)
        {
            var result = await _userService.Create(userDTO);
            if (!result.Success)
            {
                Log.Error("An error has occured while creating the user, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            Log.Information("User successfully created");
            return Ok(result.Message);
        }

        /// <summary>
        /// Updates a user's information by processing a PUT request with user data provided in the request body.
        /// </summary>
        /// <param name="userDTO">The data transfer object containing updated user information.</param>
        /// <returns>
        /// An HTTP response indicating the result of the user update process, including success status and message.
        /// </returns>
        /// <response code="200">Returns update user success message.</response>
        /// <response code="400">If updating was unsuccessful.</response>
        /// <response code="401">If the user is not authorized to access this endpoint.</response>
        [Authorize]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO userDTO)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                Log.Error("An error has occured while updating the user.");
                return BadRequest("Cannot update user: User doesn't exist");
            }
            var result = await _userService.Update(userDTO, email);
            if (!result.Success)
            {
                Log.Error("An error has occured while updating the user, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            Log.Information("User successfully updated");
            return Ok(result.Message);
        }

        /// <summary>
        /// Updates a user's document by processing a PUT request with document data provided in the request body.
        /// </summary>
        /// <param name="documentDto">The data transfer object containing updated document information.</param>
        /// <returns>
        /// An HTTP response indicating the result of the document update process, including success status and message.
        /// </returns>
        /// <response code="200">Returns update user document success message.</response>
        /// <response code="400">If updating document was unsuccessful.</response>
        /// <response code="401">If the user is not authorized to access this endpoint.</response>
        /// <response code="403">If the user is not client.</response>
        [Authorize(Roles = "CLIENT")]
        [HttpPut("update-document")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateDocument([FromBody] DocumentDTO documentDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return BadRequest("Cannot update identification document: User doesn't exist");
            }
            var result = await _userService.UpdateDocument(email, documentDto);
            if (!result.Success)
            {
                Log.Error("An error has occured while updating the document, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            Log.Information("Document successfully updated");
            return Ok(result.Message);
        }

        /// <summary>
        /// Changes a user's password by processing a PUT request with updated password data provided in the request body.
        /// </summary>
        /// <param name="updatePasswordDTO">The data transfer object containing updated password information.</param>
        /// <returns>
        /// An HTTP response indicating the result of the password update process, including success status and message.
        /// </returns>
        /// <response code="200">Returns update user password success message.</response>
        /// <response code="400">If updating password was unsuccessful.</response>
        /// <response code="401">If the user is not authorized to access this endpoint.</response>
        [Authorize]
        [HttpPut("update-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordDTO updatePasswordDTO)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return BadRequest("Cannot update password: User doesn't exist");
            }
            var result = await _userService.UpdatePassword(updatePasswordDTO, email);
            if (!result.Success)
            {
                Log.Error("An error has occured while updating the user password, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            Log.Information("Password successfully updated");
            return Ok(result.Message);
        }

        /// <summary>
        /// Deletes a user by processing a DELETE request with data provided in the request body.
        /// </summary>
        /// <param name="deleteUserDto">The data transfer object containing user deletion information.</param>
        /// <returns>
        /// An HTTP response indicating the result of the user deletion process, including success status and message.
        /// </returns>
        /// <response code="200">Returns delete user success message.</response>
        /// <response code="400">If deleting user was unsuccessful.</response>
        /// <response code="401">If the user is not authorized to access this endpoint.</response>
        [Authorize]
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete([FromBody] DeleteUserDto deleteUserDto)
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            if (email is null)
            {
                return BadRequest("Cannot delete user: User doesn't exist");
            }
            var result = await _userService.Delete(deleteUserDto, email);
            if (!result.Success)
            {
                Log.Error("An error has occured while deleting the user, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            Log.Information("User successfully deleted");
            return Ok(result.Message);
        }

        /// <summary>
        /// Uploads a identification document image by processing a POST request with the image data provided as a form file.
        /// </summary>
        /// <param name="formFile">The form file containing the identification document image data.</param>
        /// <returns>
        /// An HTTP response indicating the result of the identification document image upload process, including success status and message.
        /// </returns>
        /// <response code="200">Returns upload user document image success message.</response>
        /// <response code="400">If uploading document was unsuccessful.</response>
        /// <response code="401">If the user is not authorized to access this endpoint.</response>
        /// <response code="403">If the user is not client.</response>
        [Authorize(Roles = "CLIENT")]
        [HttpPost("upload-passport-photo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadDocumentImage(IFormFile formFile)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return BadRequest("Cannot upload document image: User doesn't exist");
            }
            var result = await _userService.UploadDocumentImage(email, formFile);

            if (!result.Success)
            {
                Log.Error("An error has occured while uploading the passport photo, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            Log.Information("Passport photo successfully uploaded");
            return Ok(result.Message);
        }

        /// <summary>
        /// Retrieves user data for the currently authenticated user by processing a GET request.
        /// </summary>
        /// <returns>
        /// An HTTP response containing the user data in the form of a <see cref="ReadUserDetailsDTO"/> object.
        /// </returns>
        /// <response code="200">Returns authenticated user data.</response>
        /// <response code="400">If returning data was unsuccessful.</response>
        /// <response code="401">If the user is not authorized to access this endpoint.</response>
        /// <response code="403">If the user is not client.</response>
        [Authorize(Roles = "CLIENT")]
        [HttpGet("load-user-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> LoadUserData()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return BadRequest("Cannot load user data: User doesn't exist");
            }
            var result = await _userService.LoadUserData(email);

            if (!result.Success)
            {
                Log.Error("An error has occured while fetching the logged-in user data, error message: {@ErrorMessage}", result.Message);
                return BadRequest(result.Message);
            }

            Log.Information("Logged-in user data successfully fetched");
            return Ok(result.Data);
        }
    }
}