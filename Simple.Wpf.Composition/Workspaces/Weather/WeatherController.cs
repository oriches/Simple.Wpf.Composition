namespace Simple.Wpf.Composition.Workspaces.Weather
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Windows.Threading;
    using Services;

    public sealed class WeatherController : BaseController<WeatherViewModel>
    {
        private readonly IDisposable _weatherDisposable;

        public WeatherController(WeatherViewModel viewModel, IWeatherService weatherService)
            : base(viewModel)
        {
            viewModel.AddCountryAndCities("uk", new [] { "london", "manchester", "birmingham", "glasgow", "edinburgh" });

            _weatherDisposable = viewModel.SelectedCityStream
                .DistinctUntilChanged()
                .Where(x => x != null)
                .SelectMany(x => weatherService.GetCityWeather("uk", x).ToObservable())
                .ObserveOn(new DispatcherSynchronizationContext())
                .Subscribe(x => ViewModel.Update(x));
        }

        public override WeatherViewModel ViewModel
        {
            get
            {
                { return base.ViewModel; }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _weatherDisposable.Dispose();
        }
    }
}