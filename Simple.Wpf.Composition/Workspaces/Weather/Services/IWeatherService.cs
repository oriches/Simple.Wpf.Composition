namespace Simple.Wpf.Composition.Workspaces.Weather.Services
{
    using System.Threading.Tasks;
    using Model;

    public interface IWeatherService
    {
        Task<CityWeather> GetCityWeather(string country, string city);
    }
}