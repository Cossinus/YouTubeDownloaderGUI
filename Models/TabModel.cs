using YouTubeDownloader.Enums;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader.Models;

public class TabModel
{
	public TabModel(TabType tabType)
	{
		TabType = tabType;
		TabName = $"New {TabType} tab";
	}
	
	public string TabName { get; }
	public TabType TabType { get; }
	
	public required AppContainer AppContainer { get; init; }
}