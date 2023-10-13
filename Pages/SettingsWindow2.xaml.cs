using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RaceClient.Models;
using RaceClient.Navigation;
using RaceClient.Services;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace RaceClient.Pages
{
  /// <summary>
  /// Interaction logic for SettingsWindow2.xaml
  /// </summary>
  /// 
  public class SettingsViewModel : AViewModel
  {
    public AppSettings AppSettings { get; set; }
    public SettingsViewModel()
    {
      AppSettings = IniLazySingleTonService.Instance.ReadSettings();
    }
  }

  public partial class SettingsWindow2 : Window
  {
    private readonly SimpleNavigationService navigationService;
    public IConfiguration Config { get; set; }
    public SettingsWindow2(SimpleNavigationService navigationService)
    {
      InitializeComponent();
      this.navigationService = navigationService;
      DataContext = new SettingsViewModel();
    }

    public Task ActivateAsync(object parameter)
    {
      return Task.CompletedTask;
    }

    private async void Previous_Click(object sender, RoutedEventArgs e)
    {
      //todo: save
      var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "appsettings.json");
      var jsonSettings = new JsonSerializerSettings();
      jsonSettings.Converters.Add(new ExpandoObjectConverter());
      jsonSettings.Converters.Add(new StringEnumConverter());
      SettingsViewModel set = (SettingsViewModel)DataContext;
      var newJson = JsonConvert.SerializeObject(set, Formatting.Indented, jsonSettings);
      File.WriteAllText(appSettingsPath, newJson);


      //write also to the orig appsettings
      var index = appSettingsPath.LastIndexOf("bin");
      //var orig = string.Format("{0}appsettings.json",appSettingsPath.Substring(0, index));
      //File.WriteAllText(orig, newJson);

      Hide();
      var result = await navigationService.ShowDialogAsync<MainWindow>(null);
    }

  }
}
