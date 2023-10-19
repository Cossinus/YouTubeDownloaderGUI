using Newtonsoft.Json;

namespace YouTubeDownloader.Utils;

public abstract class FfMpegApi
{
	public class Root
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("permalink")]
        public string Permalink { get; set; }
        [JsonProperty("bin")]
        public BinVersion Bin { get; set; }
    }
    
    [JsonObject("bin")]
    public class BinVersion
    {
        [JsonProperty("windows-64")]
        public Win64 Win64 { get; set; }
    }

    [JsonObject("windows_64")]
    public class Win64
    {
        public string ffmpeg { get; set; }
        public string ffprobe { get; set; }
    }
}