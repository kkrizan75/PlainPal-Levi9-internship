using AutoMapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using PlanePal.Model.FlightModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using Xunit;

namespace PlanePal.BookingUnitTests
{
    public class DeleteBookingTests
    {
        private readonly IBookingService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IEmailService _emailService = Substitute.For<IEmailService>();
        private readonly IUserService _userService = Substitute.For<IUserService>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        public DeleteBookingTests()
        {
            _sut = new BookingService(_unitOfWork, _userService, _mapper, _emailService);
        }

        [Fact]
        public async Task DeleteBooking_NotExistingFlight_ReturnsError()
        {
            var flightId = 1;
            var email = "test@gmail.com";
            _unitOfWork.BookedFlightRepository.GetOne(Arg.Any<int>()).ReturnsNull();
            var result = await _sut.DeleteBooking(flightId, email);
            Assert.Equal($"Flight with an id {flightId}  does not exists.", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeleteBooking_NotAuthorizedUser_ReturnsError()
        {
            var flight = new BookedFlight();
            flight.Id = 1;
            flight.UserEmail = "test@gmail.com";
            _unitOfWork.BookedFlightRepository.GetOne(Arg.Any<int>()).Returns(flight);
            var result = await _sut.DeleteBooking(flight.Id, "tes@email.com");
            Assert.Equal($"User is not authorized.", result.Message);
            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task DeleteBooking_FlightInNextTwoHours_ReturnsError(int departureHours)
        {
            var flight = new BookedFlight();
            flight.UserEmail = "test@gmail.com";
            flight.DepartureDate = DateTime.Now.AddHours(departureHours);
            _unitOfWork.BookedFlightRepository.GetOne(Arg.Any<int>()).Returns(flight);
            var result = await _sut.DeleteBooking(1, flight.UserEmail);
            Assert.Equal("Booking can't be canceled, because it is in 2 hours.", result.Message);
            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        public async Task DeleteBooking_HappyPath_ReturnsSuccess(int departureHour)
        {
            // Arange
            var flight = new BookedFlight();
            flight.DepartureDate = DateTime.Now.AddHours(departureHour);
            flight.UserEmail = "test@gmail.com";
            _unitOfWork.BookedFlightRepository.GetOne(Arg.Any<int>()).Returns(flight);
            _unitOfWork.BookedFlightRepository.UpdateAndSave(flight).Returns(1);

            // Act
            var result = await _sut.DeleteBooking(1, flight.UserEmail);

            // Assert
            Assert.True(flight.IsCanceled);
            Assert.Equal("Booking successfully canceled.", result.Message);
            Assert.True(result.Success);
        }
    }
}