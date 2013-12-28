namespace Simple.Wpf.Composition.Workspaces.Weather
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Subjects;
    using System.Security.Cryptography;
    using Infrastructure;
    using Model;

    public sealed class WeatherViewModel : BaseViewModel
    {
        private readonly BehaviorSubject<string> _selectedCitySubject;
        private readonly ObservableCollection<string> _cities;

        private string _selectedCity;
        private string _country;
        private CityWeather _weather;

        public WeatherViewModel()
        {
            _selectedCitySubject = new BehaviorSubject<string>(null);
            _cities = new ObservableCollection<string>();
        }

        public IObservable<string> SelectedCityStream { get { return _selectedCitySubject; } }

        public string Country { get { return _country; } }

        public IEnumerable<string> Cities { get { return _cities; } }

        public string SelectedCity
        {
            get
            {
                return _selectedCity;
            }
            set
            {
                if (SetPropertyAndNotify(ref _selectedCity, value))
                {
                    _selectedCitySubject.OnNext(_selectedCity);
                }
            }
        }

        public double? Temperature { get { return  _weather != null ? (double?)_weather.Temp : null;  } }

        public string Description { get { return _weather != null ? _weather.Description : null; } }

        public void AddCountryAndCities(string country, IEnumerable<string> cities)
        {
            _country = country;

            _cities.Clear();

            foreach (var city in cities)
            {
                _cities.Add(city);
            }
        }

        public void Update(CityWeather weather)
        {
            _weather = weather;

            OnPropertyChanged(() => Temperature);
            OnPropertyChanged(() => Description);
        }
    }
}