using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace AdventOfCode;

public static class InputHelper
{
    public static string[] GetInput(int day)
    {
        return PersistInput(day, false, GetResponse("https://adventofcode.com/2022/day/{0}/input", day)).Split("\n");
    }
    public static string[] GetSampleInput(int day)
    {
        return PersistInput(day, true, GetCodeSnippetFromHtml(GetResponse("https://adventofcode.com/2022/day/{0}", day))).Split("\n");
    }
    private static string GetResponse(string urlWithPlaceholder, int day)
    {
        var cacheResponse = GetCachedResponse(day, !urlWithPlaceholder.EndsWith("/input"));
        if (!String.IsNullOrEmpty(cacheResponse))
            return cacheResponse;
        return GetResponse(String.Format(urlWithPlaceholder, day));
    }
    private static string GetResponse(string url)
    {
        var cookie = new Cookie();
        cookie.Name = "session";
        cookie.Domain = "adventofcode.com";
        cookie.Value = Environment.GetEnvironmentVariable("COOKIE");
        var clientHandler = new HttpClientHandler();
        clientHandler.CookieContainer = new CookieContainer();
        clientHandler.CookieContainer.Add(cookie);

        var client = new HttpClient(clientHandler);
        var response = client.GetStringAsync(url);
        return response.Result;
    }
    private static string GetCodeSnippetFromHtml(string response)
    {
        if (!response.Contains("<html")) // if it's a cached response this will only contain the actual values
            return response;
        var htmlSnippet = new HtmlDocument();
        htmlSnippet.LoadHtml(response);
        var node = htmlSnippet.DocumentNode.SelectSingleNode("/html[1]/body[1]/main[1]/article[1]/pre[1]/code[1]");
        return node.InnerText;
    }
    private static string GetCachedResponse(int day, bool sampleInput)
    {
        var directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        var sourceFilePath = GetCacheFilename(day, sampleInput);
        if (!File.Exists(sourceFilePath))
            return String.Empty;
        return File.ReadAllText(sourceFilePath).Replace("\r\n", "\n");
    }
    private static string PersistInput(int day, bool sampleInput, string content)
    {
        var directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        using (var writer = new StreamWriter(GetCacheFilename(day, sampleInput), false, Encoding.Default))
        {
            writer.Write(content);
        }
        return content;
    }
    private static string GetCacheFilename(int day, bool sampleInput)
    {
        var directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        var cacheFilepath = Path.Combine(directoryInfo.Parent.Parent.Parent.ToString(), "Inputs"); // starting at: "<...>>\\AoC\\2022\\AdventOfCode\\bin\\Debug\\net6.0\\"
        if (sampleInput)
            cacheFilepath = Path.Combine(cacheFilepath, String.Format("{0:D2}-sample.txt", day));
        else
            cacheFilepath = Path.Combine(cacheFilepath, String.Format("{0:D2}.txt", day));
        return cacheFilepath;
    }
}

public static class BaseDayExtension
{
    public static string[] GetInput(this BaseDay baseDay, bool sampleInput)
    {
        if (sampleInput)
            return GetSampleInput(baseDay);
        return GetInput(baseDay);
    }
    public static string[] GetInput(this BaseDay baseDay)
    {
        return InputHelper.GetInput(int.Parse(baseDay.GetType().Name.Replace("Day_", ""))); // get the currently used day based on the class name
    }
    public static string[] GetSampleInput(this BaseDay baseDay)
    {
        return InputHelper.GetSampleInput(int.Parse(baseDay.GetType().Name.Replace("Day_", ""))); // get the currently used day based on the class name
    }
}
public static class CharExtensions
{
    public static int PriorityValue(this char c)
    {
        int charValue = 0;
        if (c == 0)
            return charValue;
        // Lowercase item types a through z have priorities 1 through 26.
        // Uppercase item types A through Z have priorities 27 through 52.
        if (char.IsLower(c))
            charValue = c - 96;
        else
            charValue = c - 38;

        return charValue;
    }
}