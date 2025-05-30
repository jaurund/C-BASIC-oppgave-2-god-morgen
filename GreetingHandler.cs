using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DeepL;
using Fastenshtein;

public static class GreetingHandler
{
    private static readonly string apiKey = "4a3aafff-6114-4528-92bc-a5b25a357357:fx";
    private static readonly Translator translator = new Translator(apiKey);

    public static async Task<string> HandleGreetingAsync(string userInput)
    {
        string cleanedInput = Regex.Replace(userInput.ToLowerInvariant(), @"[^\w\s]", "").Trim();

        string detectedLang = DetectLangSimple(cleanedInput);

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

    private static string DetectLangSimple(string text)
    {
        // Simple keyword-based guessing (can improve later)
        if (text.Contains("bonjour"))
            return "FR";
        if (text.Contains("guten"))
            return "DE";
        if (text.Contains("buenos"))
            return "ES";
        if (text.Contains("god"))
            return "NB"; // Norwegian Bokmål
        return "EN";
    }

    private static async Task<string> TranslateResponseAsync(string englishText, string targetLang)
    {
        if (targetLang.ToUpper() == "EN")
            return englishText;
        var result = await translator.TranslateTextAsync(englishText, "EN", targetLang);
        return result.Text;
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
