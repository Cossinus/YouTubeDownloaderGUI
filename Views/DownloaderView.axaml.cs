using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader.Views;

public partial class DownloaderView : UserControl
{
	public DownloaderView()
	{
		InitializeComponent();
	}

	private async void SelectTargetDirectory(object? sender, RoutedEventArgs e)
	{
		var storageProvider = TopLevel.GetTopLevel(this)!.StorageProvider;
		var viewModel = (DataContext as DownloaderViewModel)!;
		
		var options = await storageProvider.TryGetFolderFromPathAsync(new Uri(viewModel.TargetDirectory));

		var picker = storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
		{
			SuggestedStartLocation = options
		});

		if (picker.Result.Count > 0)
		{
			viewModel.TargetDirectory = picker.Result[0].Path.LocalPath;
		}
	}
}