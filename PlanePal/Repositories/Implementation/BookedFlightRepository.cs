using Microsoft.EntityFrameworkCore;
using PlanePal.DbContext;
using PlanePal.Model.FlightModel;
using PlanePal.Repositories.Interfaces;

namespace PlanePal.Repositories.Implementation
{
    public class BookedFlightRepository : BaseRepository<BookedFlight, int>, IBookedFlightRepository
    {
        private readonly PlanePalDbContext _context;

        public BookedFlightRepository(PlanePalDbContext dataContext) : base(dataContext)
        {
            _context = dataContext;
        }

        public async Task<IEnumerable<BookedFlight>> GetBookedFlightsByUserEmailIcaoAndDepartureDate
            (string email, string icao, DateTime departureDate)
        {
            return await _context.BookedFlights
                .Where(bf => bf.UserEmail.Equals(email)
                    && bf.DepartureDate == departureDate
                    && bf.FlightIcao.Equals(icao))
                .ToListAsync();
        }

        public async Task<IEnumerable<BookedFlight>> GetFutureBookedFlightsByUserEmail(string email)
        {
            return await _context.BookedFlights
               .Where(bf => bf.UserEmail.Equals(email)
                   && bf.DepartureDate >= DateTime.Now
                   && bf.IsCanceled == false)
               .ToListAsync();
        }
    }
}