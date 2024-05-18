using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace YouTubeDownloader.Interfaces;

public interface IDownloaderService
{
	void InitPanel(StackPanel panel);
	
	Task<Bitmap?> DownloadThumbnail(string filePath, string url);
	Task DownloadBinaries(string directoryPath = "");
}