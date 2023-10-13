using System;
using System.Collections.Generic;

namespace RaceClient.Models
{
  public class Car
  {
    public string name { get; set; }
    public string brand { get; set; }
    public string description { get; set; }
    public string brandimagedir { get; set; }
    public List<string> tags { get; set; }
    public string @class { get; set; }
    public Specs specs { get; set; }
    public List<List<string>> torqueCurve { get; set; }
    public List<List<string>> powerCurve { get; set; }
  }

  public class Specs
  {
    public string bhp { get; set; }
    public string torque { get; set; }
    public string weight { get; set; }
    public string topspeed { get; set; }
    public string acceleration { get; set; }
    public string pwratio { get; set; }
    public int range { get; set; }
  }

  public class CarData
  {
    public CarData(string carModel, string skin, string imgDir)
    {
      CarModel = carModel;
      ImageDir = imgDir;
      Id = Guid.NewGuid();
      Skin = skin;
    }

    public string UIName { get; set; }
    public string CarModel { get; set; }
    public Guid Id { get; set; }

    public string ImageDir { get; set; }

    public string Skin { get; set; }
    public CarType Type { get; set; }

    public Car Car { get; set; }
  }

  public enum CarType
  {
    AllTypes,
    SingleSeater,
    GT3
  }

}
