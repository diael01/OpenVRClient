using RaceClient.Models;
using RaceClient.Navigation;
using RaceClient.Pages;
using RaceClient.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RaceClient
{

  public partial class MainWindow : Window, IActivable
  {
    readonly SimpleNavigationService navigationService;
    UserSelection us = new UserSelection();
    DispatcherTimer timer = new DispatcherTimer();
    AutoResetEvent ar = new AutoResetEvent(false);
    public MainWindow(SimpleNavigationService navigationService)
    {
      InitializeComponent();
      this.navigationService = navigationService;
      ListenerSingletonService.Instance.ConnectServer();
      ListenerSingletonService.Instance.SendProcessState();
      //todo: play the video...
      if (IniLazySingleTonService.Instance.GetAutoMode())
      {
        SetTypes();
        int timeout = IniLazySingleTonService.Instance.GetTimeout();
        timer.Interval = TimeSpan.FromSeconds(timeout);
        
        //F:\Projects\OPENVR\raceclient\Assets
        var src = string.Format("{0}\\Backgrounds\\Loading_singleplayer.mp4", IniLazySingleTonService.Instance.GetAssetsLocation());        
        if (File.Exists(src))
        {
          Video.Source = new Uri(src);
          SetVisibility(false);
          //Video.Play();
          ThreadPassElement e = new ThreadPassElement(Video, timer, ar);
          ThreadPool.QueueUserWorkItem(PlayVideo, e);
          ThreadPool.QueueUserWorkItem(StartTimer, e);         
        }
      } else
        SetVisibility(true);
    }

    public struct ThreadPassElement
    {
      public ThreadPassElement(MediaElement m1, DispatcherTimer t1, AutoResetEvent e1)
      {
        m = m1;
        e = e1;
        t = t1;
      }
      public MediaElement m;
      public AutoResetEvent e;
      public DispatcherTimer t;
    }

    public void PlayVideo(Object threadContext)
    {
      this.Dispatcher.Invoke(() =>
      {
        ThreadPassElement tp = (ThreadPassElement)threadContext;
        if (tp.m!=null && tp.m.Source != null)
        {
          tp.m.Play();
          tp.e.Set();
        }
      });     
    }

    public void StartTimer(Object threadContext)
    {
      ThreadPassElement tp = (ThreadPassElement)threadContext;
      if(tp.e != null) {
        tp.e.WaitOne();            
        tp.t.Tick += timer_Tick;
        tp.t.Start();
      }
    }

    public Task ActivateAsync(object parameter)
    {
      us = (UserSelection)parameter;
      if (us == null)
        us = new UserSelection();
      return Task.CompletedTask;
    }

    private async void timer_Tick(object sender, EventArgs e)
    {
      while (true)
      {
        if (us != null)
        {
          if (us.MPConfig == null)
            us.MPConfig = ListenerSingletonService.Instance.Listen(us);
          else
          {
            ListenerSingletonService.Instance.Listen(us);
            if (us.GameStatus == GameStatus.StartGame)
            {
              SetTypes(); //because is a new object; todo: make it same object            
              timer.Stop();
              if (Video.Source != null)
                Video.Stop();
              break;
            }
          }
        } else
          throw new Exception("Fatal: UserSelection null!");
      }
      Hide();
      await navigationService.ShowDialogAsync<AutomatedAssistWindow>(us);
    }

    private void SetVisibility(bool visible)
    {
      if (visible)
      {
        MultiPlayerButton.Visibility = Visibility.Visible;
        SinglePlayerButton.Visibility = Visibility.Visible;
        Settings.Visibility = Visibility.Visible;
        //RaceTimer.Visibility = Visibility.Hidden;
      } else
      {
        MultiPlayerButton.Visibility = Visibility.Hidden;
        SinglePlayerButton.Visibility = Visibility.Hidden;
        Settings.Visibility = Visibility.Hidden;
        //RaceTimer.Visibility = Visibility.Visible;
      }
    }


    private async void SinglePlayer_Click(object sender, RoutedEventArgs e)
    {
      us.PlayerType = PlayerType.SinglePlayer;
      Hide();
      var result = await navigationService.ShowDialogAsync<SinglePlayerWindow>(us);
    }

    private async void MultiPlayer_Click(object sender, RoutedEventArgs e)
    {
      Hide();
      var result = await navigationService.ShowDialogAsync<RaceWindow>(us);
    }

    private async void Settings_Click(object sender, RoutedEventArgs e)
    {
      Hide();
      var result = await navigationService.ShowDialogAsync<SettingsWindow2>(null);
    }

    private void SetTypes()
    {
      us.PlayerType = PlayerType.MultiPlayer;
      us.GameType = GameType.Race;
    }
  }
}
