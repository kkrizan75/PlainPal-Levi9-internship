using AutoMapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using PlanePal.DTOs.BookedFlight;
using PlanePal.Model.FlightModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using Xunit;

namespace PlanePal.BookingUnitTests
{
    public class UpdateBookedFlightTests
    {
        private readonly IBookingService _sut;
        private readonly IUserService _userService = Substitute.For<IUserService>();
        private readonly IEmailService _emailService = Substitute.For<IEmailService>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly UpdateBookedFlightDTO _updateBookedFlightDTO;

        public UpdateBookedFlightTests()
        {
            _sut = new BookingService(_unitOfWork, _userService, _mapper, _emailService);
            _updateBookedFlightDTO = new UpdateBookedFlightDTO();
        }

        [Fact]
        public async Task Update_IncorrectBookedFlightId_ReturnsError()
        {
            // Arange
            var flight = new BookedFlight();
            flight.Id = 666;
            _unitOfWork.BookedFlightRepository.GetOne(Arg.Any<int>()).ReturnsNull();

            // Act
            var result = await _sut.UpdateBookedFlight(_updateBookedFlightDTO, "test@gmail.com");

            // Assert
            Assert.Null(result.Data);
            Assert.Equal($"Booked flight with id {_updateBookedFlightDTO.Id} not found", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Update_IncorrectUserEmail_ReturnsError()
        {
            // Arange
            var flight = new BookedFlight();
            flight.Id = 666;
            _unitOfWork.BookedFlightRepository.GetOne(Arg.Any<int>()).Returns(flight);
            flight.UserEmail = "incorrectEmail@gmail.com";

            // Act
            var result = await _sut.UpdateBookedFlight(_updateBookedFlightDTO, "test@gmail.com");

            // Assert
            Assert.Null(result.Data);
            Assert.Equal($"User is not authorized.", result.Message);
            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Update_FlightInNextTwoHours_ReturnsError(int departureHours)
        {
            // Arange
            var bookedFlight = new BookedFlight();
            bookedFlight.DepartureDate = DateTime.Now.AddHours(departureHours);
            bookedFlight.UserEmail = "test@gmail.com";

            // Act
            _unitOfWork.BookedFlightRepository.GetOne(Arg.Any<int>()).Returns(bookedFlight);

            // Assert
            var result = await _sut.UpdateBookedFlight(_updateBookedFlightDTO, "test@gmail.com");
            Assert.Equal("Booking can't be updated, because it is in less than 2 hours.", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Update_TicketsQuantityMoreThan5_ReturnsError()
        {
            //Arange
            _updateBookedFlightDTO.Id = 666;
            _updateBookedFlightDTO.TicketQuantity = 911;
            var bookedFlight = new BookedFlight();
            bookedFlight.DepartureDate = DateTime.Now.AddDays(1);
            bookedFlight.UserEmail = "test@gmail.com";
            _unitOfWork.BookedFlightRepository.GetOne(Arg.Any<int>()).Returns(bookedFlight);

            // Act
            var result = await _sut.UpdateBookedFlight(_updateBookedFlightDTO, "test@gmail.com");

            // Assert
            Assert.Null(result.Data);
            Assert.Equal("Ticket quantity cannot be less than 1 or more than 5.", result.Message);
            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Update_TicketsQuantityLessThan1_ReturnsError(int ticketQuantity)
        {
            //Arange
            _updateBookedFlightDTO.Id = 666;
            _updateBookedFlightDTO.TicketQuantity = ticketQuantity;
            var bookedFlight = new BookedFlight();
            bookedFlight.UserEmail = "test@gmail.com";
            bookedFlight.DepartureDate = DateTime.Now.AddDays(1);
            _unitOfWork.BookedFlightRepository.GetOne(Arg.Any<int>()).Returns(bookedFlight);

            // Act
            var result = await _sut.UpdateBookedFlight(_updateBookedFlightDTO, "test@gmail.com");

            // Assert
            Assert.Null(result.Data);
            Assert.Equal("Ticket quantity cannot be less than 1 or more than 5.", result.Message);
            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(2)]
        public async Task Update_HappyPath_ReturnsSuccess(int ticketQuantity)
        {
            //Arange
            _updateBookedFlightDTO.Id = 1;
            _updateBookedFlightDTO.TicketQuantity = ticketQuantity;
            var bookedFlight = new BookedFlight();
            bookedFlight.UserEmail = "test@gmail.com";
            bookedFlight.DepartureDate = DateTime.Now.AddDays(1);
            _unitOfWork.BookedFlightRepository.GetOne(Arg.Any<int>()).Returns(bookedFlight);
            _unitOfWork.BookedFlightRepository.UpdateAndSave(bookedFlight).Returns(1);

            // Act
            var result = await _sut.UpdateBookedFlight(_updateBookedFlightDTO, "test@gmail.com");

            // Assert
            Assert.Null(result.Data);
            Assert.Equal("Succesfully updated!", result.Message);
            Assert.True(result.Success);
        }
    }
}