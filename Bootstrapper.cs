using System;
using System.Net.Http;
using Splat;
using YouTubeDownloader.Interfaces;
using YouTubeDownloader.Services;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader;

public static class Bootstrapper
{
	public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
	{
		services.RegisterLazySingleton(() => new MainWindowViewModel());
		services.RegisterLazySingleton(() => new HttpClient());
		services.RegisterLazySingleton<IDownloaderService>(() => new DownloaderService(resolver.GetRequiredService<MainWindowViewModel>(), resolver.GetRequiredService<HttpClient>()));
	}

	public static TService GetRequiredService<TService>(this IReadonlyDependencyResolver resolver)
	{
		var service = resolver.GetService<TService>();
		if (service is null)
		{
			throw new InvalidOperationException($"Failed to resolve object class of type {typeof(TService)}");
		}

		return service;
	}
}