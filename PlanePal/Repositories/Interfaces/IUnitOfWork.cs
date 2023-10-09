namespace PlanePal.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IScheduledFlightRepository ScheduledFlightRepository { get; }
        IAirlineRepository AirlineRepository { get; }
        IAirportRepository AirportRepository { get; }
        IBookedFlightRepository BookedFlightRepository { get; }
        IDocumentRepository DocumentRepository { get; }

        IUserRoleRepository UserRoleRepository { get; }

        Task SaveChanges();
    }
}