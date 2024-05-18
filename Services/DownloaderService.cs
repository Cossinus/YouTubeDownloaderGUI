using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using YouTubeDownloader.Interfaces;
using YouTubeDownloader.Utils;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader.Services;

public class DownloaderService(
	MainWindowViewModel mainWindow,
	HttpClient httpClient)
	: IDownloaderService
{
	private const string YtDlpDownloadUrl = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";
	private const string FfDownloadUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip";

	private StackPanel _downloadPanel = null!;
	
	private readonly string _ytDlpBinaryName = Path.GetFileName(Storage.YtDlpBinaryPath);
	private readonly string _ffmpegBinaryName = Path.GetFileName(Storage.FfmpegBinaryPath);
	
	public void InitPanel(StackPanel panel)
	{
		_downloadPanel = panel;
	}

	public async Task<Bitmap?> DownloadThumbnail(string filePath, string url)
	{
		if (File.Exists(filePath))
		{
			return new Bitmap(filePath);
		}
		
		using var response = await httpClient.GetAsync(url);
		response.EnsureSuccessStatusCode();

		var data = await response.Content.ReadAsByteArrayAsync();
		return new Bitmap(new MemoryStream(data));
	}
	
	public async Task DownloadBinaries(string directoryPath = "")
	{
		if (string.IsNullOrEmpty(directoryPath))
		{
			directoryPath = Storage.BasePath;
		}
		else
		{
			Directory.CreateDirectory(directoryPath);
		}
		
		_downloadPanel.Height = 30;

		await DownloadYtDlpBinary(directoryPath);
		await DownloadFfBinary(directoryPath);

		_downloadPanel.Height = 0;
	}

	private async Task DownloadYtDlpBinary(string directoryPath)
	{
		if (!File.Exists(Storage.YtDlpBinaryPath))
		{
			using var client = new DownloadWithProgress(
				YtDlpDownloadUrl,
				Path.Combine(directoryPath, Path.GetFileName(YtDlpDownloadUrl)),
				httpClient);
			
			mainWindow.FileName = _ytDlpBinaryName;
			
			client.ProgressChanged += (totalFileSize, totalBytesDownloaded) =>
			{
				mainWindow.FileSize = totalFileSize!.Value;
				mainWindow.BytesDownloaded = totalBytesDownloaded;
			};
			
			await client.StartDownload();
		}
	}

	private async Task DownloadFfBinary(string directoryPath)
	{
		if (!File.Exists(Path.Combine(directoryPath, _ffmpegBinaryName)))
		{
			var zipName = Path.Combine(directoryPath, Path.GetFileName("ffmpeg.zip"));
		
			using var client = new DownloadWithProgress(FfDownloadUrl, zipName, httpClient);

			mainWindow.FileName = _ffmpegBinaryName;

			client.ProgressChanged += (totalFileSize, totalBytesDownloaded) =>
			{
				mainWindow.FileSize = totalFileSize!.Value;
				mainWindow.BytesDownloaded = totalBytesDownloaded;
			};
		
			await client.StartDownload();

			using (var zipArchive = ZipFile.Open(zipName, ZipArchiveMode.Read))
			{
				foreach (var entry in zipArchive.Entries)
				{
					if (!entry.Name.Equals(_ffmpegBinaryName)) continue;
					
					entry.ExtractToFile(Path.Combine(directoryPath, entry.Name));
				}
			}
			
			File.Delete(zipName);
		}
	}
}