using RaceClient.DataFile;
using RaceClient.Models;
using RaceClient.Navigation;
using RaceClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;


namespace RaceClient.Pages
{
  /// <summary>
  /// Interaction logic for SummaryWindow.xaml
  /// </summary>
  public partial class SummaryWindow : Window, IActivable
  {
    private readonly SimpleNavigationService navigationService;
  
    UserSelection us;
    DispatcherTimer timer = new DispatcherTimer();

    public SummaryWindow(SimpleNavigationService navigationService)
    {
      InitializeComponent();
      this.navigationService = navigationService;
      
      RaceTimer.Visibility = Visibility.Visible;
      RaceTimer.Timeout = 20;
      int timeout = IniLazySingleTonService.Instance.GetTimeout();
      //timer.Interval = TimeSpan.FromSeconds(timeout);
      //timer.Tick += timer_Tick;
      //timer.Start();
    }

    private async void timer_Tick(object sender, EventArgs e)
    {
     timer.Stop();   
      if (Utils.StartACS())
      {
        //process finished, wait 
        us.GameStatus = GameStatus.EndGame;
        Thread.Sleep(5);
        Hide();
        await navigationService.ShowDialogAsync<MainWindow>(us);
      }
    }

    public Task ActivateAsync(object parameter)
    {
      us = (UserSelection)parameter;
      if (us != null)
      {
        ImagesPath paths = new ImagesPath();
        var a = new AllAssists();
          //set the carName and TrackName from MPCOnfig
          if (us.MPConfig != null)
          {
            //set the one and only track; really?
            //TrackButton.Content = string.Format("{0}", us.MPConfig.TrackName);
            
            var TrackData = TrackLazySingleTonService.Instance.SetTrack(us.MPConfig.TrackName, us.MPConfig.TrackVariation).FirstOrDefault();
            TrackName.Text = TrackData.Track.name;//us.MPConfig.TrackName;
            us.TrackImageDir = TrackData.ImageDir;
            us.VariationImageDir = TrackData.VariationImageDir;
            paths.TrackImage = us.TrackImageDir;
            paths.VariationImage = us.VariationImageDir;
        }

        if (!string.IsNullOrEmpty(us.CarName))
        {
          //if not set is an image, if is set => it will be the car name until there is dynamic image
          //CarButton.Content = string.Format("{0}/{1}", us.CarName, us.CarSkin);
          paths.CarImage = us.CarImageDir;
        }
                   
        switch (us.AssistHelp)
        {
          case AssistHelp.Gamer:          
            a.Initialize("gamer.ini",paths);
            break;
          case AssistHelp.Pro:           
            a.Initialize("pro.ini",  paths);
            break;
          case AssistHelp.SemiPro:           
            a.Initialize("tracks.ini", paths);
            break;
          case AssistHelp.Racer:           
            a.Initialize("racer.ini", paths);
            break;
        }
        DataContext = a;
      }
      return Task.CompletedTask;
    }

  
    private async void Previous_Click(object sender, RoutedEventArgs e)
    {
      if (us != null)        
          await navigationService.ShowDialogAsync<MultiPlayerCarWindow>(us);
      
    }
  }
}
