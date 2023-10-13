using RaceClient.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace RaceClient.Models
{
  public class AViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;
    public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }

  public enum AssistHelp
  {
    Gamer,
    Pro,
    SemiPro,
    Racer
  }

  public enum PlayerType
  {
    SinglePlayer,
    MultiPlayer,
  }

  public enum GameType
  {
    Totem,
    FreeRoam,
    Race
  }

  public enum GameStatus
  {
    Idle,
    StartGame,
    EndGame,
  }


  public class UserSelection
  {
    public string CarModel { get; set; }

    public string CarName { get; set; }

    public string CarSkin { get; set; }

    public string CarImageDir { get; set; }

    public string Track { get; set; }

    public string? TrackVariation { get; set; }

    public string TrackImageDir { get; set; }

    public string VariationImageDir { get; set; }

    public AssistHelp AssistHelp { get; set; }

    public GameType GameType { get; set; }

    public PlayerType PlayerType { get; set; }

    public AppSettings MPConfig { get; set; }

    public GameStatus GameStatus { get; set; }

  }

  public class AllAssists : AViewModel
  {
    public Assists Assist { get; set; }
    string Checked;
    string Unchecked;
    string Gamer;
    string SemiPro;
    string Pro;
    string AssetLocation;
    string Cog;
    ImagesPath paths;
    public void Initialize(string iniFile, ImagesPath paths=null)
    {
      Assist = IniLazySingleTonService.Instance.ReadAssistIniFileName(iniFile);
      AssetLocation = IniLazySingleTonService.Instance.GetAssetsLocation();
      if (!string.IsNullOrEmpty(AssetLocation))
      {
        Cog = AssetLocation + "\\Cog.png";
        Checked = AssetLocation + "\\Checked.png";
        Unchecked = AssetLocation + "\\Unchecked.png";
        Gamer = AssetLocation + "\\Assists\\Gamer.png";
        SemiPro = AssetLocation + "\\Assists\\Moderate.png";
        Pro = AssetLocation + "\\Assists\\Pro.png";
      }
      SetAssistSpecs(iniFile);
      if(paths!=null)
        SetImages(paths);
    }

    private void SetAssistSpecs(string iniFile)
    {
      Assist.Ideal = (ImageSource)new ImageSourceConverter().ConvertFromString(Assist.IDEAL_LINE == 1 ? Checked : Unchecked);
      Assist.Blip = (ImageSource)new ImageSourceConverter().ConvertFromString(Assist.AUTO_BLIP == 1 ? Checked : Unchecked);
      Assist.Brake = (ImageSource)new ImageSourceConverter().ConvertFromString(Assist.AUTO_BRAKE == 1 ? Checked : Unchecked);
      Assist.Shifter = (ImageSource)new ImageSourceConverter().ConvertFromString(Assist.AUTO_SHIFTER == 1 ? Checked : Unchecked);

      Assist.Abs = (ImageSource)new ImageSourceConverter().ConvertFromString(Assist.ABS == 1 ? Checked : (Assist.ABS == 0 ? Unchecked : (Assist.ABS == 2 ? Cog : "")));
      Assist.Traction = (ImageSource)new ImageSourceConverter().ConvertFromString(Assist.TRACTION_CONTROL == 1 ? Checked : (Assist.TRACTION_CONTROL == 0 ? Unchecked : (Assist.TRACTION_CONTROL == 2 ? Cog : "")));
      /////////
      Assist.Clutch = (ImageSource)new ImageSourceConverter().ConvertFromString(Assist.AUTO_CLUTCH == 1 ? Checked : Unchecked);
      Assist.Visual = (ImageSource)new ImageSourceConverter().ConvertFromString(Assist.VISUALDAMAGE == 1 ? Checked : Unchecked);
      Assist.Tyre = (ImageSource)new ImageSourceConverter().ConvertFromString(Assist.TYRE_BLANKETS == 1 ? Checked : Unchecked);
      Assist.Stability = string.Format("{0}%", Assist.STABILITY_CONTROL.ToString());

      switch (iniFile.ToLower())
      {
        case "gamer.ini":
          Assist.AssistImage = (ImageSource)new ImageSourceConverter().ConvertFromString(Gamer);
          break;
        case "tracks.ini":
          Assist.AssistImage = (ImageSource)new ImageSourceConverter().ConvertFromString(SemiPro);
          break;
        case "pro.ini":
          Assist.AssistImage = (ImageSource)new ImageSourceConverter().ConvertFromString(Pro);
          break;
        case "racer.ini":
          Assist.AssistImage = (ImageSource)new ImageSourceConverter().ConvertFromString(Pro);
          break;
      }      

    }

    private void SetImages(ImagesPath paths)
    {
      if(!string.IsNullOrEmpty(paths.CarImage))
        Assist.CarImage = (ImageSource)new ImageSourceConverter().ConvertFromString(paths.CarImage);
      if (!string.IsNullOrEmpty(paths.TrackImage))
        Assist.TrackImage = (ImageSource)new ImageSourceConverter().ConvertFromString(paths.TrackImage);
      if (!string.IsNullOrEmpty(paths.VariationImage))
        Assist.VariationImage = (ImageSource)new ImageSourceConverter().ConvertFromString(paths.VariationImage);
      if (!string.IsNullOrEmpty(paths.BrandImage))
        Assist.BrandImage = (ImageSource)new ImageSourceConverter().ConvertFromString(paths.BrandImage);
      if (!string.IsNullOrEmpty(paths.CountryImage))
        Assist.CountryImage = (ImageSource)new ImageSourceConverter().ConvertFromString(paths.CountryImage);
    }
  }
}
