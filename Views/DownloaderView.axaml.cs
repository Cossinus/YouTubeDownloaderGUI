using Avalonia.Controls;
using Avalonia.Interactivity;
using Splat;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader.Views;

public partial class DownloaderView : UserControl
{
	public DownloaderView()
	{
		InitializeComponent();
	}

	private void Button_OnClick(object? sender, RoutedEventArgs e)
	{
		Locator.Current.GetRequiredService<DownloaderViewModel>().Test = "Tested";
	}
}