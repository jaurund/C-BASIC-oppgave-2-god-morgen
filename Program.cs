using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;


class Program
{
    static async Task Main()
    {
        string apiKey = "a405afdc1cd34e0aaa160424e6f97561"; //Key from OpenWeatherMap
        string city = "Bergen";
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

        using HttpClient client = new HttpClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string weatherMain = root.GetProperty("weather")[0].GetProperty("main").GetString();
            double temp = root.GetProperty("main").GetProperty("temp").GetDouble();

            Console.WriteLine($"Weather in {city}: {weatherMain}, Temperature: {temp}°C");

            // Greetings based on current weather
            if (temp < 0)
            {
                Console.WriteLine("Stay warm!");
            }
            else if (temp < 20)
            {
                Console.WriteLine("Have a nice day!");
            }
            else
            {
                Console.WriteLine("It's a hot day, stay hydrated!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
        }
    }
}
