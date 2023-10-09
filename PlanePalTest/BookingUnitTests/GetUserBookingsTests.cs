using AutoMapper;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using PlanePal.DTOs.BookedFlight;
using PlanePal.Model.FlightModel;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using System.Linq.Expressions;
using Xunit;

namespace PlanePal.BookingUnitTests
{
    public class GetUserBookingsTests
    {
        private readonly IBookingService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IEmailService _emailService = Substitute.For<IEmailService>();
        private readonly IUserService _userService = Substitute.For<IUserService>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        public GetUserBookingsTests()
        {
            _sut = new BookingService(_unitOfWork, _userService, _mapper, _emailService);
        }

        [Fact]
        public async Task GetBookedFlights_NotExistingEmail_ReturnsError()
        {
            var email = "test@gmail.com";
            _unitOfWork.UserRepository.GetOne(Arg.Any<string>()).ReturnsNull();

            var result = await _sut.GetBookedFlights(email);
            Assert.Equal($"User with email {email} not found", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task GetBookedFlights_NotExistingBookings_ReturnsSuccess()
        {
            var user = new User { Email = "test@gmail.com" };

            _unitOfWork.UserRepository.GetOne("test@gmail.com").Returns(user);
            _unitOfWork.BookedFlightRepository.GetAllBy(Arg.Any<Expression<Func<BookedFlight, bool>>>())
                                              .Returns(Enumerable.Empty<BookedFlight>());

            var result = await _sut.GetBookedFlights("test@gmail.com");
            Assert.Equal("You don't have any booked flights yet!", result.Message);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task GetBookedFlights_ThrowsException_ReturnsFalse()
        {
            _unitOfWork.BookedFlightRepository.GetAllBy(Arg.Any<Expression<Func<BookedFlight, bool>>>())
                                                        .ThrowsAsync<Exception>();

            var result = await _sut.GetBookedFlights("test@gmail.com");
            Assert.False(result.Success);
        }

        [Fact]
        public async Task GetBookedFlights_HappyPath_ReturnsSuccess()
        {
            var user = new User { Email = "test@gmail.com" };
            _unitOfWork.UserRepository.GetOne("test@gmail.com").Returns(user);
            List<BookedFlight> bookedFlights = new()
            {
                new BookedFlight { UserEmail = "test@gmail.com", TicketQuantity = 1 }
            };
            _unitOfWork.BookedFlightRepository.GetAllBy(Arg.Any<Expression<Func<BookedFlight, bool>>>())
                                              .Returns(bookedFlights);
            var result = await _sut.GetBookedFlights("test@gmail.com");
            Assert.Equal(_mapper.Map<IEnumerable<BookedFlightInfoDTO>>(bookedFlights), result.Data);
            Assert.True(result.Success);
        }
    }
}