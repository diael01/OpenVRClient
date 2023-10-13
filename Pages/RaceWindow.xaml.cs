using RaceClient.DataFile;
using RaceClient.Models;
using RaceClient.Navigation;
using RaceClient.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RaceClient.Pages
{


  public partial class RaceWindow : Window, IActivable
  {
    private readonly SimpleNavigationService navigationService;
    private IniFile assistSaveFile;
    UserSelection us;
    DispatcherTimer timer = new DispatcherTimer();
    bool carClick = false;

    public RaceWindow(SimpleNavigationService navigationService)
    {
      InitializeComponent();
      this.navigationService = navigationService;
      RaceButtonStatus();

      var a = new AllAssists();
      a.Initialize("gamer.ini");
      assistSaveFile = new IniFile(IniLazySingleTonService.Instance.ReadIniFileName("assists.ini"));
      DataContext = a;

      if (IniLazySingleTonService.Instance.GetAutoMode())
      {
        //Make track button readonly
        TrackButton.IsEnabled = false; //will update the image
        RaceTimer.Visibility = Visibility.Visible;
        int timeout = IniLazySingleTonService.Instance.GetTimeout();
        timer.Interval = TimeSpan.FromSeconds(timeout);
        timer.Tick += timer_Tick;
        timer.Start();
      } else
        TrackButton.IsEnabled = true;
    }

    private async void timer_Tick(object sender, EventArgs e)
    {
      RaceButtonStatus();
      if (carClick == false && us != null && string.IsNullOrEmpty(us.CarName))
      {
        Hide();
        await navigationService.ShowDialogAsync<MultiPlayerCarWindow>(us);
      } else if (carClick == false && us != null && !string.IsNullOrEmpty(us.CarName) && !string.IsNullOrEmpty(us.Track)) //car has been selected now waiting to start race
      {
        timer.Stop();
        Utils.StartACS();
      }
    }

    public Task ActivateAsync(object parameter)
    {
      us = (UserSelection)parameter;
      if (us != null)
      {
        if (IniLazySingleTonService.Instance.GetAutoMode())
        {
          //set the carName and TrackName from MPCOnfig
          if (us.MPConfig != null)
          {
            //set the one and only track; really?
            TrackButton.Content = string.Format("{0}", us.MPConfig.TrackName);
            us.Track = us.MPConfig.TrackName;
            //do not set the car 'coz is a list
          }
        } else
        {
          if (!string.IsNullOrEmpty(us.Track))
            TrackButton.Content = (string.IsNullOrEmpty(us.TrackVariation) ? us.Track : string.Format("{0}/{1}", us.Track, us.TrackVariation));
        }
        if (!string.IsNullOrEmpty(us.CarName))  //if not set is an image, if is set => it will be the car name until there is dynamic image
          CarButton.Content = string.Format("{0}/{1}", us.CarName, us.CarSkin);
        switch (us.AssistHelp)
        {
          case AssistHelp.Gamer:
            SetUI(GamerButton, "Gamer", true);
            break;
          case AssistHelp.Pro:
            SetUI(ProButton, "Pro", true);
            break;
          case AssistHelp.SemiPro:
            SetUI(SemiProButton, "SemiPro", true);
            break;
          case AssistHelp.Racer:
            SetUI(RacerButton, "Racer", true);
            break;
        }
      }
      return Task.CompletedTask;
    }

    private void SetAssist(Assists a)
    {
      assistSaveFile["ASSISTS"].Set("IDEAL_LINE", a.IDEAL_LINE);
      assistSaveFile["ASSISTS"].Set("AUTO_BLIP", a.AUTO_BLIP);
      assistSaveFile["ASSISTS"].Set("STABILITY_CONTROL", a.STABILITY_CONTROL);
      assistSaveFile["ASSISTS"].Set("AUTO_BRAKE", a.AUTO_BRAKE);
      assistSaveFile["ASSISTS"].Set("AUTO_SHIFTER", a.AUTO_SHIFTER);
      assistSaveFile["ASSISTS"].Set("ABS", a.ABS);
      assistSaveFile["ASSISTS"].Set("TRACTION_CONTROL", a.TRACTION_CONTROL);
      assistSaveFile["ASSISTS"].Set("AUTO_CLUTCH", a.AUTO_CLUTCH);
      assistSaveFile["ASSISTS"].Set("VISUALDAMAGE", a.VISUALDAMAGE);
      assistSaveFile["ASSISTS"].Set("DAMAGE", a.DAMAGE);
      assistSaveFile["ASSISTS"].Set("FUEL_RATE", a.FUEL_RATE);
      assistSaveFile["ASSISTS"].Set("TYRE_WEAR", a.TYRE_WEAR);
      assistSaveFile["ASSISTS"].Set("TYRE_BLANKETS", a.TYRE_BLANKETS);
      assistSaveFile["ASSISTS"].Set("SLIPSTREAM", a.SLIPSTREAM);
      assistSaveFile.Save();
    }
    private void SetAssistBtnsData(string ini)
    {
      var all = new AllAssists();
      all.Assist = IniLazySingleTonService.Instance.ReadAssistIniFileName(ini);
      all.Initialize(ini);
      DataContext = all;
      SetAssist(all.Assist);
      UnSelectAll();
      RaceButtonStatus();
    }

    private void UnSelectAll()
    {
      SetUI(GamerButton, "Gamer", false);
      SetUI(ProButton, "Pro", false);
      SetUI(SemiProButton, "SemiPro", false);
      SetUI(RacerButton, "Racer", false);
    }

    private async void Gamer_Click(object sender, RoutedEventArgs e)
    {
      var ini = "gamer.ini";
      SetAssistBtnsData(ini);
      us.AssistHelp = AssistHelp.Gamer;
      SetUI(GamerButton, "Gamer", true);
    }

    private async void Pro_Click(object sender, RoutedEventArgs e)
    {
      var ini = "pro.ini";
      SetAssistBtnsData(ini);
      us.AssistHelp = AssistHelp.Pro;
      SetUI(ProButton, "Pro", true);

    }

    private async void Racer_Click(object sender, RoutedEventArgs e)
    {
      var ini = "racer.ini";
      SetAssistBtnsData(ini);
      us.AssistHelp = AssistHelp.Racer;
      SetUI(RacerButton, "Racer", true);
    }

    private async void SemiPro_Click(object sender, RoutedEventArgs e)
    {
      var ini = "tracks.ini";
      SetAssistBtnsData(ini);
      us.AssistHelp = AssistHelp.SemiPro;
      SetUI(SemiProButton, "SemiPro", true);
    }

    private async void Cars_Click(object sender, RoutedEventArgs e)
    {
      timer.Stop();
      carClick = true;
      if (us != null)
      {
        Prepare();
        if (us.PlayerType == PlayerType.MultiPlayer && !IniLazySingleTonService.Instance.GetAutoMode())
          await navigationService.ShowDialogAsync<MultiPlayerCarWindow>(us);
        else
          await navigationService.ShowDialogAsync<CarWindow>(us);
      }

    }

    private async void Tracks_Click(object sender, RoutedEventArgs e)
    {
      Prepare();
      var result = await navigationService.ShowDialogAsync<TrackWindow>(us);
    }

    private async void Previous_Click(object sender, RoutedEventArgs e)
    {

      if (us != null)
      {
        Prepare();
        if (us.PlayerType == PlayerType.MultiPlayer)
          await navigationService.ShowDialogAsync<MainWindow>(us);
        else
          await navigationService.ShowDialogAsync<SinglePlayerWindow>(us);
      }
    }


    private int RaceButtonStatus()
    {
      Process[] pName = Process.GetProcessesByName("acs.exe");
      if (pName.Length == 0) //process not running
      {
        StartRaceButton.Style = (Style)FindResource("NormalStyle");
        StartRaceButton.Content = "Start Race";
      } else
      {
        StartRaceButton.Style = (Style)FindResource("SelectedStyle");
        StartRaceButton.Content = "Race Running...";
      }
      return pName.Length;
    }

    private async void StartRace_Click(object sender, RoutedEventArgs e)
    {
      Utils.StartACS();
      RaceButtonStatus();
    }

    private void SetUI(Button btn, string content, bool select)
    {
      btn.Content = select ? "V " + content : content;
      btn.Style = select ? (Style)FindResource("SelectedStyle") : (Style)FindResource("NormalStyle");
    }

    void Prepare()
    {
      Hide();
      RaceButtonStatus();
    }
  }
}
