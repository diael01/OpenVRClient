using RaceClient.DataFile;
using RaceClient.Models;
using RaceClient.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RaceClient.Views
{

  public class Tracks : AViewModel
  {
    public List<TrackData> AllTracks { get; set; }

    public UserSelection Us { get; set; }
    public Tracks(UserSelection us)
    {
      //AllTracks = TrackLazySingleTonService.Instance.ReadAllTracks();
      Us = us;
      if (us.PlayerType == PlayerType.SinglePlayer)
        AllTracks = TrackLazySingleTonService.Instance.ReadAllTracks();
      else if (us.MPConfig != null)
        AllTracks = TrackLazySingleTonService.Instance.SetTrack(us.MPConfig.TrackName, us.MPConfig.TrackVariation);
    }
  }

  public class TrackDataTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate
        SelectTemplate(object item, DependencyObject container)
    {
      if (item != null && item is TrackData)
      {
        var trackitem = (TrackData)item;
        var window = Application.Current.MainWindow;
      }
      return null;
    }
  }


  public partial class TracksView : UserControl
  {
    public string TrackName { get; set; }
    public TracksView()
    {
      InitializeComponent();
    }

    public void Initialize(UserSelection us)
    {
      Tracks t = new Tracks(us);
      DataContext = t;
    }

    private void tracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      TrackData trackData = (TrackData)trackList.SelectedItem;
      var index = trackList.SelectedIndex;
      Tracks t = (Tracks)DataContext;
      if (trackData != null && t.Us != null)
      {
        TrackName = trackData.Track.name;
        var iniFile = new IniFile(IniLazySingleTonService.Instance.ReadIniFileName("race.ini"));
        if (string.IsNullOrEmpty(trackData.Variation))
        {
          t.Us.Track = (trackData.Main ?? "").ToLowerInvariant();
          iniFile["RACE"].Set("TRACK", t.Us.Track);
          t.Us.TrackImageDir = (trackData.ImageDir ?? "").ToLowerInvariant();
          iniFile["RACE"].Set("CONFIG_TRACK", "");
          t.Us.TrackVariation = null;
        } else //is a variation
        {
          t.Us.Track = (trackData.Main ?? "").ToLowerInvariant();
          iniFile["RACE"].Set("TRACK", t.Us.Track);
          t.Us.TrackVariation = (trackData.Variation ?? "").ToLowerInvariant();
          iniFile["RACE"].Set("CONFIG_TRACK", t.Us.TrackVariation);
          t.Us.TrackImageDir = (trackData.ImageDir ?? "").ToLowerInvariant();
        }
        iniFile.Save();

      }
    }
  }

}
