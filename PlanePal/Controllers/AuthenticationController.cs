using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanePal.DTOs.User;
using PlanePal.Services.Interfaces;
using Serilog;

namespace PlanePal.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token upon successful login.
        /// </summary>
        /// <param name="login">The user's login credentials.</param>
        /// <returns>
        ///   <list type="table">
        ///     <item>
        ///       <term>200 OK</term>
        ///       <description>Returns a JWT token upon successful authentication.</description>
        ///     </item>
        ///     <item>
        ///       <term>400 Bad Request</term>
        ///       <description>Returns a "Username or password incorrect." message if the authentication fails.</description>
        ///     </item>
        ///   </list>
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO login)
        {
            var result = await _authenticationService.Authenticate(login);
            if (result is null)
            {
                Log.Warning("Unsuccessful login attempt");
                return BadRequest("Username or password incorrect.");
            }

            var token = _authenticationService.GenerateToken(result);
            Log.Information("User has successfully logged in.");
            return Ok(token);
        }
    }
}