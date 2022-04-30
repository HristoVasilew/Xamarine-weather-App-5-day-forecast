using CompleteWeatherApp.Helper;
using CompleteWeatherApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnweWeatherApp;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompleteWeatherApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {

        public CurrentWeatherPage()
        {
            InitializeComponent();
            GetWeatherWithGeoLoaction();
        }

        private string Location = "Sofia";

        private void OnGetWeatherButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_cityEntry.Text))
            {

                Location = _cityEntry.Text;
                GetWeatherInfo(GenerateRequestUri(Constants.OpenWeatherMapEndpoint));

                GetForecast();
            }
        }

        async void GetWeatherWithGeoLoaction()
        {

            var location = await Geolocation.GetLocationAsync();

            if (location != null)
            {
                var result = await ApiCaller.Get(GenerateRequesturiGeo(Constants.OpenWeatherMapEndpoint, location.Latitude, location.Longitude));
                var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);
                _cityEntry.Text = weatherInfo.name.ToUpper();
                Location = weatherInfo.name.ToUpper();

                GetWeatherInfo(GenerateRequestUri(Constants.OpenWeatherMapEndpoint));
                GetForecast();
            }
        }

        string GenerateRequestUri(string endpoint)
        {
            string requestUri = endpoint;
            requestUri += $"?q={_cityEntry.Text}";
            requestUri += "&units=metric"; // or units=metric
            requestUri += $"&APPID={Constants.OpenWeatherMapAPIKey}";
            Location = _cityEntry.Text;
            return requestUri;
        }

        string GenerateRequesturiGeo(string endpoint, double lati, double longt)
        {
            string requestUri = endpoint;
            requestUri += $"?lat={lati}";
            requestUri += $"&lon={longt}";
            requestUri += "&units=imperial"; // or units-metric
            requestUri += $"&APPID={Constants.OpenWeatherMapAPIKey}";
            Location = cityTxt.Text;

            return requestUri;
        }

        private async void GetWeatherInfo(string url)
        {


            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);
                    descriptionTxt.Text = weatherInfo.weather[0].description.ToUpper();
                    iconImg.Source = $"w{weatherInfo.weather[0].icon}";
                    cityTxt.Text = weatherInfo.name.ToUpper();
                    _cityEntry.Text = cityTxt.Text;
                    temperatureTxt.Text = weatherInfo.main.temp.ToString("0");
                    temperatureTxtmin.Text = weatherInfo.main.temp_min.ToString("0") + " / " + weatherInfo.main.temp_max.ToString("0");
                    
                    humidityTxt.Text = $"{weatherInfo.main.humidity}%";
                    pressureTxt.Text = $"{weatherInfo.main.pressure} hpa";
                    windTxt.Text = $"{weatherInfo.wind.speed} m/s";
                    cloudinessTxt.Text = $"{weatherInfo.clouds.all}%";

                    var dt = new DateTime().ToUniversalTime().AddSeconds(weatherInfo.dt);
                    dateTxt.Text = dt.ToString("dddd, MMM dd").ToUpper();

                    GetForecast();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No weather information found", "OK");
            }
        }

        private async void GetForecast()
        {
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=2c24436d9d9a44bc6d9eae99d7835bb9&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {
                        //var date = DateTime.ParseExact(list.dt_txt, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    dayOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MMM");
                    iconOneImg.Source = $"w{allList[0].weather[0].icon}";
                    tempOneTxt.Text = allList[0].main.temp.ToString("0");

                    dayTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dddd");
                    dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MMM");
                    iconTwoImg.Source = $"w{allList[1].weather[0].icon}";
                    tempTwoTxt.Text = allList[1].main.temp.ToString("0");

                    dayThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dddd");
                    dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MMM");
                    iconThreeImg.Source = $"w{allList[2].weather[0].icon}";
                    tempThreeTxt.Text = allList[2].main.temp.ToString("0");

                    dayFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dddd");
                    dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MMM");
                    iconFourImg.Source = $"w{allList[3].weather[0].icon}";
                    tempFourTxt.Text = allList[3].main.temp.ToString("0");

                    dayFiveTxt.Text = DateTime.Parse(allList[4].dt_txt).ToString("dddd");
                    dateFiveTxt.Text = DateTime.Parse(allList[4].dt_txt).ToString("dd MMM");
                    iconFiveImg.Source = $"w{allList[4].weather[0].icon}";
                    tempFiveTxt.Text = allList[4].main.temp.ToString("0");

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }
    }
}