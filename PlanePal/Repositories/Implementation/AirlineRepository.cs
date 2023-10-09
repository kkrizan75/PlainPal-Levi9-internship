using PlanePal.DbContext;
using PlanePal.Model.Static;
using PlanePal.Repositories.Interfaces;

namespace PlanePal.Repositories.Implementation
{
    public class AirlineRepository : BaseRepository<Airline, int>, IAirlineRepository
    {
        public AirlineRepository(PlanePalDbContext dataContext) : base(dataContext)
        {
        }
    }
}