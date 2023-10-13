using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RaceClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Track = RaceClient.Models.Track;

namespace RaceClient.Services
{
  public interface ITrackService
  {
    List<TrackData> ReadAllTracks();

    List<TrackData> SetTrack(string track, string variation); //this will probably be modified to send the liast of track, same way as for cars; why should the user have only 1 choice!?
  }

  public class TrackLazySingleTonService : LazySingletonService<TrackLazySingleTonService>, ITrackService
  {

    public List<TrackData> SetTrack(string track, string variation)
    {
      if (Config != null)
      {
        string tracksDir = Config.GetValue<string>("AppSettings:TracksLocation");
        var trackNames = Directory.GetDirectories(tracksDir);
        var fullPath = trackNames.Where(c => trackNames.Any(c => c.Substring(c.LastIndexOf("\\") + 1).Equals(track))).FirstOrDefault();
        List<TrackData> obs = new List<TrackData>();
        string data = string.Format("{0}\\ui\\ui_track.json", fullPath);
        if (File.Exists(data))
        {
          string imageDir = string.Format("{0}\\ui\\outline.png", fullPath);
          string varImageDir = string.Format("{0}\\ui\\preview.png", fullPath);
          if (!File.Exists(varImageDir))
            varImageDir = string.Format("{0}\\ui\\dlc_preview.png", fullPath);
          AddTrack(imageDir, track, ref obs, data, variation, varImageDir);
        } else //perhaps ther eis a variation
        {
          data = string.Format("{0}\\{1}\\ui\\ui_track.json", fullPath, variation);
          if (File.Exists(data))
          {
            string imageDir = string.Format("{0}\\{1}\\ui\\outline.png", fullPath);
            string varImageDir = string.Format("{0}\\{1}\\ui\\preview.png", fullPath);
            if (!File.Exists(varImageDir))
              varImageDir = string.Format("{0}\\{1}\\ui\\dlc_preview.png", fullPath);
            AddTrack(imageDir, track, ref obs, data, variation, varImageDir);
          }
        }
        return obs;
      }
      return null;
    }

    public List<TrackData> ReadAllTracks()
    {
      try
      {
        if (Config != null)
        {
          string tracksDir = Config.GetValue<string>("AppSettings:TracksLocation");
          var trackNames = Directory.GetDirectories(tracksDir);
          List<TrackData> obs = new List<TrackData>();
          foreach (var track in trackNames)
          {
            //var carInfo = new Car();
            string data = string.Format("{0}\\ui\\ui_track.json", track);
            if (File.Exists(data))
            {
              string imageDir = string.Format("{0}\\ui\\outline.png", track);

              AddTrack(imageDir, track, ref obs, data, null);
            } else
            {
              var subTrackNames = Directory.GetDirectories(string.Format("{0}\\ui", track));
              foreach (var subTrack in subTrackNames)
              {
                string subdata = string.Format("{0}\\ui_track.json", subTrack);
                if (!File.Exists(subdata))
                  subdata = string.Format("{0}\\dlc_ui_track.json", subTrack);
                string img = string.Format("{0}\\outline.png", subTrack);
                var variation = track.Substring(track.LastIndexOf('\\') + 1);
                AddTrack(img, subTrack, ref obs, subdata, variation);
              }
            }
          }
          return obs;
        }
      } catch (Exception ex)
      {
        //log it
        throw;
      }
      return null;
    }

    private void AddTrack(string imageDir, string track, ref List<TrackData> obs, string data, string variation = null, string variationImageDir=null)
    {

      if (File.Exists(imageDir))
      {
        //read data from .json
        var trackModel = track.Substring(track.LastIndexOf('\\') + 1);
        var trackData = variation != null ? new TrackData(trackModel, imageDir, variation, variationImageDir) : new TrackData(trackModel, imageDir);
        var json = File.ReadAllText(data);
        trackData.UIName = variation != null ? string.Format("{0}/{1}", trackModel, variation) : trackModel;
        trackData.Track = JsonConvert.DeserializeObject<Track>(json);
        //trackData.Variation = variation;
        trackData.Country = trackData.Track.country;
        obs.Add(trackData);
      }
    }
  }
}
