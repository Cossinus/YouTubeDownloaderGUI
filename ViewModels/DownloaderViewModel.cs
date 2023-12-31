﻿using System.Reactive;
using ReactiveUI;

namespace YouTubeDownloader.ViewModels;

public class DownloaderViewModel : AppContainer
{
	private string _test = "Test";
	public string Test
	{
		get => _test;
		set => this.RaiseAndSetIfChanged(ref _test, value);
	}
	
	public DownloaderViewModel()
	{
		TestButCommand = ReactiveCommand.Create(TestBut);
	}
	
	public ReactiveCommand<Unit, Unit> TestButCommand { get; }
	
	private void TestBut()
	{
		Test = "Tested";
	}
}