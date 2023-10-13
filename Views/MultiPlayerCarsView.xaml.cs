using RaceClient.DataFile;
using RaceClient.Models;
using RaceClient.Services;
using System.Windows.Controls;

namespace RaceClient.Views
{

  public partial class MultiPlayerCarsView : UserControl
  {
    IniFile iniFile;

    public MultiPlayerCarsView()
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
        }
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
