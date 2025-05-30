using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fastenshtein;
using DeepL;

public static class GreetingHandler
{
    private static readonly string apiKey = "4a3aafff-6114-4528-92bc-a5b25a357357:fx";
    private static readonly Translator translator = new Translator(apiKey);

    public static async Task<string> HandleGreetingAsync(string userInput)
    {
        string cleanedInput = Regex.Replace(userInput.ToLowerInvariant(), @"[^\w\s]", "").Trim();

        var translator = new DeepL();
        var detectedLanguages = await translator.DetectAsync(cleanedInput);
        string detectedLang = detectedLanguages?[0].Language ?? "en";

        var distance = new Levenshtein(cleanedInput);
        int score = distance.DistanceFrom("good morning");
        if (score < 3)
        {
            return await TranslateResponseAsync("Good morning to you too!", detectedLang);
        }
        return await TranslateResponseAsync(
            "Sorry, I didn't understand that greeting, but let’s continue...",
            detectedLang
        );
    }

    private static async Task<string> TranslateResponseAsync(string englishText, string targetLang)
    {
        var translator = new DeepL();
        string translated = await translator.TranslateAsync(englishText, "en", targetLang);
        return translated;
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
