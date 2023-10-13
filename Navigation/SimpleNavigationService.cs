using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace RaceClient.Navigation
{
  public class SimpleNavigationService
  {
    private readonly IServiceProvider serviceProvider;

    public SimpleNavigationService(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public async Task ShowAsync<T>(object parameter = null) where T : Window
    {
      var window = serviceProvider.GetRequiredService<T>();
      if (window is IActivable activableWindow)
      {
        await activableWindow.ActivateAsync(parameter);
      }

      window.Show();
    }

    public async Task<bool?> ShowDialogAsync<T>(object parameter = null) where T : Window
    {
      var window = serviceProvider.GetRequiredService<T>();
      if (window is IActivable activableWindow)
      {
        await activableWindow.ActivateAsync(parameter);
      }

      return window.ShowDialog();
    }
  }
}
