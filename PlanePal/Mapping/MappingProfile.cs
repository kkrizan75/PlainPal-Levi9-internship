using AutoMapper;
using PlanePal.DTOs.BookedFlight;
using PlanePal.DTOs.ScheduledFlight;
using PlanePal.DTOs.User;
using PlanePal.Enums;
using PlanePal.Model.FlightModel;
using PlanePal.Model.Static;
using PlanePal.Model.UserModel;

namespace PlanePal.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //IdentificationDocument
            CreateMap<IdentificationDocument, DocumentDTO>().ReverseMap();
            CreateMap<IdentificationDocument, DocumentDetailsDTO>().ReverseMap();
            //Address
            CreateMap<Address, AddressDTO>().ReverseMap();

            //User
            CreateMap<CreateUserDTO, User>().ReverseMap();
            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.Address, opt =>
                opt.MapFrom(src => new Address
                {
                    StreetNumber = src.StreetNumber,
                    City = src.City,
                    Country = src.Country,
                    Street = src.Street,
                }));
            CreateMap<User, ReadUserDTO>();
            CreateMap<User, ReadUserDetailsDTO>();

            //Flight
            CreateMap<ScheduledFlight, ScheduledFlightDTO>()
                .ReverseMap()
                .ForMember(dest => dest.Flight_status, opt => opt.MapFrom(src => (FlightStatus)src.Flight_status));
            // Flight details
            CreateMap<ScheduledFlight, FlightDetailsDTO>()
                .ReverseMap();
            CreateMap<FlightDetailsDTO, ScheduledFlight>()
                .ReverseMap();
            CreateMap<ArrivalDTO, Arrival>()
                .ReverseMap();
            CreateMap<DepartureDTO, Departure>()
                .ReverseMap();
            CreateMap<FlightDTO, Flight>()
                .ReverseMap();
            CreateMap<AirLineDTO, FlightAirline>()
                .ReverseMap();

            //Airline
            CreateMap<AirlineDetailsDTO, Airline>().ReverseMap();
            // Flight details
            CreateMap<AirLineDTO, FlightAirline>().ReverseMap();
            CreateMap<Departure, DepartureDTO>().ReverseMap();
            CreateMap<Arrival, ArrivalDTO>().ReverseMap();
            CreateMap<Flight, FlightDTO>().ReverseMap();
            CreateMap<ScheduledFlight, FlightDetailsDTO>().ReverseMap();
            CreateMap<ScheduledFlight, ScheduledFlightDTO>().ReverseMap();
            //Airport
            CreateMap<AirportDetailsDTO, Airport>().ReverseMap();

            // booking flight

            CreateMap<BookedFlightInfoDTO, BookedFlight>().ReverseMap().ForMember(dest => dest.FlightDate, opt => opt.MapFrom(src => src.DepartureDate));
        }
    }
}