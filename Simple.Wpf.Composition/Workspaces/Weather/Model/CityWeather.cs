namespace Simple.Wpf.Composition.Workspaces.Weather.Model
{
    public sealed class CityWeather
    {
        public CityWeather(string city, double temp, string description)
        {
            City = city;
            Temp = temp;
            Description = description;
        }

        public double Temp { get; }
        public string City { get; }
        public string Description { get; }
    }
}