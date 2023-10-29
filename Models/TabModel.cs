using YouTubeDownloader.Enums;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader.Models;

public class TabModel
{
	public TabModel(TabType tabType)
	{
		if (tabType == TabType.YtDlp) AppContainer = new DownloaderViewModel();
		else AppContainer = new EditorViewModel();
		
		TabType = tabType;
		TabName = $"New {TabType} tab";
	}
	
	public string TabName { get; }
	public TabType TabType { get; }
	
	public AppContainer AppContainer { get; init; }
}