using System.Collections.Generic;

namespace RaceClient.Models
{
  public class AppSettings
  {
    public string ServerIP { get; set; }
    public string ServerPort { get; set; }
    public string CarsLocation { get; set; }
    public string TracksLocation { get; set; }
    public string AssistLocation { get; set; }
    public string ExeLocation { get; set; }
    public string IniLocation { get; set; }
    public string PodIP { get; set; }
    public string ServerName { get; set; }
    public string ServerHTTPPort { get; set; }
    public IEnumerable<string> CarList { get; set; }
    public string TrackName { get; set; }
    public string TrackVariation { get; set; }
    public string AutoMode { get; set; }
    public int Timeout { get; set; }

  }

}
