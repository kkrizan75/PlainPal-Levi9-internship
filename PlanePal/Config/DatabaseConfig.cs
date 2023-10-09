using PlanePal.DbContext;

namespace PlanePal.Config
{
    public static class DatabaseConfig
    {
        public static void InitializeDatabase(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var dbContext = serviceProvider.GetRequiredService<PlanePalDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Airlines.RemoveRange(dbContext.Airlines);
                dbContext.Airports.RemoveRange(dbContext.Airports);
                dbContext.ScheduledFlights.RemoveRange(dbContext.ScheduledFlights);
                dbContext.Arrivals.RemoveRange(dbContext.Arrivals);
                dbContext.Departures.RemoveRange(dbContext.Departures);
                dbContext.Flights.RemoveRange(dbContext.Flights);
                dbContext.SaveChanges();
            }
        }
    }
}