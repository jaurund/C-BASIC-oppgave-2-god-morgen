using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        string? weatherKey = ReadApiKey("key.txt", "OpenWeatherMap");
        if (weatherKey is null)
            return;

        string? deepLKey = ReadApiKey("key2deepL.txt", "DeepL");
        if (deepLKey is null)
            return;

        GreetingHandler.Initialize(deepLKey);

        Console.WriteLine("Wish me a good morning, user!");
        string? userInput = Console.ReadLine();
        if (userInput is null)
        {
            Console.WriteLine("No input provided.");
            return;
        }

        string greetingResponse = await GreetingHandler.HandleGreetingAsync(userInput);
        Console.WriteLine(greetingResponse);

        string apiKey = weatherKey; //API key from OpenWeatherMap
        string city = "Bergen";
        string url =
            $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

        using HttpClient client = new HttpClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string? weatherMain = root.GetProperty("weather")[0].GetProperty("main").GetString();
            double temp = root.GetProperty("main").GetProperty("temp").GetDouble();

            Console.WriteLine($"The weather in {city}: {weatherMain}, Temperature: {temp}°C");

            // Categorize temperature
            string tempMessage = temp switch
            {
                <= 0 => "It's freezing today, be careful when driving and wear warm clothes.",
                <= 10 => "It's quite cold today. A proper jacket is a must.",
                <= 20 => "The temperature is mild, maybe bring a light jacket.",
                <= 30 => "It's a warm day, so light clothes should do.",
                _ => "It's very hot today — stay in the shade and hydrate!",
            };

            // Create message based on weather
            string weatherMessage = weatherMain switch
            {
                "Rain" => "Also, bring waterproofs — it's rainy.",
                "Clear" => "The skies are clear, so enjoy the sun!",
                "Snow" => "Snow is expected, so be cautious when walking.",
                "Clouds" => "It's cloudy, but dry for now.",
                "Drizzle" => "A light drizzle is in the air, maybe bring an umbrella.",
                _ => "Check outside for more detailed weather conditions.",
            };

            Console.WriteLine($"{tempMessage} {weatherMessage}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
        }
    }

    private static string? ReadApiKey(string filePath, string serviceName)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: {serviceName} API key file '{filePath}' not found.");
                return null;
            }

            string key = File.ReadAllText(filePath).Trim();
            if (string.IsNullOrWhiteSpace(key))
            {
                Console.WriteLine($"Error: {serviceName} API key in '{filePath}' is empty.");
                return null;
            }

            return key;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading {serviceName} API key: {ex.Message}");
            return null;
        }
    }
}
