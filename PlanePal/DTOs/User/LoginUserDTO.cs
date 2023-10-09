namespace PlanePal.DTOs.User
{
    /// <summary>
    /// Represents a data transfer object for user login information.
    /// </summary>
    public class LoginUserDTO
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string Password { get; set; }
    }
}