﻿using System.Net.Http;
using Boilerplate.Client.Windows.Services;

namespace Boilerplate.Client.Windows;

public static partial class Program
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in windows project only.

        ConfigurationBuilder configurationBuilder = new();
        configurationBuilder.AddClientConfigurations();
        var configuration = configurationBuilder.Build();
        services.TryAddTransient<IConfiguration>(sp => configuration);

        Uri.TryCreate(configuration.GetApiServerAddress(), UriKind.Absolute, out var apiServerAddress);
        services.TryAddTransient(sp =>
        {
            var handler = sp.GetRequiredKeyedService<HttpMessageHandler>("DefaultMessageHandler");
            HttpClient httpClient = new(handler)
            {
                BaseAddress = apiServerAddress
            };
            return httpClient;
        });

        services.AddWpfBlazorWebView();
        if (BuildConfiguration.IsDebug())
        {
            services.AddBlazorWebViewDeveloperTools();
        }

        services.TryAddTransient<IStorageService, WindowsStorageService>();
        services.TryAddTransient<IBitDeviceCoordinator, WindowsDeviceCoordinator>();
        services.TryAddTransient<IExceptionHandler, WindowsExceptionHandler>();

        services.AddClientCoreProjectServices();
    }
}
