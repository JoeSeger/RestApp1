using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RESTApp1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    ///
    ///
    ///
    public struct WeatherResponse
    {
        public string CityName { get; set; }  // The name of the city
        public float Temperature { get; set; } // Temperature in Celsius
        public string Description { get; set; } // Weather description (e.g., clear sky)

        // Override ToString to display weather information in a formatted way
        public override string ToString()
        {
            return $"Weather in {CityName}: {Temperature}°C, {Description}";
        }
    }
    public sealed partial class MainPage : Page
    {


        private const string ApiKey = "8d306e680790d338541c6f64448f1d3a";
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";



        public MainPage()
        {
            this.InitializeComponent();
        }

     

        private async void OnGetWeatherClick(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text;
            if (string.IsNullOrEmpty(city))
            {
                await new MessageDialog("Please enter city name").ShowAsync();
                return;
            }

            WeatherResponse weatherResponse = await GetWeatherAsync(city);

            ResultTextBlock.Text = weatherResponse.ToString();

        }

        private static async Task<WeatherResponse> GetWeatherAsync(string city)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"{BaseUrl}?q={city}&appid={ApiKey}&units=metric";
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return ParseWeatherResponse(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                return new WeatherResponse
                {
                    CityName = "Error",
                    Temperature = -1,
                    Description = ex.Message
                };
            }
        }

        private static WeatherResponse ParseWeatherResponse(string jsonResponse)
        {
            var json = JObject.Parse(jsonResponse);

            return  new WeatherResponse
            {
                CityName = json["name"]?.ToString(),
                Temperature = float.Parse(json["main"]?["temp"]?.ToString() ?? string.Empty),
                Description = json["weather"]?[0]?["description"]?.ToString()
            };
        }
    }
}
