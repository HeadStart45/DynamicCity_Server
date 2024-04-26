
using DynamicCity_Models;

namespace DynamicCity.Services.BackgroundServices
{
    public class TimeLoopBGService : BackgroundService
    {
        private ICityService cityService {  get; set; } 
        public TimeLoopBGService(ICityService _cityService) 
        { 
            cityService = _cityService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(cityService.GetSecondsPerMinuteInMilliseconds());
                cityService.AddMinute();
            }
        }
    }


        
}
