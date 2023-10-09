namespace PlanePal.DTOs.User
{
    public class UpdateUserDTO
    {
        /// <summary>
        /// User's first name.
        /// </summary>
        /// <example>John</example>
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name.
        /// </summary>
        /// <example>Doe</example>
        public string LastName { get; set; }

        /// <summary>
        /// User's phone number.
        /// </summary>
        /// <example>+1234567890</example>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// User's date of birth.
        /// </summary>
        /// <example>1990-01-15</example>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// User's street.
        /// </summary>
        /// <example>Strazilovska</example>
        public string Street { get; set; }

        /// <summary>
        /// User's street number.
        /// </summary>
        /// <example>22</example>
        public string StreetNumber { get; set; }

        /// <summary>
        /// User's city.
        /// </summary>
        /// <example>Novi Sad</example>
        public string City { get; set; }

        /// <summary>
        /// User's country.
        /// </summary>
        /// <example>Serbia</example>
        public string Country { get; set; }
    }
}