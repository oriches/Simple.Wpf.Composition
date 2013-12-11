namespace Simple.Wpf.Composition.Workspaces.Weather.Model
{
    public sealed class CityWeather
    {
        public double Temp { get; private set; }
        public string City { get; private set; }
        public string Description { get; private set; }

        public CityWeather(string city, double temp, string description)
        {
            City = city;
            Temp = temp;
            Description = description;
        }
    }
}