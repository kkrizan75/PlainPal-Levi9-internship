using PlanePal.DbContext;
using PlanePal.Model.Static;
using PlanePal.Repositories.Interfaces;

namespace PlanePal.Repositories.Implementation
{
    public class AirportRepository : BaseRepository<Airport, int>, IAirportRepository
    {
        public AirportRepository(PlanePalDbContext dataContext) : base(dataContext)
        {
        }
    }
}