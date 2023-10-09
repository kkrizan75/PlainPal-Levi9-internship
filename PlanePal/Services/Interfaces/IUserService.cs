using PlanePal.DTOs.User;
using PlanePal.Model.Shared;
using PlanePal.Model.UserModel;

namespace PlanePal.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<string>> Create(CreateUserDTO userDTO);

        Task<ServiceResponse<string>> Update(UpdateUserDTO userDTO, string email);

        Task<ServiceResponse<string>> Delete(DeleteUserDto deleteUserDto, string email);

        Task<ServiceResponse<string>> UpdatePassword(UpdatePasswordDTO passwordDTO, string email);

        Task<ServiceResponse<IEnumerable<ReadUserDTO>>> GetAll();

        string CheckDocument(User user, DateTime departureTime);

        Task<ServiceResponse<string>> UpdateDocument(string email, DocumentDTO documentDto);

        Task<ServiceResponse<string>> UploadDocumentImage(string email, IFormFile formFile);

        Task<ServiceResponse<ReadUserDetailsDTO>> LoadUserData(string userEmail);
    }
}