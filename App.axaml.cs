using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HotAvalonia;
using Splat;
using YouTubeDownloader.ViewModels;
using YouTubeDownloader.Views;

namespace YouTubeDownloader;

public partial class App : Application
{
	public override void Initialize()
	{
		this.EnableHotReload();
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

			desktop.Startup += OnStartup;
			//TODO Load/save state
			//TODO on exit delete everything from temp
		}

		base.OnFrameworkInitializationCompleted();
	}
	
	private void OnStartup(object? s, ControlledApplicationLifetimeStartupEventArgs e)
	{
		Locator.Current.GetRequiredService<MainWindowViewModel>().AddTab();
	}
}