using AutoMapper;
using Azure;
using PlanePal.DTOs.User;
using PlanePal.Model.Shared;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Interfaces;
using PlanePal.Services.Shared;

namespace PlanePal.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAzureStorage _azureStorage;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IAzureStorage azureStorage)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _azureStorage = azureStorage;
        }

        private User PrepeareUserForDB(User newUser)
        {
            newUser.RoleId = 2;
            newUser.Salt = HashnSaltService.GenerateSalt();
            newUser.Password = HashnSaltService.HashWithSalt(newUser.Password, newUser.Salt);
            return newUser;
        }

        public async Task<ServiceResponse<string>> Create(CreateUserDTO userDTO)
        {
            var user = _unitOfWork.UserRepository.GetOne(userDTO.Email);
            if (user != null)
            {
                return new ServiceResponse<string>(null, false, $"User with an email {userDTO.Email} already exists");
            }
            try
            {
                var newUser = _mapper.Map<User>(userDTO);
                newUser = PrepeareUserForDB(newUser);
                if(await _unitOfWork.UserRepository.Create(newUser) != 0)
                {
                    return new ServiceResponse<string>(null, true, $"User with an email {userDTO.Email} successfully created.");
                }
                return new ServiceResponse<string>(null, false, $"User with an email {userDTO.Email} not created.");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResponse<string>> Delete(DeleteUserDto deleteUserDto, string email)
        {
            var user = _unitOfWork.UserRepository.GetOne(email);
            if (user == null)
            {
                return new ServiceResponse<string>(null, false, $"User with email {email}  does not exists.");
            }
            if (user.UserStatus == Enums.UserStatus.BLOCKED || user.UserStatus == Enums.UserStatus.DELETED)
            {
                return new ServiceResponse<string>(null, false, $"Can only delete users who are active or pending.");
            }

            if (user.Password.CompareTo(HashnSaltService.HashWithSalt(deleteUserDto.Password, user.Salt)) != 0)
            {
                return new ServiceResponse<string>(null, false, "Passwords does not match.");
            }
            var bookings = await _unitOfWork.BookedFlightRepository.GetFutureBookedFlightsByUserEmail(email);
            if (bookings.Any())
            {
                return new ServiceResponse<string>(null, false, $"User can't delete profile if he has active future bookings.");
            }
            user.UserStatus = Enums.UserStatus.DELETED;
            if (await _unitOfWork.UserRepository.SaveChanges()!= 0)
            {
                return new ServiceResponse<string>(null, true, $"User successfully deleted.");
            }
            return new ServiceResponse<string>(null, false, $"User not deleted.");
        }

        public async Task<ServiceResponse<IEnumerable<ReadUserDTO>>> GetAll()
        {
            var allUsers = await _unitOfWork.UserRepository.GetAllUsers();
            if(allUsers == null)
            {
                return new ServiceResponse<IEnumerable<ReadUserDTO>>(false, "Users cannot be fetched!");
            }
            var allUsersDTO = _mapper.Map<IEnumerable<ReadUserDTO>>(allUsers);
            if(allUsersDTO == null)
            {
                return new ServiceResponse<IEnumerable<ReadUserDTO>>(false, "Users cannot be mapped!");
            }
            return new ServiceResponse<IEnumerable<ReadUserDTO>>(allUsersDTO, true, "Users successfully fetched.");
        }

        public async Task<ServiceResponse<string>> Update(UpdateUserDTO userDTO, string email)
        {
            var user = await _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(email);
            if (user is null)
            {
                return new ServiceResponse<string>(null, false, $"User with an email {email} not found");
            }
            else
            {
                user = user.UpdateUser(userDTO);
                if (await _unitOfWork.UserRepository.UpdateAndSave(user) != 0)
                {
                    return new ServiceResponse<string>(null, true, "User successfully updated");
                }
                return new ServiceResponse<string>(null, false, "User not updated");
            }
        }

        public async Task<ServiceResponse<string>> UpdatePassword(UpdatePasswordDTO passwordDTO, string email)
        {
            var user = _unitOfWork.UserRepository.GetOne(email);
            if (user is null)
            {
                return new ServiceResponse<string>(null, false, $"User with an id {email} not found");
            }
            if (user.Password.CompareTo(HashnSaltService.HashWithSalt(passwordDTO.OldPassword, user.Salt)) != 0)
            {
                return new ServiceResponse<string>(null, false, "Incorrect old password");
            }
            else
            {
                user.Password = HashnSaltService.HashWithSalt(passwordDTO.NewPassword, user.Salt);
                if (await _unitOfWork.UserRepository.SaveChanges() != 0)
                {
                    return new ServiceResponse<string>(null, true, "Password successfully changed");
                }
                return new ServiceResponse<string>(null, false, "Password not changed");
            }
        }

        public string CheckDocument(User user, DateTime departureTime)
        {
            var threeMonthsAfter = departureTime.AddMonths(3);
            if (user.DocumentId == 0) return "Cannot book flights without an attached document!";
            if (DateTime.Compare(user.IdentificationDocument.ExpirationDate, threeMonthsAfter) <= 0) return "Cannot book flights if the attached document expires in less than 3 months from departure time!";
            return "";
        }

        public async Task<ServiceResponse<string>> UpdateDocument(string email, DocumentDTO documentDto)
        {
            var user = _unitOfWork.UserRepository.GetOne(email);
            if (user is null)
            {
                return new ServiceResponse<string>(null, false, $"User with an email {email} not found");
            }
            try
            {
                var document = _mapper.Map<IdentificationDocument>(documentDto);
                if (await _unitOfWork.DocumentRepository.Create(document) != 0)
                {
                    user.IdentificationDocument = document;
                    user.DocumentId = document.Id;
                    if (await _unitOfWork.UserRepository.UpdateAndSave(user) != 0)
                        return new ServiceResponse<string>(document.DocumentNumber, true, "Successfully updated document.");
                }
                return new ServiceResponse<string>(null, false, "Unable to update user document.");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResponse<string>> UploadDocumentImage(string email, IFormFile formFile)
        {
            var user = await _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(email);
            if (user is null)
            {
                return new ServiceResponse<string>(null, false, $"User with an email {email} not found");
            }
            if (user.IdentificationDocument.DocumentImageUri is not null)
            {
                try
                {
                    await _azureStorage.DeleteAsync(user.IdentificationDocument.DocumentImageUri);
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<string>(null, false, ex.Message);
                }
            }

            try
            {
                var response = await _azureStorage.UploadAsync(formFile);
                user.IdentificationDocument.DocumentImageUri = response.Blob.Name;
                if (response.Error)
                {
                    return new ServiceResponse<string>(null, false, $"Error occured while trying to upload image.");
                }
                await _unitOfWork.DocumentRepository.SaveChanges();
                return new ServiceResponse<string>(null, true, $"Successfully uploaded image.");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResponse<ReadUserDetailsDTO>> LoadUserData(string userEmail)
        {
            var user = await _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(userEmail);
            if (user is null)
            {
                return new ServiceResponse<ReadUserDetailsDTO>(null, false, $"User with an email {userEmail} not found");
            }

            try
            {
                // returning only image name and content type for blob dto
                var blobDTO = await _azureStorage.DownloadAsync(user.IdentificationDocument.DocumentImageUri);
                var userDTO = _mapper.Map<ReadUserDetailsDTO>(user);
                userDTO.IdentificationDocument.BlobDto = blobDTO;
                return new ServiceResponse<ReadUserDetailsDTO>(userDTO, true, $"User data successfully loaded");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReadUserDetailsDTO>(null, false, ex.Message);
            }
        }
    }
}