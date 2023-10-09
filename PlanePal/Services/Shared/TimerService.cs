using PlanePal.Services.Interfaces;
using Serilog;

namespace PlanePal.Services.Shared
{
    public class TimerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TimerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await FetchData();

            while (!stoppingToken.IsCancellationRequested)
            {
                // Set up the timer for every 12 hours
                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
                await FetchData();
            }
        }

        private async Task FetchData()
        {
            try
            {
                Log.Information("Calling external flight service");
                using IServiceScope scope = _serviceProvider.CreateScope();
                var flightService = scope.ServiceProvider.GetService<IFlightService>();
                await flightService.LoadData();
            }
            catch (Exception e)
            {
                Log.Error("Something is wrong with the timer service: {@ErrorMessage}", e.Message);
            }
        }
    }
}