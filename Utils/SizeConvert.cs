using System;

namespace YouTubeDownloader.Utils;

public static class SizeConvert
{
	private static readonly string[] SizeSuffixes = [
		"bytes",
		"KB",
		"MB",
		"GB"
	];

	/// <summary>
	/// Converts provided value to a corresponding decimal value in file size with suffix
	/// </summary>
	/// <param name="value">File size</param>
	/// <param name="decimalPlaces">Decimal places in converted file size</param>
	/// <returns>String with converted value and according suffix</returns>
	public static string SizeSuffix(long value, int decimalPlaces = 2)
	{
		if (value < 0) return "-" + SizeSuffix(-value, decimalPlaces);
		if (value == 0) return "0 bytes";

		var mag = (int)Math.Log(value, 1024);
		var adjustedSize = (decimal)value / (1L << (mag * 10));

		if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
		{
			mag += 1;
			adjustedSize /= 1024;
		}

		return string.Format("{0:n" + decimalPlaces + "}{1}", adjustedSize, SizeSuffixes[mag]);
	}
	
	/// <summary>
	/// Converts provided value to a corresponding decimal value in file size
	/// </summary>
	/// <param name="value">File size</param>
	/// <param name="decimalPlaces">Decimal places in converted file size</param>
	/// <returns>Converted value</returns>
	public static decimal ConvertSize(long value, int decimalPlaces = 2)
	{
		if (value == 0) return value;

		var mag = (int)Math.Log(value, 1024);
		var adjustedSize = (decimal)value / (1L << (mag * 10));

		if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
		{
			adjustedSize /= 1024;
		}

		return adjustedSize;
	}
}