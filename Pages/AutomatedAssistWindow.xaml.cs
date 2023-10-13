using RaceClient.DataFile;
using RaceClient.Models;
using RaceClient.Navigation;
using RaceClient.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RaceClient.Pages
{
  /// <summary>
  /// Interaction logic for AutomatedRace.xaml
  /// </summary>
  public partial class AutomatedAssistWindow : Window, IActivable
  {
    private readonly SimpleNavigationService navigationService;
    private IniFile assistSaveFile;
    UserSelection us;
    DispatcherTimer timer = new DispatcherTimer();


    string AssetLocation;

    public AutomatedAssistWindow(SimpleNavigationService navigationService)
    {
      InitializeComponent();
      this.navigationService = navigationService;

      var a = new AllAssists();
      a.Initialize("gamer.ini");
      assistSaveFile = new IniFile(IniLazySingleTonService.Instance.ReadIniFileName("assists.ini"));
      DataContext = a;

      AssetLocation = IniLazySingleTonService.Instance.GetAssetsLocation();

      RaceTimer.Visibility = Visibility.Visible;
      int timeout = IniLazySingleTonService.Instance.GetTimeout();
      timer.Interval = TimeSpan.FromSeconds(timeout);
      timer.Tick += timer_Tick;
      timer.Start();

    }

    private async void timer_Tick(object sender, EventArgs e)
    {
      timer.Stop();
      Hide();
      await navigationService.ShowDialogAsync<MultiPlayerCarWindow>(us);
    }

    public Task ActivateAsync(object parameter)
    {
      us = (UserSelection)parameter;
      if (us != null)
      {

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
    }

    private void UnSelectAll()
    {
      SetUI(GamerButton, "Gamer", false);
      SetUI(ProButton, "Pro", false);
      SetUI(SemiProButton, "SemiPro", false);

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

    private async void SemiPro_Click(object sender, RoutedEventArgs e)
    {
      var ini = "tracks.ini";
      SetAssistBtnsData(ini);
      us.AssistHelp = AssistHelp.SemiPro;
      SetUI(SemiProButton, "SemiPro", true);
    }

    private void SetUI(Button btn, string content, bool select)
    {
      //switch (content)
      //{
      //  case "Gamer":
      //    Assist.AssistImage = (ImageSource)new ImageSourceConverter().ConvertFromString(Gamer);
      //    break;
      //  case "SemiPro":
      //    Assist.AssistImage = (ImageSource)new ImageSourceConverter().ConvertFromString(SemiPro);
      //    break;
      //  case "Pro":
      //    Assist.AssistImage = (ImageSource)new ImageSourceConverter().ConvertFromString(Pro);
      //    break;
      //}
      //btn.Content = (ImageSource)new ImageSourceConverter().ConvertFromString(src);
      btn.Style = select ? (Style)FindResource("SelectedStyle") : (Style)FindResource("NormalStyle");
    }


  }
}
