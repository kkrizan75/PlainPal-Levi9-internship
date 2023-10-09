namespace PlanePal.DTOs.User
{
    public class ReadUserDetailsDTO
    {
        /// <summary>
        /// User's e-mail address.
        /// </summary>
        /// <example>youremail@gmail.com</example>
        public string Email { get; set; }

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
        /// User's address information.
        /// </summary>
        /// <example>
        /// {
        ///     "Street": "Strazilovska",
        ///     "Street number": "22",
        ///     "City": "Novi Sad",
        ///     "Country": "Serbia"
        /// }
        /// </example>
        public AddressDTO Address { get; set; }

        /// <summary>
        /// User's identification document information.
        /// </summary>
        /// <example>
        /// {
        ///     "DocumentType": "Passport",
        ///     "DocumentNumber": "ABC123456",
        ///     "IssuingCountry": "SRB",
        ///     "ExpirationDate": "2030-12-31"
        /// }
        /// </example>
        public DocumentDetailsDTO IdentificationDocument { get; set; }
    }
}