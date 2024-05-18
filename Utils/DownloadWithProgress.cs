using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace YouTubeDownloader.Utils;

public class DownloadWithProgress(
	string downloadUrl,
	string destinationPath,
	HttpClient httpClient)
	: IDisposable
{
	public delegate void ProgressChangedHandler(long? totalSize, long totalBytesDownloaded);
	public event ProgressChangedHandler ProgressChanged = null!;

	public async Task StartDownload()
	{
		using var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
		response.EnsureSuccessStatusCode();
			
		await using var contentStream = await response.Content.ReadAsStreamAsync();
			
		var totalSize = response.Content.Headers.ContentLength;
		var totalBytesRead = 0L;
		var buffer = new byte[8192];
		var isMoreToRead = true;
			
		await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

		do
		{
			var bytesRead = await contentStream.ReadAsync(buffer);
			if (bytesRead == 0)
			{
				isMoreToRead = false;
				ProgressChanged(totalSize, totalBytesRead);
				continue;
			}

			await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));

			totalBytesRead += bytesRead;

			ProgressChanged(totalSize, totalBytesRead);

		} while (isMoreToRead);
	}
	
	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}