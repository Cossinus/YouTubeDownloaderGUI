using System;
using System.Collections.Generic;
using System.Linq;
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
		services.Register(() => new HttpClient());
		services.RegisterLazySingleton(() => new MainWindowViewModel());
		//services.RegisterLazySingleton<IDownloaderService>(() => new DownloaderService(resolver.GetRequiredService<MainWindowViewModel>(), resolver.GetRequiredService<HttpClient>()));
		services.RegisterLazySingleton<IDownloaderService, DownloaderService>(resolver);
	}

	public static void RegisterLazySingleton<TInterface, TService>(this IMutableDependencyResolver services,
		IReadonlyDependencyResolver resolver)
	{
		var serviceType = typeof(TService);
		var constructors = serviceType.GetConstructors().Single();

		IList<Func<object>> values = new List<Func<object>>();

		foreach (var parameter in constructors.GetParameters())
		{
			var parameterType = parameter.ParameterType;
			values.Add(() => resolver.GetRequiredService(parameterType));
		}
		
		services.RegisterLazySingleton(() => Activator.CreateInstance(serviceType, values.Select(cb => cb()).ToArray()), typeof(TInterface));
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

	public static object GetRequiredService(this IReadonlyDependencyResolver resolver, Type type)
	{
		var service = resolver.GetService(type);
		if (service is null)
		{
			throw new InvalidOperationException($"Failed to resolve object class of type {type}");
		}

		return service;
	}
}