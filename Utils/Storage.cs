using System.IO;
using System.Reflection;

namespace YouTubeDownloader.Utils;

public static class Storage
{
    public static readonly string BasePath = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
    
    public static readonly string BinariesDirectory = Path.Combine(BasePath, "binaries");
    public static readonly string YtDlpBinaryPath = Path.Combine(BinariesDirectory, "yt-dlp.exe");
    public static readonly string FfmpegBinaryPath = Path.Combine(BinariesDirectory, "ffmpeg.exe");
    
    public static readonly string TempDirectory = Path.Combine(BasePath, "temp");
    
    static Storage()
    {
        Directory.CreateDirectory(BinariesDirectory);
        Directory.CreateDirectory(TempDirectory);
    }
}