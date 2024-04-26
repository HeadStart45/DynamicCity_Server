using DynamicCity_Models;
using DynamicCity_Models.Character;
using Microsoft.Extensions.Options;

namespace DynamicCity.Services
{
    public interface ICityService
    {
        public void LoadCity();
        public List<Character> GetCharacters();
        public int GetSecondsPerMinuteInMilliseconds();
        public void AddMinute();

        public void Tick();

        public DateTime GetDateTime();
    }
    
    public class CityService : ICityService
    {
        public City City { get; set; }
        private IDatabaseService databaseService;

        
        public CityService(IOptions<CityInitData> cityInitData, IDatabaseService _databaseService) 
        {
            City = new City(cityInitData.Value.MaxX, cityInitData.Value.MaxY);
            databaseService = _databaseService;
        }

        public async void LoadCity() 
        {
            City.CharacterList = await databaseService.GetCharactersAsync();
        }

        public List<Character> GetCharacters()
        {
            return City.CharacterList;
        }

        public int GetSecondsPerMinuteInMilliseconds()
        {
            return City.SecondsPerGameMinute * 1000;
        }

        public void AddMinute()
        {
            City.DateTime = City.DateTime.AddMinutes(1);
            
            if(City.MinutesSinceTick >= City.GameMinutesPerTick)
            {
                Tick();
                City.MinutesSinceTick = 0;
            }
            else
            {
                City.MinutesSinceTick++;
            }
        }

        public void Tick()
        {
            foreach (var character in City.CharacterList)
            {
                character.Tick();
            }
        }

        public DateTime GetDateTime()
        {
            return City.DateTime;
        }
    }
}
