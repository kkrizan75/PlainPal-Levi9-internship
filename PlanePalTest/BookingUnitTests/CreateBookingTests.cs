using AutoMapper;
using FluentValidation.TestHelper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using PlanePal.DTOs.BookedFlight;
using PlanePal.Model.FlightModel;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using PlanePal.Validators;
using Xunit;

namespace PlanePal.BookingUnitTests
{
    public class CreateBookingTests
    {
        private readonly IBookingService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IEmailService _emailService = Substitute.For<IEmailService>();
        private readonly IUserService _userService = Substitute.For<IUserService>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly BookFlightValidator _validator;
        private readonly BookFlightDTO _bookFlightDTO;

        public CreateBookingTests()
        {
            _sut = new BookingService(_unitOfWork, _userService, _mapper, _emailService);
            _validator = new();
            _bookFlightDTO = new();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void BookFlight_TicketQuantityInvalid_ReturnsError(int ticketQuantity)
        {
            // Arange
            _bookFlightDTO.TicketQuantity = ticketQuantity;

            // Act
            var result = _validator.TestValidate(_bookFlightDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(_bookFlightDTO => _bookFlightDTO.TicketQuantity)
                .WithErrorMessage("Ticket quantity should be greater than 0 but lesser than 5.");
        }

        [Fact]
        public async Task BookFlight_InvalidUserEmail_ReturnsError()
        {
            // Arange
            var user = new User();
            user.Email = "test@test.com";
            var testEmail = "test123@gmail.com";
            var scheduledFlight = new ScheduledFlight();
            scheduledFlight.Id = 1;
            scheduledFlight.Flight_date = DateTime.Now;
            _bookFlightDTO.ScheduledFlightId = scheduledFlight.Id;

            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(user.Email)
                .Returns(user);
            _unitOfWork.ScheduledFlightRepository
                .GetFlightDetailsByFlightId(_bookFlightDTO.ScheduledFlightId)
                .Returns(scheduledFlight);

            // Act
            var result = await _sut.BookFlight(_bookFlightDTO, testEmail);

            // Arrange
            Assert.Null(result.Data);
            Assert.Equal($"User with email {testEmail} not found or scheduled flight not found.", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task BookFlight_InvalidUserDocument_ReturnsError()
        {
            // Arange
            string documentCheck = "TEST";

            var user = new User();
            user.Email = "test@test.com";

            var scheduledFlight = new ScheduledFlight();
            scheduledFlight.Id = 1;
            scheduledFlight.Flight_date = DateTime.Now;
            _bookFlightDTO.ScheduledFlightId = scheduledFlight.Id;

            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(user.Email)
                .Returns(user);
            _unitOfWork.ScheduledFlightRepository
                .GetFlightDetailsByFlightId(_bookFlightDTO.ScheduledFlightId)
                .Returns(scheduledFlight);
            _userService.CheckDocument(user, scheduledFlight.Flight_date)
                .Returns(documentCheck);

            // Act
            var result = await _sut.BookFlight(_bookFlightDTO, "test@test.com");

            // Arrange
            Assert.Null(result.Data);
            Assert.Equal(documentCheck, result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task BookFlight_UserStatusIsNotActive_ReturnsError()
        {
            // Arange
            var user = new User();
            user.Email = "test@test.com";
            user.UserStatus = Enums.UserStatus.BLOCKED;

            var scheduledFlight = new ScheduledFlight();
            scheduledFlight.Id = 1;
            scheduledFlight.Flight_date = DateTime.Now;

            _bookFlightDTO.ScheduledFlightId = scheduledFlight.Id;

            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(user.Email)
                .Returns(user);
            _unitOfWork.ScheduledFlightRepository
                .GetFlightDetailsByFlightId(_bookFlightDTO.ScheduledFlightId)
                .Returns(scheduledFlight);
            _userService.CheckDocument(user, scheduledFlight.Flight_date)
                .Returns("");

            // Act
            var result = await _sut.BookFlight(_bookFlightDTO, "test@test.com");

            // Arrange
            Assert.Null(result.Data);
            Assert.Equal("Only ACTIVE user can book flights", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task BookFlight_FlightStatusIsNotScheduled_ReturnsError()
        {
            // Arange
            var user = new User();
            user.Email = "test@test.com";
            user.UserStatus = Enums.UserStatus.ACTIVE;

            var scheduledFlight = new ScheduledFlight();
            scheduledFlight.Id = 1;
            scheduledFlight.Flight_date = DateTime.Now;
            scheduledFlight.Flight_status = Enums.FlightStatus.CANCELLED;

            _bookFlightDTO.ScheduledFlightId = scheduledFlight.Id;

            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(user.Email)
                .Returns(user);
            _unitOfWork.ScheduledFlightRepository
                .GetFlightDetailsByFlightId(_bookFlightDTO.ScheduledFlightId)
                .Returns(scheduledFlight);
            _userService.CheckDocument(user, scheduledFlight.Flight_date)
                .Returns("");

            // Act
            var result = await _sut.BookFlight(_bookFlightDTO, "test@test.com");

            // Arrange
            Assert.Null(result.Data);
            Assert.Equal("Cannot book the flight, because it is unavailable", result.Message);
            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        public async Task BookFlight_TicketQuantityExceeds5_ReturnsError(int ticketQuantity)
        {
            // Arange
            var user = new User();
            user.Email = "test@test.com";
            user.UserStatus = Enums.UserStatus.ACTIVE;
            user.IdentificationDocument = new IdentificationDocument();
            user.IdentificationDocument.Id = 1;
            user.DocumentId = 1;

            var scheduledFlight = new ScheduledFlight();
            scheduledFlight.Id = 1;
            scheduledFlight.Flight_date = DateTime.Now;
            scheduledFlight.Flight_status = Enums.FlightStatus.SCHEDULED;
            scheduledFlight.Departure = new Departure();
            scheduledFlight.Departure.Scheduled = DateTime.Now;
            scheduledFlight.Flight = new Flight();
            scheduledFlight.Flight.Icao = "TEST";
            scheduledFlight.Arrival = new Arrival();
            scheduledFlight.Arrival.Airport = "TEST";

            _bookFlightDTO.ScheduledFlightId = scheduledFlight.Id;
            _bookFlightDTO.TicketQuantity = 1;

            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(user.Email)
                .Returns(user);
            _unitOfWork.ScheduledFlightRepository
                .GetFlightDetailsByFlightId(_bookFlightDTO.ScheduledFlightId)
                .Returns(scheduledFlight);
            _userService.CheckDocument(user, scheduledFlight.Flight_date)
                .Returns("");

            var alreadyScheduledFlight = new BookedFlight();
            alreadyScheduledFlight.TicketQuantity = ticketQuantity;
            var alreadyScheduledFlights = new List<BookedFlight>
            {
                alreadyScheduledFlight
            };
            _unitOfWork.BookedFlightRepository
                .GetBookedFlightsByUserEmailIcaoAndDepartureDate(user.Email, scheduledFlight.Flight.Icao, scheduledFlight.Departure.Scheduled)
                .Returns(alreadyScheduledFlights);

            // Act
            var result = await _sut.BookFlight(_bookFlightDTO, "test@test.com");

            // Arrange
            Assert.Null(result.Data);
            Assert.Equal("Cannot book flight, exceeded maximum amout of tickets that can be bought: 5", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task BookFlight_HappyPathUserHasNoPreviousBookings_ReturnsSuccess()
        {
            var email = "test@test.com";
            var scheduledFlight = new ScheduledFlight();
            scheduledFlight.Departure = new Departure();
            scheduledFlight.Departure.Scheduled = DateTime.Now;
            scheduledFlight.Flight = new Flight();
            scheduledFlight.Flight.Icao = "TEST";
            SetupHappyPathData(scheduledFlight);
            _unitOfWork.BookedFlightRepository
                .GetBookedFlightsByUserEmailIcaoAndDepartureDate(email, scheduledFlight.Flight.Icao, scheduledFlight.Departure.Scheduled)
                .ReturnsNull();

            // Act
            var result = await _sut.BookFlight(_bookFlightDTO, email);

            // Arrange
            Assert.Null(result.Data);
            Assert.Equal("Succesfully booked flight!", result.Message);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task BookFlight_HappyPathTicketQuantityDoesNotExceede5_ReturnsSuccess()
        {
            var email = "test@test.com";
            var scheduledFlight = new ScheduledFlight();
            scheduledFlight.Departure = new Departure();
            scheduledFlight.Departure.Scheduled = DateTime.Now;
            scheduledFlight.Flight = new Flight();
            scheduledFlight.Flight.Icao = "TEST";
            var previousBookings = SetupHappyPathData(scheduledFlight);
            _unitOfWork.BookedFlightRepository
                .GetBookedFlightsByUserEmailIcaoAndDepartureDate(email, scheduledFlight.Flight.Icao, scheduledFlight.Departure.Scheduled)
                .Returns(previousBookings);

            // Act
            var result = await _sut.BookFlight(_bookFlightDTO, email);

            // Arrange
            Assert.Null(result.Data);
            Assert.Equal("Succesfully booked flight!", result.Message);
            Assert.True(result.Success);
        }

        private List<BookedFlight> SetupHappyPathData(ScheduledFlight scheduledFlight)
        {
            // Arange
            var user = new User();
            user.Email = "test@test.com";
            user.UserStatus = Enums.UserStatus.ACTIVE;

            scheduledFlight.Id = 1;
            scheduledFlight.Flight_date = DateTime.Now;
            scheduledFlight.Flight_status = Enums.FlightStatus.SCHEDULED;

            scheduledFlight.Departure.Airport = "TEST";
            scheduledFlight.Arrival = new Arrival();
            scheduledFlight.Arrival.Airport = "TEST";
            scheduledFlight.Arrival.Scheduled = DateTime.Now;

            _bookFlightDTO.ScheduledFlightId = scheduledFlight.Id;
            _bookFlightDTO.TicketQuantity = 1;

            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(user.Email)
                .Returns(user);
            _unitOfWork.ScheduledFlightRepository
                .GetFlightDetailsByFlightId(_bookFlightDTO.ScheduledFlightId)
                .Returns(scheduledFlight);
            _userService.CheckDocument(user, scheduledFlight.Flight_date)
                .Returns("");

            var alreadyScheduledFlight = new BookedFlight();
            alreadyScheduledFlight.TicketQuantity = 1;
            var alreadyScheduledFlights = new List<BookedFlight>
            {
                alreadyScheduledFlight
            };

            return alreadyScheduledFlights;
        }
    }
}