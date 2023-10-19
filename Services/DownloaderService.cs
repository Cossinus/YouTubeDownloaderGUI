using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using YouTubeDownloader.Interfaces;
using YouTubeDownloader.Utils;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader.Services;

public class DownloaderService : IDownloaderService
{
	private readonly MainWindowViewModel _mainWindowViewModel;
	private readonly HttpClient _httpClient;

	public DownloaderService(MainWindowViewModel mainWindowViewModel, HttpClient httpClient)
	{
		_mainWindowViewModel = mainWindowViewModel;
		_httpClient = httpClient;
	}

	private const string YtDlpDownloadUrl = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";
	private static string YtDlpBinaryName => "yt-dlp.exe";
	
	private const string FfDownloadUrl = "https://evermeet.cx/ffmpeg/getrelease/zip";
	private const string FfmpegBinaryName = "ffmpeg.exe";

	public async Task DownloadBinaries(StackPanel panel, string directoryPath = "")
	{
		if (string.IsNullOrEmpty(directoryPath))
		{
			directoryPath = Directory.GetCurrentDirectory();
		}
		else
		{
			Directory.CreateDirectory(directoryPath);
		}
		
		panel.Height = 30;

		await DownloadYtDlpBinary(panel, directoryPath);
		await DownloadFfBinary(panel, directoryPath);

		panel.Height = 0;
	}

	private async Task DownloadYtDlpBinary(StackPanel panel, string directoryPath)
	{
		if (!File.Exists(Path.Combine(directoryPath, YtDlpBinaryName)))
		{
			using var client = new DownloadWithProgress(YtDlpDownloadUrl, Path.Combine(directoryPath, Path.GetFileName(YtDlpDownloadUrl)), _httpClient);

			_mainWindowViewModel.FileName = YtDlpBinaryName;
			
			client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
			{
				_mainWindowViewModel.FileSize = totalFileSize.Value;
				_mainWindowViewModel.BytesDownloaded = totalBytesDownloaded;
				_mainWindowViewModel.ProgressPercentage = progressPercentage.Value;
			};
			
			await client.StartDownload();
		}
	}

	private async Task DownloadFfBinary(StackPanel panel, string directoryPath)
	{
		if (!File.Exists(Path.Combine(directoryPath, FfmpegBinaryName)))
		{
			var zipName = Path.Combine(directoryPath, Path.GetFileName("ffmpeg.zip"));
		
			using var client = new DownloadWithProgress(FfDownloadUrl, zipName, _httpClient);

			_mainWindowViewModel.FileName = FfmpegBinaryName;

			client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
			{
				_mainWindowViewModel.FileSize = totalFileSize.Value;
				_mainWindowViewModel.BytesDownloaded = totalBytesDownloaded;
				_mainWindowViewModel.ProgressPercentage = progressPercentage.Value;
			};
		
			await client.StartDownload();

			using (var zipArchive = ZipFile.Open(zipName, ZipArchiveMode.Read))
			{
				zipArchive.Entries[0].ExtractToFile(Path.Combine(directoryPath, $"{zipArchive.Entries[0].FullName}.exe"));
			}
		
			File.Delete(zipName);
		}
	}
}