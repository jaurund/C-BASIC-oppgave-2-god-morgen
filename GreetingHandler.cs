using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DeepL;

public static class GreetingHandler
{
    private static readonly Translator translator;

    static GreetingHandler()
    {
        try
        {
            string apiKey = File.ReadAllText("key2deepL.txt").Trim();
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("DeepL API key is missing or empty.");
            translator = new Translator(apiKey);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize DeepL translator: {ex.Message}");
            throw;
        }
    }

    private static string MapToTargetLanguage(string detectedLang)
    {
        return detectedLang switch
        {
            "en" => "EN-GB",
            "de" => "DE",
            "fr" => "FR",
            "es" => "ES",
            "it" => "IT",
            "nl" => "NL",
            "pl" => "PL",
            "pt" => "PT-PT",
            "ru" => "RU",
            "da" => "DA",
            "nb" => "NB",
            "sv" => "SV",
            _ => "EN-GB"
        };
    }

    private static async Task<string> TranslateResponseAsync(string englishText, string targetLang)
    {
        if (targetLang.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            return englishText;

        string mappedTargetLang = MapToTargetLanguage(targetLang);

        try
        {
            Console.WriteLine($"Translating '{englishText}' to target language: {mappedTargetLang}");
            var result = await translator.TranslateTextAsync(englishText, null, mappedTargetLang);
            return result.Text;
        }
        catch (DeepLException ex)
        {
            Console.WriteLine($"Translation error: {ex.Message}");
            return englishText;
        }
    }

    public static async Task<string> HandleGreetingAsync(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
            return await TranslateResponseAsync("No input detected.", "en");

        string cleanedInput = Regex.Replace(userInput.ToLowerInvariant(), @"[^\w\s]", "").Trim();

        // 1. Detect language of the user input
        string detectedLang;
        try
        {
            var detection = await translator.TranslateTextAsync(userInput, null, "EN-GB");
            detectedLang = detection.DetectedSourceLanguageCode;
            Console.WriteLine($"Detected language: {detectedLang}");
        }
        catch (DeepLException ex)
        {
            Console.WriteLine($"Language detection error: {ex.Message}");
            detectedLang = "en";
        }

        // // 2. Commented out Levenshtein translation (this caused the error)
        // string localizedGreeting;
        // string mappedTargetLang = MapToTargetLanguage(detectedLang);
        // try
        // {
        //     Console.WriteLine($"Translating 'Good morning!' to target language: {mappedTargetLang}");
        //     var translatedGreeting = await translator.TranslateTextAsync("Good morning!", null, mappedTargetLang);
        //     localizedGreeting = Regex.Replace(translatedGreeting.Text.ToLowerInvariant(), @"[^\w\s]", "").Trim();
        // }
        // catch (DeepLException ex)
        // {
        //     Console.WriteLine($"Greeting translation error: {ex.Message}");
        //     localizedGreeting = GetFallbackGreeting(detectedLang);
        // }

        // // 3. Commented out Levenshtein comparison
        // var distance = new Levenshtein(cleanedInput);
        // int score = distance.DistanceFrom(localizedGreeting);

        // // 4. Commented out score check, assume input is a greeting
        // if (score < 5)
        // {
        //     return await TranslateResponseAsync("Good morning to you too!", detectedLang);
        // }
        // return await TranslateResponseAsync(
        //     "Sorry, I didn't understand that greeting, but let’s continue...",
        //     detectedLang
        // );

        // Simplified: Always respond with "Good morning to you too!" in the detected language
        return await TranslateResponseAsync("Good morning to you too!", detectedLang);
    }
}
