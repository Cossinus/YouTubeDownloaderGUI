using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Splat;
using YouTubeDownloader.Interfaces;
using YouTubeDownloader.Utils;

namespace YouTubeDownloader.ViewModels;

public class DownloaderViewModel : AppContainer
{
	private readonly IDownloaderService _downloader;
	
	private string ThumbnailLink(string videoId) => $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg";
	
	private string _platform = string.Empty;

	private const string PrintVideoDataTemplate = "-U --print \"%(extractor)s\n%(title)s\n%(availability)s\n%(channel)s\n%(channel_follower_count)s\n%(upload_date)s\n%(view_count)s\n%(like_count)s\n%(dislike_count)s\n%(comment_count)s\n%(playlist_title)s\n%(playlist_count)s\n%(playlist_index)s\n%(playlist_uploader)s\n%(duration_string)s\n%(filesize)s\n%(filesize_approx)s\n%(description)s\" {0}";
	private readonly string _ytDlpPath = @$"{Directory.GetCurrentDirectory()}\binaries\yt-dlp.exe";

	#region Properties

	private string _ytLink = null!;
	public string YtLink
	{
		get => _ytLink;
		set => this.RaiseAndSetIfChanged(ref _ytLink, value);
	}

	private string _targetDirectory = @$"{Directory.GetCurrentDirectory()}\Downloads\";
	public string TargetDirectory
	{
		get => _targetDirectory;
		set => this.RaiseAndSetIfChanged(ref _targetDirectory, value);
	}

	private string _title = "Title";
	public string Title
	{
		get => _title;
		set => this.RaiseAndSetIfChanged(ref _title, value);
	}
	
	private string _availability = string.Empty;
	public string Availability
	{
		get => _availability;
		set => this.RaiseAndSetIfChanged(ref _availability, value);
	}
	
	private string _channelName = "Channel Name";
	public string ChannelName
	{
		get => _channelName;
		set => this.RaiseAndSetIfChanged(ref _channelName, value);
	}
	
	private string _channelFollowerCount = "Subscribers";
	public string ChannelFollowerCount
	{
		get => _channelFollowerCount;
		set => this.RaiseAndSetIfChanged(ref _channelFollowerCount, value);
	}
	
	private string _uploadDate = "Upload Date";
	public string UploadDate
	{
		get => _uploadDate;
		set => this.RaiseAndSetIfChanged(ref _uploadDate, value);
	}
	
	private string _viewCount = "Views";
	public string ViewCount
	{
		get => _viewCount;
		set => this.RaiseAndSetIfChanged(ref _viewCount, value);
	}
	
	private string _likeCount = "Likes";
	public string LikeCount
	{
		get => _likeCount;
		set => this.RaiseAndSetIfChanged(ref _likeCount, value);
	}
	
	private string _duration = "Comments";
	public string Duration
	{
		get => _duration;
		set => this.RaiseAndSetIfChanged(ref _duration, value);
	}
	
	private string _commentCount = "Comments";
	public string CommentCount
	{
		get => _commentCount;
		set => this.RaiseAndSetIfChanged(ref _commentCount, value);
	}
	
	private string _description = "Description";
	public string Description
	{
		get => _description;
		set => this.RaiseAndSetIfChanged(ref _description, value);
	}
	
	private string _fileSize = "File Size";
	public string FileSize
	{
		get => _fileSize;
		set => this.RaiseAndSetIfChanged(ref _fileSize, value);
	}
	
	private Bitmap? _thumbnail = null;
	public Bitmap? Thumbnail
	{
		get => _thumbnail;
		set => this.RaiseAndSetIfChanged(ref _thumbnail, value);
	}

	#endregion
	
	public DownloaderViewModel()
	{
		_downloader = Locator.Current.GetRequiredService<IDownloaderService>();
		FetchDataCommand = ReactiveCommand.CreateFromObservable(FetchData);
	}

	public ReactiveCommand<Unit, Unit> FetchDataCommand { get; }

	private IObservable<Unit> FetchData()
	{
		return Observable.StartAsync(async () =>
		{
			if (!UrlExtensions.IsValidURL(_ytLink)) return;
			
			var p = new Process();
			p.StartInfo = new ProcessStartInfo(_ytDlpPath)
				{
					Arguments = string.Format(PrintVideoDataTemplate, _ytLink),
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardInput = true
				};
			p.OutputDataReceived += ReadData;
			p.Start();
			//p.BeginOutputReadLine();
			
			if (!UrlExtensions.GetVideoId(_ytLink, out var videoId)) return;

			var filePath = Path.Combine(Storage.TempDirectory, $"{videoId}.jpg");
			Thumbnail = await _downloader.DownloadThumbnail(filePath, ThumbnailLink(videoId));
			
			var outputLines = (await p.StandardOutput.ReadToEndAsync()).Split('\n');
			
			if (string.IsNullOrEmpty(outputLines[0])) return;
			
			_platform = outputLines[0];
			Title = outputLines[1];
			
			Availability = outputLines[2];
			ChannelName = outputLines[3];
			
			ChannelFollowerCount = $"{outputLines[4]} Subscriber{(outputLines[4] == "1" ? "" : "s")}"; //TODO Convert to k/m
			
			UploadDate = DateTime.ParseExact(outputLines[5], "yyyyMMdd", null).ToString("yyyy-MM-dd");
			ViewCount = $"{outputLines[6]} View{(outputLines[6] == "1" ? "" : "s")}";
			LikeCount = $"{outputLines[7]}\ud83d\udc4d";
			CommentCount = $"{outputLines[9]} Comment{(outputLines[9] == "1" ? "" : "s")}";
			Duration = outputLines[10];
			
			FileSize = outputLines[14];
			if (outputLines[15] == "NA")
			{
				FileSize = outputLines[16];
			}
			
			Description = string.Join("\n", outputLines[17..]);
		});
	}

	private void ReadData(object sender, DataReceivedEventArgs e)
	{
		if (!string.IsNullOrEmpty(e.Data))
		{
			
		}
	}
}