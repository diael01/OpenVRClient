using RaceClient.DataFile;
using RaceClient.Models;
using RaceClient.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RaceClient.Views
{

  public class Cars : AViewModel
  {
    public IEnumerable<CarData> AllCars { get; set; }
    public UserSelection Us { get; set; }
    public Cars(UserSelection us, string cls = null)
    {
      Us = us;
      if (us.PlayerType == PlayerType.SinglePlayer)
      {
        AllCars = (cls != null) ? CarLazySingleTonService.Instance.ReadCars(null, cls) : CarLazySingleTonService.Instance.ReadCars();
      } else if (us.MPConfig != null)
      {
        var carList = us.MPConfig.CarList;
        AllCars = CarLazySingleTonService.Instance.ReadCars(carList);

      }
    }

  }

  public class CarDataTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate
        SelectTemplate(object item, DependencyObject container)
    {
      if (item != null && item is CarData)
      {
        var caritem = (CarData)item;
        var window = Application.Current.MainWindow;
      }
      return null;
    }
  }


  public partial class CarsView : UserControl
  {
    IniFile iniFile;
    public CarsView()
    {
      InitializeComponent();
    }

    public void Initialize(UserSelection us, string cls = null)
    {
      iniFile = new IniFile(IniLazySingleTonService.Instance.ReadIniFileName("race.ini"));
      Cars c = new Cars(us, cls);
      DataContext = c;
    }

    private void cars_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var carData = (CarData)carList.SelectedItem;
      var index = carList.SelectedIndex;
      if (carData != null)
      {
        Cars c = (Cars)DataContext;
        if (c.Us != null)
        {
          c.Us.CarName = carData.Car.name;
          c.Us.CarModel = (carData.CarModel ?? "").ToLowerInvariant();
          c.Us.CarSkin = (carData.Skin ?? "").ToLowerInvariant();
          c.Us.CarImageDir = (carData.ImageDir ?? "").ToLowerInvariant();
          TrackButton.Content = c.Us.TrackVariation != null ? string.Format("{0}/{1}", c.Us.Track, c.Us.TrackVariation) : c.Us.Track == null ? "Track mising" : c.Us.Track;
        }
        CarButton.Content = carData.UIName;
        iniFile["RACE"].Set("MODEL", (carData.CarModel ?? "").ToLowerInvariant());
        var sub = carData.ImageDir.Substring(0, carData.ImageDir.LastIndexOf("\\"));
        var skin = sub.Substring(sub.LastIndexOf("\\") + 1);
        iniFile["CAR_0"].Set("MODEL", (carData.CarModel ?? "").ToLowerInvariant());
        iniFile["CAR_0"].Set("SKIN", (skin ?? "").ToLowerInvariant());
        if (c.Us.PlayerType == PlayerType.MultiPlayer && c.Us.MPConfig != null)
        {
          iniFile["REMOTE"].Set("REQUESTED_CAR", (carData.CarModel ?? "").ToLowerInvariant());
          iniFile["SESSION_0"].Set("SPAWN_SET", "START");
        }
        iniFile.Save();
      }

    }
  }


}
