using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using YouTubeDownloader.Utils;

namespace YouTubeDownloader.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
	public MainWindowViewModel()
	{
		CloseAppCommand = ReactiveCommand.Create(CloseApp);
		MinimizeAppCommand = ReactiveCommand.Create(MinimizeApp);
	}
	
	public string AppTitle { get; set; } = "YouTube Downloader GUI";
	
	public ReactiveCommand<Unit, Unit> CloseAppCommand { get; }
	public ReactiveCommand<Unit, Unit> MinimizeAppCommand { get; }

	private void CloseApp()
	{
		if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.Shutdown();
		}
	}

	private void MinimizeApp()
	{
		if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow!.WindowState = WindowState.Minimized;
		}
	}

	#region Download Panel Properties

	private string _fileName = "Test";
	public string FileName
	{
		get => _fileName;
		set => this.RaiseAndSetIfChanged(ref _fileName, value);
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
		set
		{
			this.RaiseAndSetIfChanged(ref _bytesDownloaded, value);
			ProgressText = $"{SizeConvert.ConvertSize(value):F2}/{SizeConvert.SizeSuffix(FileSize)} {FileName} ({{1:0}}%)";
		}
	}

	private string _progressText = "{0}/{3}  ({1:0}%)";
	public string ProgressText
	{
		get => _progressText;
		set => this.RaiseAndSetIfChanged(ref _progressText, value);
	}

	#endregion
}