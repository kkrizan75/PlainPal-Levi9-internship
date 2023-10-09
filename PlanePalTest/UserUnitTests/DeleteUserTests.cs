using AutoMapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using PlanePal.DTOs.User;
using PlanePal.Enums;
using PlanePal.Model.FlightModel;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using PlanePal.Services.Shared;
using Xunit;

namespace PlanePal.UserUnitTests
{
    public class DeleteUserTests
    {
        private readonly IUserService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IAzureStorage _azureStorage = Substitute.For<IAzureStorage>();

        public DeleteUserTests()
        {
            _sut = new UserService(_unitOfWork, _mapper, _azureStorage);
        }

        [Fact]
        public async Task DeleteUser_NotExistingEmail_ThrowError()
        {
            var deleteUserDto = new DeleteUserDto { Password = "testPassword" };
            var email = "test@gmail.com";
            _unitOfWork.UserRepository.GetOne(Arg.Any<string>()).ReturnsNull();

            var result = await _sut.Delete(deleteUserDto, email);
            Assert.Equal($"User with email {email}  does not exists.", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeleteUser_UserStatusBlocked_ThrowError()
        {
            var user = new User { Email = "test@gmail.com", UserStatus = UserStatus.BLOCKED };
            var deleteUserDto = new DeleteUserDto { Password = "testPassword" };
            _unitOfWork.UserRepository.GetOne("test@gmail.com").Returns(user);

            var result = await _sut.Delete(deleteUserDto, "test@gmail.com");
            Assert.Equal("Can only delete users who are active or pending.", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeleteUser_UserStatusDeleted_ThrowError()
        {
            var user = new User { Email = "test@gmail.com", UserStatus = UserStatus.DELETED };
            var deleteUserDto = new DeleteUserDto { Password = "testPassword" };
            _unitOfWork.UserRepository.GetOne("test@gmail.com").Returns(user);

            var result = await _sut.Delete(deleteUserDto, "test@gmail.com");
            Assert.Equal("Can only delete users who are active or pending.", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeleteUser_WrongPassword_ThrowError()
        {
            var user = new User { Email = "test@gmail.com", Salt = "salt", Password = HashnSaltService.HashWithSalt("password", "salt") };
            var deleteUserDto = new DeleteUserDto { Password = "wrongPassword" };
            _unitOfWork.UserRepository.GetOne("test@gmail.com").Returns(user);

            var result = await _sut.Delete(deleteUserDto, "test@gmail.com");
            Assert.Equal("Passwords does not match.", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeleteUser_UserHasBookings_ThrowError()
        {
            var user = new User { Email = "test@gmail.com", Password = HashnSaltService.HashWithSalt("testPassword", "salt"), Salt = "salt" };
            var booking = new BookedFlight();
            booking.UserEmail = user.Email;
            booking.DepartureDate = DateTime.Now.AddHours(-2);
            booking.IsCanceled = false;
            var flights = new List<BookedFlight>();
            flights.Add(booking);
            var deleteUserDto = new DeleteUserDto { Password = "testPassword" };
            _unitOfWork.UserRepository.GetOne("test@gmail.com").Returns(user);
            _unitOfWork.BookedFlightRepository.GetFutureBookedFlightsByUserEmail(user.Email).Returns(flights);

            var result = await _sut.Delete(deleteUserDto, "test@gmail.com");
            Assert.Equal("User can't delete profile if he has active future bookings.", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeleteUser_CorrectData_ReturnTrue()
        {
            var user = new User { Email = "test@gmail.com", Password = HashnSaltService.HashWithSalt("testPassword", "salt"), Salt = "salt" };
            var deleteUserDto = new DeleteUserDto { Password = "testPassword" };
            _unitOfWork.UserRepository.GetOne("test@gmail.com").Returns(user);
            _unitOfWork.UserRepository.SaveChanges().Returns(1);
            var result = await _sut.Delete(deleteUserDto, "test@gmail.com");
            Assert.Equal($"User successfully deleted.", result.Message);
            Assert.True(result.Success);
        }
        [Fact]
        public async Task DeleteUser_InvalidDelete_ReturnFalse()
        {
            var user = new User { Email = "test@gmail.com", Password = HashnSaltService.HashWithSalt("testPassword", "salt"), Salt = "salt" };
            var deleteUserDto = new DeleteUserDto { Password = "testPassword" };
            _unitOfWork.UserRepository.GetOne("test@gmail.com").Returns(user);
            _unitOfWork.UserRepository.SaveChanges().Returns(0);
            var result = await _sut.Delete(deleteUserDto, "test@gmail.com");
            Assert.Equal($"User not deleted.", result.Message);
            Assert.False(result.Success);
        }
    }
}