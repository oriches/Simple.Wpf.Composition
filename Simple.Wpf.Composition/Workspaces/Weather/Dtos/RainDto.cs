namespace Simple.Wpf.Composition.Workspaces.Weather.Dtos
{
    using Newtonsoft.Json;

    public class RainDto
    {
        [JsonProperty(PropertyName = "1h")]
        public double oneH { get; set; }
    }
}