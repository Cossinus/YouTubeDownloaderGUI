using System;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData.Binding;
using ReactiveUI;
using Splat;
using YouTubeDownloader.Enums;
using YouTubeDownloader.Interfaces;
using YouTubeDownloader.Models;
using YouTubeDownloader.Utils;
using YouTubeDownloader.Views;

namespace YouTubeDownloader.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public string AppTitle { get; set; } = "YouTube Downloader GUI";

	private IObservableCollection<TabModel> Tabs { get; }

	private AppContainer _currentAppContainer = null!;
	public AppContainer CurrentAppContainer
	{
		get => _currentAppContainer;
		set => this.RaiseAndSetIfChanged(ref _currentAppContainer, value);
	}

	private int _currentSelectedTabIndex;
	private Control _previousSelectedButton = null!;
	private TabType _selectedBinary;
	
	public MainWindowViewModel()
	{
		CloseAppCommand = ReactiveCommand.Create(CloseApp);
		MinimizeAppCommand = ReactiveCommand.Create(MinimizeApp);
		AddTabCommand = ReactiveCommand.Create(AddTab);
		SwitchTabLeftCommand = ReactiveCommand.Create(SwitchTabLeft);
		SwitchTabRightCommand = ReactiveCommand.Create(SwitchTabRight);
		SelectBinaryCommand = ReactiveCommand.Create<TabType>(SelectBinary);
		SelectTabCommand = ReactiveCommand.Create<int>(SelectTab);
		CloseTabCommand = ReactiveCommand.Create<int>(CloseTab);
		
		Tabs = new ObservableCollectionExtended<TabModel>();
	}
	
	public ReactiveCommand<Unit, Unit> CloseAppCommand { get; }
	public ReactiveCommand<Unit, Unit> MinimizeAppCommand { get; }
	public ReactiveCommand<int, Unit> SelectTabCommand { get; }
	public ReactiveCommand<int, Unit> CloseTabCommand { get; }
	public ReactiveCommand<Unit, Unit> AddTabCommand { get; }
	public ReactiveCommand<Unit, Unit> SwitchTabLeftCommand { get; }
	public ReactiveCommand<Unit, Unit> SwitchTabRightCommand { get; }
	public ReactiveCommand<TabType, Unit> SelectBinaryCommand { get; }

	private void CloseApp()
	{
		if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.Shutdown();
		}
	}

	private void MinimizeApp()
	{
		if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow!.WindowState = WindowState.Minimized;
		}
	}

	#region TabCommands

	public void AddTab()
	{
		var newTab = new TabModel(_selectedBinary)
		{
			AppContainer = new DownloaderViewModel()
		};

		if (_selectedBinary == TabType.FfMpeg)
		{
			newTab = new TabModel(_selectedBinary)
			{
				AppContainer = new EditorViewModel()
			};
		}
		
		var tabButton = new Button
		{
			Classes = { "Tab" }, 
			Command = SelectTabCommand,
			CommandParameter = Tabs.Count
		};
		
		var buttonLabel = new Label
		{
			Content = newTab.TabName
		};

		var closeImage = new Image
		{
			Source = new Bitmap(AssetLoader.Open(new Uri("avares://YoutubeDownloader/Assets/Close.png")))
		};
		
		var closeTabButton = new Button
		{
			Classes = { "CloseTab" },
			Content = closeImage,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Right,
			Margin = new Thickness(2,2,7,2),
			Padding = new Thickness(6,6,6,6),
			Background = Brushes.Transparent,
			Command = CloseTabCommand,
			CommandParameter = Tabs.Count
		};
		
		tabButton.Content = new Panel
		{
			Children =
			{
				buttonLabel,
				closeTabButton
			}
		};
		
		if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			var mainWindow = desktop.MainWindow as MainWindow;
			var tabsPanel = mainWindow!.TabsParent;
			
			tabsPanel.Children.Insert(tabsPanel.Children.Count - 1, tabButton);
		}
		
		Tabs.Add(newTab);

		if (Tabs.Count == 1)
		{
			tabButton.Classes.Add("SelectedTab");
			_previousSelectedButton = tabButton;
			SelectTab(0);
		}
	}

	private void CloseTab(int tabIndex)
	{
		//TODO instead ask user if he is sure that he wants to close his last tab
		if (Tabs.Count == 1) return;
		
		if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			var mainWindow = desktop.MainWindow as MainWindow;
			var tabsParent = mainWindow!.TabsParent;
			var currButton = tabsParent.Children[tabIndex];
			
			if (_previousSelectedButton == currButton)
			{
				var childIndex = tabIndex == Tabs.Count - 1 ? tabIndex - 1 : tabIndex + 1;
				
				SelectTab(childIndex);
			}
			
			Tabs.RemoveAt(tabIndex);
			tabsParent.Children.RemoveAt(tabIndex);

			for (var i = 0; i < Tabs.Count; i++)
			{
				var tabButton = tabsParent.Children[i] as Button;
				tabButton!.CommandParameter = i;

				var panel = tabButton.Content as Panel;
				var closeButton = panel!.Children.Last() as Button;
				closeButton!.CommandParameter = i;
			}
		}
	}

	private void SwitchTabLeft()
	{
		if (_currentSelectedTabIndex == 0) return;
		
		SelectTab(--_currentSelectedTabIndex);
	}

	private void SwitchTabRight()
	{
		if (_currentSelectedTabIndex == Tabs.Count - 1) return;
		
		SelectTab(++_currentSelectedTabIndex);
	}

	private void SelectTab(int tabIndex)
	{
		CurrentAppContainer = Tabs[tabIndex].AppContainer;
		_currentSelectedTabIndex = tabIndex;
		
		if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			var mainWindow = desktop.MainWindow as MainWindow;
			var currButton = mainWindow!.TabsParent.Children[tabIndex];

			if (_previousSelectedButton == currButton) return;
			
			_previousSelectedButton.Classes.Remove("SelectedTab");
			currButton.Classes.Add("SelectedTab");
			_previousSelectedButton = currButton;
		}
	}

	private void SelectBinary(TabType tabType)
	{
		if (_selectedBinary == tabType)
		{
			AddTab();
			return;
		}
		
		_selectedBinary = tabType;

		if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			var mainWindow = desktop.MainWindow as MainWindow;

			mainWindow!.SelectedYt.Height = 0;
			mainWindow.SelectedFf.Height = 0;
			if (tabType == TabType.YtDlp) mainWindow.SelectedYt.Height = 30;
			else mainWindow.SelectedFf.Height = 30;
		}
		
		AddTab();
	}

	#endregion

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