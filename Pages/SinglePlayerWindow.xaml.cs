using RaceClient.Models;
using RaceClient.Navigation;
using System.Threading.Tasks;
using System.Windows;

namespace RaceClient.Pages
{
  /// <summary>
  /// Interaction logic for SingleRaceView.xaml
  /// </summary>
  public partial class SinglePlayerWindow : Window, IActivable
  {
    private readonly SimpleNavigationService navigationService;

    UserSelection us;
    public SinglePlayerWindow(SimpleNavigationService navigationService)
    {
      InitializeComponent();
      this.navigationService = navigationService;
    }

    public Task ActivateAsync(object parameter)
    {
      us = (UserSelection)parameter;
      return Task.CompletedTask;
    }

    private async void Totem_Click(object sender, RoutedEventArgs e)
    {
      //will uncomment when they ready
      //Hide();
      //var result = await navigationService.ShowDialogAsync<TotemWindow>(null);
      us.GameType = GameType.Totem;
    }

    private async void FreeRoam_Click(object sender, RoutedEventArgs e)
    {
      us.GameType = GameType.FreeRoam;
    }
    private async void Race_Click(object sender, RoutedEventArgs e)
    {
      us.GameType = GameType.Race;
      Hide();
      var result = await navigationService.ShowDialogAsync<RaceWindow>(us);
    }

    private async void Previous_Click(object sender, RoutedEventArgs e)
    {
      Hide();
      var result = await navigationService.ShowDialogAsync<MainWindow>(us);
    }

    private async void Next_Click(object sender, RoutedEventArgs e)
    {

      Hide();
      var result = await navigationService.ShowDialogAsync<RaceWindow>(us);
    }
  }
}
