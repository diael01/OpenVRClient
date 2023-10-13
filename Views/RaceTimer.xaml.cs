using RaceClient.Services;
using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RaceClient.Views
{
  /// <summary>
  /// Interaction logic for Timer.xaml
  /// </summary>
  public partial class RaceTimer : UserControl
  {
    public int Timeout = IniLazySingleTonService.Instance.GetTimeout();
    public string Message { get; set; }
    public DispatcherTimer Timer { get; set; }
    public AutoResetEvent ActionEvent { get; set; }


    public RaceTimer()
    {
      InitializeComponent();
      Timer = new DispatcherTimer();
      ActionEvent = new AutoResetEvent(true);
      Timer.Interval = TimeSpan.FromSeconds(1);
      Timer.Tick += timer_Tick;
      SetMessage("{0}");
      Timer.Start();
    }

    public void SetMessage(string msg)
    {
      Message = msg;
    }

    void timer_Tick(object sender, EventArgs e)
    {
      lblTime.Text = string.Format(Message, Timeout);
      Timeout--;
      if (Timeout == 0)
      {
        Timer.Stop();
        ActionEvent.Set();
      }
    }
  }
}
