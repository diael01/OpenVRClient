using System.Diagnostics;
using System.Threading;

namespace RaceClient.Services
{
  public static class Utils
  {

    public static bool StartACS()
    {
      Process[] pName = Process.GetProcessesByName("acs.exe");
      if (pName.Length == 0) //process not running
      {
        var exe = IniLazySingleTonService.Instance.ReadExeFileName();
        if (!string.IsNullOrEmpty(exe))
        {
          var dir = exe.Substring(0, exe.LastIndexOf("\\"));
          Process process = new Process();
          process.StartInfo.FileName = exe;
          process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
          process.StartInfo.WorkingDirectory = dir;
          process.Start();
          return process.WaitForExit(Timeout.Infinite);
        }
      }
      return false;
    }
  }
}
