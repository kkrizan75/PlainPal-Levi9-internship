using AutoMapper;
using PlanePal.DTOs.BookedFlight;
using PlanePal.Model.FlightModel;
using PlanePal.Model.Shared;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Interfaces;
using Serilog;

namespace PlanePal.Services.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public BookingService(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<string>> BookFlight(BookFlightDTO dto, string userEmail)
        {
            var user = await _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(userEmail);
            var scheduledFlight =
                await _unitOfWork.ScheduledFlightRepository.GetFlightDetailsByFlightId(dto.ScheduledFlightId);

            if (user is null || scheduledFlight is null)
            {
                return new ServiceResponse<string>(null, false, $"User with email {userEmail} not found or scheduled flight not found.");
            }
            string documentCheck = _userService.CheckDocument(user, scheduledFlight.Flight_date);

            if (documentCheck != "")
            {
                return new ServiceResponse<string>(null, false, documentCheck);
            }

            if (user.UserStatus != Enums.UserStatus.ACTIVE)
            {
                return new ServiceResponse<string>(null, false, "Only ACTIVE user can book flights");
            }

            if (scheduledFlight.Flight_status != Enums.FlightStatus.SCHEDULED)
            {
                return new ServiceResponse<string>(null, false, "Cannot book the flight, because it is unavailable");
            }

            if (!(await IsTicketQuantityValid(dto, scheduledFlight, userEmail)))
            {
                return new ServiceResponse<string>(null, false, "Cannot book flight, exceeded maximum amout of tickets that can be bought: 5");
            }

            try
            {
                var bookedFlight = new BookedFlight(user, scheduledFlight, dto.TicketQuantity);
                _unitOfWork.BookedFlightRepository.Add(bookedFlight);
                await _unitOfWork.SaveChanges();

                var emailBody =
                    $"Dear {userEmail}. You have successfully booked flight at {bookedFlight.DepartureDate}."
                    + $"Departure airport: {bookedFlight.DepartureAirport}."
                    + $"Arrival airport: {bookedFlight.ArrivalAirport}."
                    + $"Ticket quantity: {bookedFlight.TicketQuantity}";

                await _emailService.SendMail(user.Email, "Reservation confirmation", emailBody);
                Log.Information("Email successfully sent");
                return new ServiceResponse<string>(null, true, "Succesfully booked flight!");
            }
            catch (Exception ex)
            {
                Log.Error("There was a problem with the email service: {@ErrorMessage}", ex.Message);
                return new ServiceResponse<string>(null, false, ex.Message);
            }
        }

        private async Task<bool> IsTicketQuantityValid(BookFlightDTO dto, ScheduledFlight scheduledFlight, string email)
        {
            var userBookedFlights = await _unitOfWork.BookedFlightRepository
                .GetBookedFlightsByUserEmailIcaoAndDepartureDate(email, scheduledFlight.Flight.Icao, scheduledFlight.Departure.Scheduled);
            // if it's null it means that this is the users first reservation for the current flight,
            // the ticket quantity does not exceed 5
            if (userBookedFlights is null) return true;

            int ticketQuantity = 0;
            foreach (var bookedFlight in userBookedFlights)
            {
                ticketQuantity += bookedFlight.TicketQuantity;
            }
            if (ticketQuantity >= 5 || ticketQuantity + dto.TicketQuantity > 5) return false;
            return true;
        }

        public async Task<ServiceResponse<IEnumerable<BookedFlightInfoDTO>>> GetBookedFlights(string email)
        {
            var user = _unitOfWork.UserRepository.GetOne(email);
            if (user is null)
            {
                return new ServiceResponse<IEnumerable<BookedFlightInfoDTO>>(null, false, $"User with email {email} not found");
            }
            try
            {
                var bookedFlights = await _unitOfWork.BookedFlightRepository.GetAllBy(x => x.UserEmail == email);
                if (bookedFlights.Any())
                {
                    return new ServiceResponse<IEnumerable<BookedFlightInfoDTO>>(_mapper.Map<IEnumerable<BookedFlightInfoDTO>>(bookedFlights), true);
                }
                return new ServiceResponse<IEnumerable<BookedFlightInfoDTO>>(null, true, "You don't have any booked flights yet!");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<BookedFlightInfoDTO>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResponse<string>> UpdateBookedFlight(UpdateBookedFlightDTO updateBookedFlightDTO, string email)
        {
            var bookedFlight = _unitOfWork.BookedFlightRepository.GetOne(updateBookedFlightDTO.Id);
            if (bookedFlight is null)
            {
                return new ServiceResponse<string>(false, $"Booked flight with id {updateBookedFlightDTO.Id} not found");
            }
            if (bookedFlight.UserEmail != email)
            {
                return new ServiceResponse<string>(null, false, $"User is not authorized.");
            }
            if (bookedFlight.DepartureDate < DateTime.Now.AddHours(2))
            {
                return new ServiceResponse<string>(null, false, "Booking can't be updated, because it is in less than 2 hours.");
            }
            if (updateBookedFlightDTO.TicketQuantity <= 0 || updateBookedFlightDTO.TicketQuantity > 5)
            {
                return new ServiceResponse<string>(false, "Ticket quantity cannot be less than 1 or more than 5.");
            }
            try
            {
                bookedFlight.TicketQuantity = updateBookedFlightDTO.TicketQuantity;
                if (await _unitOfWork.BookedFlightRepository.UpdateAndSave(bookedFlight) != 0)
                {
                    return new ServiceResponse<string>(true, "Succesfully updated!");
                }

                return new ServiceResponse<string>(false, "Failed to update the booking in the database");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message);
            }
        }

        public async Task<ServiceResponse<string>> DeleteBooking(int id, string email)
        {
            var booking = _unitOfWork.BookedFlightRepository.GetOne(id);
            if (booking == null)
            {
                return new ServiceResponse<string>(null, false, $"Flight with an id {id}  does not exists.");
            }
            if (booking.UserEmail != email)
            {
                return new ServiceResponse<string>(null, false, $"User is not authorized.");
            }
            if (booking.DepartureDate >= DateTime.Now.AddHours(2))
            {
                booking.IsCanceled = true;
                if (await _unitOfWork.BookedFlightRepository.UpdateAndSave(booking) != 0)
                {
                    return new ServiceResponse<string>(null, true, "Booking successfully canceled.");
                }

                return new ServiceResponse<string>(null, false, "Failed to cancel the booking, error while updating the database.");
            }
            else
            {
                return new ServiceResponse<string>(null, false, "Booking can't be canceled, because it is in 2 hours.");
            }
        }
    }
}