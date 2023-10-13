using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaceClient.Models;
using RaceClient.Navigation;
using RaceClient.Pages;
using RaceClient.Services;
using System;
using System.IO;
using System.Windows;

namespace RaceClient
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public IServiceProvider ServiceProvider { get; private set; }

    public IConfiguration Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      var builder = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

      Configuration = builder.Build();

      // Create a service collection and configure our dependencies
      var serviceCollection = new ServiceCollection();
      ConfigureServices(serviceCollection);

      // Build the our IServiceProvider and set our reference to it
      ServiceProvider = serviceCollection.BuildServiceProvider();

      var navigationService = ServiceProvider.GetRequiredService<SimpleNavigationService>();
      var task = navigationService.ShowAsync<MainWindow>();
    }

    private void ConfigureServices(IServiceCollection services)
    {
      services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
      services.AddSingleton<IConfiguration>(Configuration);
      CarLazySingleTonService.Instance.Config = Configuration;
      TrackLazySingleTonService.Instance.Config = Configuration;
      IniLazySingleTonService.Instance.Config = Configuration;
      ListenerSingletonService.Instance.Config = Configuration;


      // Add SimpleNavigationService for the application.
      services.AddScoped<SimpleNavigationService>();

      // Register all the Windows of the applications.
      services.AddTransient<MainWindow>();
      services.AddTransient<CarWindow>();
      services.AddTransient<SinglePlayerWindow>();
      services.AddTransient<MultiPlayerCarWindow>();
      services.AddTransient<RaceWindow>();
      services.AddTransient<AutomatedAssistWindow>();
      services.AddTransient<SummaryWindow>();
      services.AddTransient<TrackWindow>();
      services.AddTransient<SettingsWindow2>();

    }
  }
}
