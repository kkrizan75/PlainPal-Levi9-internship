using System.ComponentModel.DataAnnotations;

namespace PlanePal.DTOs.User
{
    public class DeleteUserDto
    {
        /// <summary>
        ///     User's password.
        /// </summary>
        /// <example>MyAwes0m3P4ssw0rD!!1!</example>
        [Required]
        public string Password { get; set; }
    }
}