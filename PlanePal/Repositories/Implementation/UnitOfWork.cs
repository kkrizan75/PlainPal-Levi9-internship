using PlanePal.DbContext;
using PlanePal.Repositories.Interfaces;

namespace PlanePal.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PlanePalDbContext _dbContext;
        private IUserRepository _userRepository;
        private IScheduledFlightRepository _scheduledFlightRepository;
        private IAirportRepository _airportRepository;
        private IAirlineRepository _airlineRepository;
        private IBookedFlightRepository _bookedFlightRepository;
        private IDocumentRepository _documentRepository;
        private IUserRoleRepository _userRoleRepository;

        public UnitOfWork(PlanePalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_dbContext);
        public IScheduledFlightRepository ScheduledFlightRepository => _scheduledFlightRepository ??= new ScheduledFLightRepository(_dbContext);
        public IAirportRepository AirportRepository => _airportRepository ??= new AirportRepository(_dbContext);
        public IAirlineRepository AirlineRepository => _airlineRepository ??= new AirlineRepository(_dbContext);
        public IBookedFlightRepository BookedFlightRepository => _bookedFlightRepository ??= new BookedFlightRepository(_dbContext);
        public IDocumentRepository DocumentRepository => _documentRepository ??= new DocumentRepository(_dbContext);

        public IUserRoleRepository UserRoleRepository => _userRoleRepository ??= new UserRoleRepository(_dbContext);

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}