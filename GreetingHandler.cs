using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DeepL;
using Fastenshtein;

public static class GreetingHandler
{
    private static readonly Translator translator;

    // Static constructor to initialize translator with key from file
    static GreetingHandler()
    {
        string apiKey = File.ReadAllText("key2deepL.txt")!.Trim();
        translator = new Translator(apiKey);
    }

    // Map detected source language to valid DeepL target language
    private static string MapToTargetLanguage(string detectedLang)
    {
        return detectedLang.ToUpper() switch
        {
            "EN" => "EN-GB", // Map generic English to British English
            "EN-US" => "EN-US",
            "EN-GB" => "EN-GB",
            "DE" => "DE",
            "FR" => "FR",
            "ES" => "ES",
            "IT" => "IT",
            "NL" => "NL",
            "PL" => "PL",
            "PT" => "PT-PT", // Map Portuguese to European Portuguese
            "RU" => "RU",
            // Add more mappings for other languages as needed
            _ => "EN-GB" // Fallback to English if language is unsupported as target
        };
    }

    private static async Task<string> TranslateResponseAsync(string englishText, string targetLang)
    {
        if (detectedLang.StartsWith("EN", StringComparison.OrdinalIgnoreCase))
            return englishText;

        string targetLang = MapToTargetLanguage(detectedLang);

        try
        {
            // Translate the English text to the target language
            var result = await translator.TranslateTextAsync(englishText, "EN-GB", targetLang);
            return result.Text;
        }
        catch (DeepLException ex)
        {
            Console.WriteLine($"Translation error: {ex.Message}");
            return englishText; // Fallback to original text on error
        }
    }
    public static async Task<string> HandleGreetingAsync(string userInput)
    {
        string cleanedInput = Regex.Replace(userInput.ToLowerInvariant(), @"[^\w\s]", "").Trim();

        // 1. Detect language of the user input, no translation yet
        var detection = await translator.TranslateTextAsync(userInput, null, "EN-GB");

        //debug
        Console.WriteLine($"Detected language: {detectedLang}");


        string detectedLang = detection.DetectedSourceLanguageCode.ToUpper();

        // 2. Translate default English greeting to the detected language for Levenshtein referencing
        var translatedGreeting = await translator.TranslateTextAsync("Good morning!", "EN-GB", detectedLang);
        string localizedGreeting = Regex.Replace(translatedGreeting.Text.ToLowerInvariant(), @"[^\w\s]", "").Trim();

        // 3. Compare cleaned user input with localized greeting using Levenshtein distance
        var distance = new Levenshtein(cleanedInput);
        int score = distance.DistanceFrom(localizedGreeting);

        // 4. if close enough, accept greeting, if not, return default response and proceed
        if (score < 5) // Arbitrary threshold for "closeness"
        {
            return await TranslateResponseAsync("Good morning to you too!", detectedLang);
        }

        return await TranslateResponseAsync(
            "Sorry, I didn't understand that greeting, but let’s continue...",
            detectedLang
        );
    }
}
