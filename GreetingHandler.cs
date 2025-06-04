using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DeepL;

public static class GreetingHandler
{
    private static Translator? translator;

    public static void Initialize(string apiKey)
    {
        try
        {
            translator = new Translator(apiKey);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize DeepL translator: {ex.Message}");
            translator = null;
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
            _ => "EN-GB",
        };
    }

    private static async Task<string> TranslateResponseAsync(string englishText, string targetLang)
    {
        if (translator is null)
            return "Translation service not available.";

        if (targetLang.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            return englishText;

        string mappedTargetLang = MapToTargetLanguage(targetLang);

        try
        {
            // Translate the English text to the target language
            var result = await translator.TranslateTextAsync(englishText, null, mappedTargetLang);
            return result.Text;
        }
        // If the target language is not supported, default to English
        catch (DeepLException ex)
        {
            Console.WriteLine($"Translation error: {ex.Message}");
            return englishText;
        }
    }

    public static async Task<string> HandleGreetingAsync(string userInput)
    {
        if (translator is null)
            return "DeepL translator not initialized.";

        if (string.IsNullOrWhiteSpace(userInput))
            return await TranslateResponseAsync("No input detected.", "en");

        string cleanedInput = Regex.Replace(userInput.ToLowerInvariant(), @"[^\w\s]", "").Trim();

        // 1. Detect language of the user input
        string detectedLang;
        try
        {
            var detection = await translator.TranslateTextAsync(userInput, null, "EN-GB");
            detectedLang = detection.DetectedSourceLanguageCode;
        }
        catch (DeepLException ex)
        {
            Console.WriteLine($"Language detection error: {ex.Message}");
            detectedLang = "en";
        }

        // Simplified: Always respond with "Good morning to you too!" in the detected language
        return await TranslateResponseAsync("Good morning to you too!", detectedLang);
    }
}
