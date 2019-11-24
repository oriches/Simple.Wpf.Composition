using Newtonsoft.Json;

namespace Simple.Wpf.Composition.Workspaces.Weather.Dtos
{
    public class RainDto
    {
        [JsonProperty(PropertyName = "1h")] public double oneH { get; set; }
    }
}