using System;

namespace YouTubeDownloader.Utils;

public class NumberConvert
{
    private static readonly string[] Suffixes = [
        "K",
        "M",
        "B",
    ];
    
    public static string ValueSuffix(long value, int decimalPlaces = 2)
    {
        if (value < 0) return "-" + ValueSuffix(-value, decimalPlaces);
        if (value == 0) return "0";

        if (value < 1000) return value.ToString();
        if (value < 1000000) return $"{Math.Round((decimal)value / 1000, decimalPlaces)}{Suffixes[0]}";
        if (value < 1000000000) return $"{Math.Round((decimal)value / 1000000, decimalPlaces)}{Suffixes[1]}";
        
        return value.ToString();
    }
}