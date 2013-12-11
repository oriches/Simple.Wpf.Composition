namespace Simple.Wpf.Composition.Workspaces.Weather.Services
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Dtos;
    using Model;
    using Rest;
    using Rest.Extensions;

    public sealed class WeatherService : IWeatherService
    {
        public async Task<CityWeather> GetCityWeather(string country, string city)
        {
            var url = new Uri(string.Format("http://api.openweathermap.org/data/2.5/weather?q={0},{1}", city, country));
            var result = await new RestClient()
                .WithGzipEncoding()
                .WithCredentials(CredentialCache.DefaultCredentials)
                .GetAsync<SnapshotDto>(url);

            if (!result.Successfully)
            {
                throw result.Exception;
            }

            var resource = result.Resource;
            return new CityWeather(city, resource.main.temp, resource.weather[0].description);
        }
    }
}