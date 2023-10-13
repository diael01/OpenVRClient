using Microsoft.Extensions.Configuration;
using RaceClient.DataFile;
using RaceClient.Models;
using System.IO;

namespace RaceClient.Services
{
  public interface IIniService
  {
    string ReadIniFileName(string fileName);

    string ReadExeFileName();

    Assists ReadAssistIniFileName(string fileName);

    AppSettings ReadSettings();
  }

  public class IniLazySingleTonService : LazySingletonService<IniLazySingleTonService>, IIniService
  {

    public string GetAssetsLocation()
    {
      var path = Config.GetValue<string>("AppSettings:ImagesLocation");
      return path;
    }

    public string ReadExeFileName()
    {
      var path = Config.GetValue<string>("AppSettings:ExeLocation");
      return path;
    }

    public string ReadIniFileName(string fileName)
    {
      var path = Config.GetValue<string>("AppSettings:IniLocation");
      return string.Format("{0}\\{1}", path, fileName);
    }

    public Assists ReadAssistIniFileName(string fileName)
    {
      var path = Config.GetValue<string>("AppSettings:AssistLocation");
      string fullPath = string.Format("{0}\\{1}", path, fileName);
      Assists a = new Assists();
      if (File.Exists(fullPath))
      {
        var iniFile = new IniFile(fullPath);
        a.IDEAL_LINE = iniFile["ASSISTS"].GetInt("IDEAL_LINE", 0);
        a.AUTO_BLIP = iniFile["ASSISTS"].GetInt("AUTO_BLIP", 0);
        a.STABILITY_CONTROL = iniFile["ASSISTS"].GetInt("STABILITY_CONTROL", 0);
        a.AUTO_BRAKE = iniFile["ASSISTS"].GetInt("AUTO_BRAKE", 0);
        a.AUTO_SHIFTER = iniFile["ASSISTS"].GetInt("AUTO_SHIFTER", 0);
        a.ABS = iniFile["ASSISTS"].GetInt("ABS", 0);
        a.TRACTION_CONTROL = iniFile["ASSISTS"].GetInt("TRACTION_CONTROL", 0);
        a.AUTO_CLUTCH = iniFile["ASSISTS"].GetInt("AUTO_CLUTCH", 0);
        a.VISUALDAMAGE = iniFile["ASSISTS"].GetInt("VISUALDAMAGE", 0);
        a.DAMAGE = iniFile["ASSISTS"].GetInt("DAMAGE", 0);
        a.FUEL_RATE = iniFile["ASSISTS"].GetInt("FUEL_RATE", 0);
        a.TYRE_WEAR = iniFile["ASSISTS"].GetInt("TYRE_WEAR", 0);
        a.TYRE_BLANKETS = iniFile["ASSISTS"].GetInt("TYRE_BLANKETS", 0);
        a.SLIPSTREAM = iniFile["ASSISTS"].GetInt("SLIPSTREAM", 0);
      }
      return a;
    }

    public AppSettings ReadSettings()
    {
      AppSettings s = new AppSettings();
      s.ServerName = Config.GetValue<string>("AppSettings:ServerName");
      s.ServerIP = Config.GetValue<string>("AppSettings:ServerIP");
      s.ServerPort = Config.GetValue<string>("AppSettings:ServerPort");
      s.ServerHTTPPort = Config.GetValue<string>("AppSettings:ServerHTTPPort");
      s.PodIP = Config.GetValue<string>("AppSettings:PodIP");
      s.CarsLocation = Config.GetValue<string>("AppSettings:CarsLocation");
      s.TracksLocation = Config.GetValue<string>("AppSettings:TracksLocation");
      s.AssistLocation = Config.GetValue<string>("AppSettings:AssistLocation");
      s.ExeLocation = Config.GetValue<string>("AppSettings:ExeLocation");
      return s;
    }

    public bool GetAutoMode()
    {
      var mod = Config.GetValue<string>("AppSettings:AutoMode");
      return mod == "On" ? true : false;
    }

    public int GetTimeout()
    {
      return Config.GetValue<int>("AppSettings:Timeout");
    }
  }
}
