using System.Reactive;
using ReactiveUI;

namespace YouTubeDownloader.ViewModels;

public class EditorViewModel : AppContainer
{
	private string _test = "Test";
	public string Test
	{
		get => _test;
		set => this.RaiseAndSetIfChanged(ref _test, value);
	}
	
	public EditorViewModel()
	{
		TestButCommand = ReactiveCommand.Create(TestBut);
	}
	
	public ReactiveCommand<Unit, Unit> TestButCommand { get; }
	
	private void TestBut()
	{
		Test = "Tested2";
	}
}