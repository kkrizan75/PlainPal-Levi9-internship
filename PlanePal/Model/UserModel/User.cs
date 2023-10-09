using PlanePal.DTOs.User;
using PlanePal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanePal.Model.UserModel
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }

        [Key]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Salt { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public UserStatus UserStatus { get; set; }

        [Required]
        [ForeignKey("Address")]
        public int AddressId { get; set; }

        [Required]
        public Address Address { get; set; }

        [ForeignKey("IdentificationDocument")]
        public int DocumentId { get; set; }

        public IdentificationDocument IdentificationDocument { get; set; }

        [ForeignKey("UserRole")]
        public int RoleId { get; set; }

        public UserRole UserRole { get; set; }

        public User()
        {
        }

        public User UpdateUser(UpdateUserDTO dto)
        {
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            PhoneNumber = dto.PhoneNumber;
            DateOfBirth = dto.DateOfBirth;
            Address = new Address()
            {
                City = dto.City,
                Street = dto.Street,
                StreetNumber = dto.StreetNumber,
                Country = dto.Country
            };
            return this;
        }
    }
}