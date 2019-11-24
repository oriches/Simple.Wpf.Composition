using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using Simple.Wpf.Composition.Infrastructure;
using Simple.Wpf.Composition.Workspaces.Weather.Model;

namespace Simple.Wpf.Composition.Workspaces.Weather
{
    public sealed class WeatherViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> _cities;
        private readonly BehaviorSubject<string> _selectedCitySubject;

        private string _selectedCity;
        private CityWeather _weather;

        public WeatherViewModel()
        {
            _selectedCitySubject = new BehaviorSubject<string>(null);
            _cities = new ObservableCollection<string>();
        }

        public IObservable<string> SelectedCityStream => _selectedCitySubject;

        public string Country { get; private set; }

        public IEnumerable<string> Cities => _cities;

        public string SelectedCity
        {
            get => _selectedCity;
            set
            {
                if (SetPropertyAndNotify(ref _selectedCity, value)) _selectedCitySubject.OnNext(_selectedCity);
            }
        }

        public double? Temperature => _weather != null ? (double?) _weather.Temp : null;

        public string Description => _weather != null ? _weather.Description : null;

        public void AddCountryAndCities(string country, IEnumerable<string> cities)
        {
            Country = country;

            _cities.Clear();

            foreach (var city in cities) _cities.Add(city);
        }

        public void Update(CityWeather weather)
        {
            _weather = weather;

            OnPropertyChanged(() => Temperature);
            OnPropertyChanged(() => Description);
        }
    }
}