using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;


class Program
{
    static async Task Main()
    {
        Console.WriteLine("Good morning, user!");
        string userInput = Console.ReadLine();

        string greetingResponse = await GreetingHandler.HandleGreetingAsync(userInput);
        Console.WriteLine(greetingResponse);

        string apiKey = "a405afdc1cd34e0aaa160424e6f97561"; //API key from OpenWeatherMap
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

            Console.WriteLine($"The weather in {city}: {weatherMain}, Temperature: {temp}°C");

            // Categorize temperature
            string tempMessage = temp switch
            {
                <= 0 => "It's freezing today, be careful when driving and wear warm clothes.",
                <= 10 => "It's quite cold today. A proper jacket is a must.",
                <= 20 => "The temperature is mild, maybe bring a light jacket.",
                <= 30 => "It's a warm day, so light clothes should do.",
                _ => "It's very hot today — stay in the shade and hydrate!"
            };

            // Create message based on weather
            string weatherMessage = weatherMain switch
            {
                "Rain" => "Also, bring waterproofs — it's rainy.",
                "Clear" => "The skies are clear, so enjoy the sun!",
                "Snow" => "Snow is expected, so be cautious when walking.",
                "Clouds" => "It's cloudy, but dry for now.",
                "Drizzle" => "A light drizzle is in the air, maybe bring an umbrella.",
                _ => "Check outside for more detailed weather conditions."
            };

            Console.WriteLine($"{tempMessage} {weatherMessage}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
        }
    }
}
