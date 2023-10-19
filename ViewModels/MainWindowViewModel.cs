using ReactiveUI;

namespace YouTubeDownloader.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
	private string _fileName = "Test";
	public string FileName
	{
		set
		{
			this.RaiseAndSetIfChanged(ref _fileName, value);
			ProgressText = $"{{0}}/{{3}} {value} ({{1:0}})";
		}
	}

	private double _progressPercentage;
	public double ProgressPercentage
	{
		get => _progressPercentage;
		set => this.RaiseAndSetIfChanged(ref _progressPercentage, value);
	}

	private long _fileSize;
	public long FileSize
	{
		get => _fileSize;
		set => this.RaiseAndSetIfChanged(ref _fileSize, value);
	}

	private long _bytesDownloaded;
	public long BytesDownloaded
	{
		get => _bytesDownloaded;
		set => this.RaiseAndSetIfChanged(ref _bytesDownloaded, value);
	}

	private string _progressText = "{0}/{3}  ({1:0})";
	public string ProgressText
	{
		get => _progressText;
		set => this.RaiseAndSetIfChanged(ref _progressText, value);
	}
}