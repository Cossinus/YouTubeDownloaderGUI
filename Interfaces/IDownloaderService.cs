using System.Threading.Tasks;
using Avalonia.Controls;

namespace YouTubeDownloader.Interfaces;

public interface IDownloaderService
{
	Task DownloadBinaries(StackPanel panel, string directoryPath = "");
}