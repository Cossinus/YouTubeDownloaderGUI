using ReactiveUI;

namespace YouTubeDownloader.ViewModels;

public class DownloaderViewModel : ViewModelBase
{
	private string _test = "Test";
	public string Test
	{
		get => _test;
		set => this.RaiseAndSetIfChanged(ref _test, value);
	}
	
	public DownloaderViewModel()
	{
		
	}
}