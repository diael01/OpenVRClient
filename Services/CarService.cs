using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RaceClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace RaceClient.Services
{
  public interface ICarService
  {
    IEnumerable<CarData> ReadCars(IEnumerable<string> filter = null, string cls = null);

    IEnumerable<string> ReadClasses();

  }

  public class CarLazySingleTonService : LazySingletonService<CarLazySingleTonService>, ICarService
  {

    public IEnumerable<string> ReadClasses()
    {
      string carsDir = Config.GetValue<string>("AppSettings:CarsLocation");
      var carNames = Directory.GetDirectories(carsDir).ToList();
      List<string> existingTags = new List<string>();
      foreach (var carName in carNames)
      {
        string data = string.Format("{0}\\ui\\ui_car.json", carName);
        var json = File.ReadAllText(data);
        var car = JsonConvert.DeserializeObject<Car>(json);
        var missing = car.tags.Except(existingTags).ToList();
        existingTags.AddRange(missing);
      }
      return existingTags;
    }

    public IEnumerable<CarData> ReadCars(IEnumerable<string> filter = null, string cls = null)
    {
      try
      {
        if (Config != null)
        {
          string carsDir = Config.GetValue<string>("AppSettings:CarsLocation");
          var carNames = Directory.GetDirectories(carsDir).ToList();
          Dictionary<string, IEnumerable<string>> carSkins = null;
          IEnumerable<string> common = null, onlyNames = null;

          if (filter != null)
          {
            carSkins = new Dictionary<string, IEnumerable<string>>();
            foreach (var carAndskin in filter)
            {

              var car = carAndskin.Substring(0, carAndskin.IndexOf(":")).Trim();
              if (carSkins.Keys.Contains(car))
                continue; //we already added all the skins
              //find ONLY Skins
              var allcarskins = filter.Where(c => c.Substring(0, c.IndexOf(":")).Equals(car)).ToList();
              var justskins = allcarskins.Select(cn => cn.Substring(cn.IndexOf(":") + 1).Trim()).ToList();
              //add them to map
              carSkins[car] = justskins;
            }
            //get only the car names from server
            onlyNames = filter.Select(c => c.Substring(0, c.IndexOf(":")).Trim()).ToList();
            //strip off the directory names and leave only the car names from local directories so we can compare
            var onlyNamesLocally = carNames.Select(c => c.Substring(c.LastIndexOf("\\") + 1)).ToList();
            //intersect local with server names
            var commonNames = (filter != null) ? onlyNames.Intersect(onlyNamesLocally).ToList() : carNames;
            //add back the directory names for the ImageDir         
            common = carNames.Where(c => commonNames.Any(cn => cn.Equals(c.Substring(c.LastIndexOf("\\") + 1)))).ToList();
          } else
            common = carNames;

          List<CarData> obs = new List<CarData>();
          foreach (var car in common)
          {
            string data = string.Format("{0}\\ui\\ui_car.json", car);
            string brandimageDir = string.Format("{0}\\ui\\badge.png", car);
            var skinDir = string.Format("{0}\\skins", car);
            if (Directory.Exists(skinDir))
            {
              var carModel = car.Substring(car.LastIndexOf('\\') + 1);
              var localSkins = Directory.GetDirectories(skinDir).AsEnumerable();
              var onlySkins = localSkins.Select(s => s.Substring(s.LastIndexOf("\\") + 1)).AsEnumerable();
              if (carSkins != null) //multiplayer
              {
                IEnumerable<string> skins = carSkins[carModel];
                TryAddCar(ref skins, ref carModel, ref skinDir, car, ref cls, ref obs);//, true);
              } else
              {
                TryAddCar(ref onlySkins, ref carModel, ref skinDir, car, ref cls, ref obs);//, false);
              }
            }
          }
          return obs;
        }
      } catch (Exception ex)
      {
        //log and throw;
        throw;
      }
      return null;
    }


    private void TryAddCar(ref IEnumerable<string> skins, ref string carModel, ref string skinDir, string car, ref string cls, ref List<CarData> obs)//, bool multi)
    {
      bool added = false;
      foreach (var skin in skins)
      {
        var imageDir = string.Format("{0}\\{1}\\preview.jpg", skinDir, skin);
        string data = string.Format("{0}\\ui\\ui_car.json", car);
        var carData = new CarData(carModel, skin, imageDir);
        carData.Type = CarType.AllTypes;
        carData.UIName = string.Format("{0}/{1}", carModel, skin);
        var json = File.ReadAllText(data);
        carData.Car = JsonConvert.DeserializeObject<Car>(json);
        if ((!string.IsNullOrEmpty(cls) && (carData.Car.@class.ToLower().Contains(cls) ||
                            carData.Car.@class.Contains(cls) ||
                            carData.Car.name.ToLower().Contains(cls) ||
                            carData.Car.name.Contains(cls))
              )
          || string.IsNullOrEmpty(cls))
        {
          obs.Add(carData);
          added = true;
        }
        if (added)
        {
          break;//only add 1 car skin for each car
        }
      }
    }
  }
}
