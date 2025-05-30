using Fastenshtein;
using LibreTranslate.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public static class GreetingHandler
{

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
