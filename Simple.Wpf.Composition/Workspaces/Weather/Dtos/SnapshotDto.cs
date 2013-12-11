namespace Simple.Wpf.Composition.Workspaces.Weather.Dtos
{
    public class SnapshotDto
    {
        public SysDto sys { get; set; }

        public WeatherDto[] weather { get; set; }

        public string @base { get; set; }

        public MainDto main { get; set; }

        public WindDto wind { get; set; }

        public RainDto rain { get; set; }

        public CloudsDto clouds { get; set; }

        public int dt { get; set; }

        public int id { get; set; }

        public string name { get; set; }

        public int cod { get; set; }
    }
}