using System.Threading.Tasks;
using Simple.Wpf.Composition.Workspaces.Weather.Model;

namespace Simple.Wpf.Composition.Workspaces.Weather.Services
{
    public interface IWeatherService
    {
        Task<CityWeather> GetCityWeather(string country, string city);
    }
}