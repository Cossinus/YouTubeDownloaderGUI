using System.Text.RegularExpressions;

namespace YouTubeDownloader.Utils;

public static class UrlExtensions
{
    private static readonly Regex UrlRegex;
    private static readonly Regex IdRegex;
    
    static UrlExtensions()
    {
        UrlRegex = new Regex(
            @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        IdRegex = new Regex(
            @"(?:\?v=|\/embed\/|\/\d{2,}\/|\/vi?\/|youtu\.be\/|\/embed\/|\/e\/|watch\?v=|&v=|\/v\/)([a-zA-Z0-9_-]{11})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
    
    public static bool IsValidURL(string url)
    {
        return UrlRegex.IsMatch(url);
    }
    
    public static bool GetVideoId(string url, out string videoId)
    {
        videoId = string.Empty;
        
        var match = IdRegex.Match(url);
        
        if (match.Success)
        {
            videoId = match.Groups[1].Value;
            return true;
        }
        
        return false;
    }
}