namespace PlanePal.DbContext
{
    using Microsoft.EntityFrameworkCore;
    using PlanePal.Model.FlightModel;
    using PlanePal.Model.Static;
    using PlanePal.Model.UserModel;

    public class PlanePalDbContext : DbContext
    {
        public PlanePalDbContext(DbContextOptions<PlanePalDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Airline> Airlines { get; set; }
        public DbSet<IdentificationDocument> IdentificationDocuments { get; set; }
        public DbSet<ScheduledFlight> ScheduledFlights { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Departure> Departures { get; set; }
        public DbSet<Arrival> Arrivals { get; set; }
        public DbSet<FlightAirline> FlightAirlines { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<BookedFlight> BookedFlights { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasAlternateKey(u => u.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Email);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Id)
                .IsUnique();
        }
    }
}