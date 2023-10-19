using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Splat;
using YouTubeDownloader.ViewModels;
using YouTubeDownloader.Views;

namespace YouTubeDownloader;

public partial class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow = new MainWindow
			{
				DataContext = Locator.Current.GetRequiredService<MainWindowViewModel>(),
			};
		}

		base.OnFrameworkInitializationCompleted();
	}
}