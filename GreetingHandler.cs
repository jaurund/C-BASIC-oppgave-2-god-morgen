using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class GreetingHandler
{
    public static string GetGreetingLanguage(string input)
    {
        // Normalizes input from users, adds support for UTF-16
        string normalized = Regex.Replace(input, @"[^\p{L}]", "").ToLowerInvariant();

        return normalized switch
        {
            "goodmorning" => "en",
            "godmorgen" => "no",
            "gutenmorgen" => "de",
            "bonjour" => "fr",
            "buenosdias" => "es",
            _ => "unknown"
        };
    }

    public static string RespondToGreeting(string languageCode)
    {
        return languageCode switch
        {
            "en" => "Good morning to you too!",
            "no" => "God morgen til deg også!",
            "de" => "Guten Morgen auch!",
            "fr" => "Bonjour à vous aussi !",
            "es" => "¡Buenos días a ti también!",
            _ => "Sorry, I didn't understand that greeting, but let’s continue..."
        };
    }
}

public class Greeting
{
    public string LanguageCode { get; set; }
    public string Response { get; set; }

    public Greeting(string languageCode, string response)
    {
        LanguageCode = languageCode;
        Response = response;
    }
}