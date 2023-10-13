using Force.DeepCloner;
using RaceClient.Models;
using RaceClient.Navigation;
using RaceClient.Views;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RaceClient.Pages
{
  public partial class CarWindow : Window, IActivable
  {
    readonly SimpleNavigationService navigationService;
    UserSelection us;
    CarsView carView;

    public CarWindow(SimpleNavigationService navigationService)
    {
      InitializeComponent();
      this.navigationService = navigationService;
    }

    public Task ActivateAsync(object parameter)
    {
      us = (UserSelection)parameter;
      carView = new CarsView();
      carView.Initialize(us);
      CarsViewTab.Content = carView;
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

    private async void Cars_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us);
      CarsViewTab.Content = carView;
    }

    private async void Singleseater_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us, "Singleseater");
      CarsViewTab.Content = carView;
    }

    private async void GT2_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us, "GT2");
      CarsViewTab.Content = carView;
    }

    private async void GT3_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us, "GT3");
      CarsViewTab.Content = carView;
    }

    private async void GT4_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us, "GT4");
      CarsViewTab.Content = carView;
    }

    private async void Sportscars_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us, "Sportscars");
      CarsViewTab.Content = carView;
    }
    private async void Hypercars_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us, "Hypercars");
      CarsViewTab.Content = carView;
    }

    private async void Street_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us, "Street");
      CarsViewTab.Content = carView;
    }

    private async void Vintage_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us, "Vintage");
      CarsViewTab.Content = carView;
    }

    private async void Drift_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us, "Drift");
      CarsViewTab.Content = carView;
    }

    private async void LMP_Click(object sender, MouseButtonEventArgs e)
    {
      carView = new CarsView();
      carView.Initialize(us, "LMP");
      CarsViewTab.Content = carView;
    }

    private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

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
