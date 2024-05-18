using Avalonia;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Splat;
using YouTubeDownloader.Interfaces;
using YouTubeDownloader.Utils;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
	public MainWindow()
	{
		InitializeComponent();
		
#if DEBUG
		this.AttachDevTools();
#endif
		
		var downloader = Locator.Current.GetRequiredService<IDownloaderService>();
		downloader.InitPanel(DownloadPanel);
		downloader.DownloadBinaries(Storage.BinariesDirectory);
	}

	#region WindowDrag

	private Point _previousMousePosition;
	private bool _leftButtonPressed;
	
	//TODO resize on top panel
	
	private void DragPanel_OnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		_previousMousePosition = e.GetPosition(this);
		_leftButtonPressed = true;
	}
	
	private void DragPanel_OnPointerMoved(object? sender, PointerEventArgs e)
	{
		var point = e.GetPosition(this);

		if (_leftButtonPressed)
		{
			var x = point.X - _previousMousePosition.X;
			var y = point.Y - _previousMousePosition.Y;
			
			Position = new PixelPoint(Position.X + (int)x,  Position.Y + (int)y);
		}
	}

	private void DragPanel_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		_leftButtonPressed = false;
	}

	#endregion
}