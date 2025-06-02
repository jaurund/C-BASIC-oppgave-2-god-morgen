// included this here to show workflow on project,
// and how it looked like before I decided to handle
// user input through NuGets instead.

```csharp

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

/* included other class to show workflow for trying to implement NuGets. This version of HandleGreetingAsync tries to handle "good morning!" as a reference in Levenshtein, after it is translated from dummy text with DeepL, but then crashes when it tries to use the dummy translate as the source language instead of userinput.

In short, the entire class is very counter-productive against itself.*/

 public static async Task<string> HandleGreetingAsync(string userInput)
    {
        string cleanedInput = Regex.Replace(userInput.ToLowerInvariant(), @"[^\w\s]", "").Trim();

        // Check similarity to "good morning" using Levenshtein
        var distance = new Levenshtein(cleanedInput);
        int score = distance.DistanceFrom("good morning");

        // Use translation with auto-detect to find the original language
        var dummyTranslation = await translator.TranslateTextAsync("Hello", null, "en-GB"); // Dummy translation to detect language
        string detectedLang = dummyTranslation.DetectedSourceLanguageCode.ToUpper();

        if (score < 3)
        {
            return await TranslateResponseAsync("Good morning to you too!", detectedLang);
        }

        return await TranslateResponseAsync(
            "Sorry, I didn't understand that greeting, but let’s continue...",
            detectedLang
        );
    }

```

/\* included other class to show workflow for trying to implement NuGets. This version crashes on trying to handle "good morning!" as a reference in Levenshtein, after it is translated from dummy text with DeepL, but then crashes when it tries to use the dummy translate as the source language instead of userinput.

In short, the entire class is very counter-productive against itself. \*/
