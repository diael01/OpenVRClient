using Force.DeepCloner;
using RaceClient.Models;
using RaceClient.Navigation;
using RaceClient.Services;
using RaceClient.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RaceClient.Pages
{
  public partial class MultiPlayerCarWindow : Window, IActivable
  {
    readonly SimpleNavigationService navigationService;
    UserSelection us;
    MultiPlayerCarsView carView;
    DispatcherTimer timer = new DispatcherTimer();

    public MultiPlayerCarWindow(SimpleNavigationService navigationService)
    {
      InitializeComponent();
      this.navigationService = navigationService;
      if (IniLazySingleTonService.Instance.GetAutoMode())
      {
        RaceTimer.Visibility = Visibility.Visible;
        int timeout = IniLazySingleTonService.Instance.GetTimeout();
        timer.Interval = TimeSpan.FromSeconds(timeout);
        timer.Tick += timer_Tick;
        timer.Start();
      }
    }

    private async void timer_Tick(object sender, EventArgs e)
    {
      timer.Stop();
      Hide();
      await navigationService.ShowDialogAsync<SummaryWindow>(us);
    }


    public Task ActivateAsync(object parameter)
    {
      us = (UserSelection)parameter;
      carView = new MultiPlayerCarsView();
      carView.Initialize(us);
      MultiPlayerCarsViewTab.Content = carView;
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


    private async void Previous_Click(object sender, RoutedEventArgs e)
    {
      Hide();
      Cars c = (Cars)carView.DataContext;
      us = c.Us.DeepClone();
      await navigationService.ShowDialogAsync<RaceWindow>(us);
    }
  }
}
