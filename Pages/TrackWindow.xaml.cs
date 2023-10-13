using Force.DeepCloner;
using RaceClient.Models;
using RaceClient.Navigation;
using RaceClient.Views;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RaceClient.Pages
{
  public partial class TrackWindow : Window, IActivable
  {
    readonly SimpleNavigationService navigationService;
    TracksView trackView; //= new TracksView();
    UserSelection us;
    public TrackWindow(SimpleNavigationService navigationService)
    {
      InitializeComponent();
      this.navigationService = navigationService;
    }

    public Task ActivateAsync(object parameter)
    {
      us = (UserSelection)parameter;
      trackView = new TracksView();
      trackView.Initialize(us);
      TracksViewTab.Content = trackView;
      return Task.CompletedTask;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      WindowState = WindowState.Minimized;
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }
    private void Tracks_Click(object sender, MouseButtonEventArgs e)
    {
      TracksViewTab.Content = trackView;
    }

    private async void Previous_Click(object sender, RoutedEventArgs e)
    {
      Hide();
      Tracks c = (Tracks)trackView.DataContext;
      us = c.Us.DeepClone();
      await navigationService.ShowDialogAsync<RaceWindow>(us);
    }

  }
}
