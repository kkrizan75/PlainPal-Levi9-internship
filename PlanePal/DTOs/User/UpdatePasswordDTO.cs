namespace PlanePal.DTOs.User
{
    public class UpdatePasswordDTO
    {
        /// <summary>
        /// User's old password.
        /// </summary>
        /// <example>My0ldP4ssw0rD!!1!</example>
        public string OldPassword { get; set; }

        /// <summary>
        ///     User's nw password.
        /// </summary>
        /// <example>MyNewP4ssw0rD!!1!</example>
        public string NewPassword { get; set; }
    }
}