using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HotAvalonia;
using Splat;
using YouTubeDownloader.Utils;
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
			desktop.Exit += OnExit;
			//TODO Load/save state
		}

		base.OnFrameworkInitializationCompleted();
	}
	
	private void OnStartup(object? s, ControlledApplicationLifetimeStartupEventArgs e)
	{
		Locator.Current.GetRequiredService<MainWindowViewModel>().AddTab();
	}
	
	private void OnExit(object? s, ControlledApplicationLifetimeExitEventArgs e)
	{
		var di = new DirectoryInfo(Storage.TempDirectory);

		foreach (var file in di.EnumerateFiles())
		{
			file.Delete();
		}
	}
}