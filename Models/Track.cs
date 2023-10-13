using System;
using System.Collections.Generic;

namespace RaceClient.Models
{
  // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
  public class Track
  {
    public string name { get; set; }
    public string description { get; set; }
    public List<string> tags { get; set; }
    public List<string> geotags { get; set; }
    public string country { get; set; }
    public string city { get; set; }
    public string length { get; set; }
    public string width { get; set; }
    public string pitboxes { get; set; }
    public string run { get; set; }
  }

  public class TrackData
  {
    public TrackData(string main, string imgDir, string variation = null, string variationImageDir = null)
    {
      ImageDir = imgDir;
      Main = main;
      Variation = variation;
      VariationImageDir = variationImageDir;
      Id = Guid.NewGuid();
    }

    public string UIName { get; set; }
    public Guid Id { get; set; }

    public string Main { get; set; }

    public string Variation { get; set; }

    public string ImageDir { get; set; }

    public string VariationImageDir { get; set; }

    public Track Track { get; set; }

    public string Country { get; set; }
  }
}
