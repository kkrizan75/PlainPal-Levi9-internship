using Microsoft.EntityFrameworkCore;
using PlanePal.DbContext;
using PlanePal.Model.FlightModel;
using PlanePal.Repositories.Interfaces;

namespace PlanePal.Repositories.Implementation
{
    public class ScheduledFLightRepository : BaseRepository<ScheduledFlight, int>, IScheduledFlightRepository
    {
        private readonly PlanePalDbContext _context;

        public ScheduledFLightRepository(PlanePalDbContext dataContext) : base(dataContext)
        {
            _context = dataContext;
        }

        public async Task<IEnumerable<ScheduledFlight>> GetAllFlights()
        {
            return await _dataContext.Set<ScheduledFlight>()
                .Include("Departure")
                .Include("Arrival")
                .Include("Airline")
                .Include("Flight")
                .DefaultIfEmpty()
                .ToListAsync();
        }

        public async Task<ScheduledFlight> GetFlightDetailsByFlightId(int id)
        {
            return await _context.ScheduledFlights
                .Include(s => s.Departure)
                .Include(s => s.Arrival)
                .Include(s => s.Airline)
                .Include(s => s.Flight)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}