using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAppX.Helper;
using WeatherAppX.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherAppX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        public CurrentWeatherPage()
        {
            InitializeComponent();
            //GetWeatherInfo();
            GetCoordinate();
        }

        private string Location { get; set; } = "France";
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        private async void GetCoordinate()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                if(location != null)
                {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                    Location = await GetCity(location);
                    Location = "Lagos";

                    GetWeatherInfo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task<string> GetCity(Location location)
        {
            var palces = await Geocoding.GetPlacemarksAsync(location);

            var currentPalce = palces?.FirstOrDefault();

            if (currentPalce != null)
                return $"{currentPalce.Locality} , {currentPalce.CountryName}";

            return null;
        }

        private async void GetBackground()
        {
            var url = $"https://api.pexels.com/v1/search?query={Location}&per_page=15&page=1";

            var result = await ApiCaller.Get(url, "563492ad6f91700001000001b8c7c360a98b428e97de9684d950dc5e");
            if (result.IsSuccessful)
            {
                var bgInfo = JsonConvert.DeserializeObject<BackgroundInf>(result.Response);
                if(bgInfo != null && bgInfo.photos.Length> 0)
                {
                    bgImg.Source = ImageSource.FromUri(new Uri(bgInfo.photos[new Random().Next(0, bgInfo.photos.Length -1)]
                                                            .src.medium));
                }
            }
        }

        private async void GetWeatherInfo()
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={Location}&appid=8a5fb922f61645e6df8ce9dfa21a50d9&units=metric";

            var result = await ApiCaller.Get(url);
            if (result.IsSuccessful)
            {
                try
                {
                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);
                    descriptionTxt.Text = weatherInfo.weather[0].description.ToUpper();
                    iconImg.Source = $"w{weatherInfo.weather[0].icon}";
                    cityTxt.Text = weatherInfo.name.ToUpper();
                    temperatureTxt.Text = weatherInfo.main.temp.ToString("0");
                    humidityTxt.Text = $"{weatherInfo.main.humidity}%";
                    pressureTxt.Text = $"{weatherInfo.main.pressure} hpa";
                    windTxt.Text = $"{weatherInfo.wind.speed} m/s";
                    cloudinessTxt.Text = $"{weatherInfo.clouds.all}%";

                   
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(weatherInfo.dt);
                    
                    var date = dateTimeOffset.DateTime.Date;
                    
                    dateTxt.Text = date.ToString("dddd, MMM, dd").ToUpper();

                    GetForecast();
                    GetBackground();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No weather information found ", "OK");
            }
        }

        private async void GetForecast()
        {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=8a5fb922f61645e6df8ce9dfa21a50d9&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.IsSuccessful)
            {
                try
                {
                    var forecastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();
                    foreach (var info in forecastInfo.list)
                    {
                        var date = DateTime.Parse(info.dt_txt);
                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(info);
                    }

                    dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MM");
                    iconOneImg.Source = $"w{allList[0].weather[0].icon}";
                    tempOneTxt.Text = allList[0].main.temp.ToString("0");

                    dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dddd");
                    dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MM");
                    iconTwoImg.Source = $"w{allList[1].weather[0].icon}";
                    tempTwoTxt.Text = allList[1].main.temp.ToString("0");

                    dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dddd");
                    dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MM");
                    iconThreeImg.Source = $"w{allList[2].weather[0].icon}";
                    tempThreeTxt.Text = allList[2].main.temp.ToString("0");

                    dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dddd");
                    dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MM");
                    iconFourImg.Source = $"w{allList[3].weather[0].icon}";
                    tempFourTxt.Text = allList[3].main.temp.ToString("0");
                }
                catch (Exception ex)
                {
                   await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
        }
    }
}