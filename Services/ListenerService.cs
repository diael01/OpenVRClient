using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RaceClient.DataFile;
using RaceClient.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using VMSShared;

namespace RaceClient.Services
{
  public interface IListenerSingletonService
  {
    bool ConnectServer();

    AppSettings Listen(Object threadContext);

    void SendProcessState();

    void Disconnect();

    void SendGameStarted();
  }

  public class ListenerSingletonService : LazySingletonService<ListenerSingletonService>, IListenerSingletonService
  {
    TcpClient Client;
    bool StatusSent;

    public bool ConnectServer()
    {
      bool connected;
      try
      {
        if (!IsConnected())
        {
          Client = new TcpClient();
          var ip = Config.GetValue<string>("AppSettings:ServerIP");
          var serverPort = Config.GetValue<int>("AppSettings:ServerPort");
          Client.ConnectAsync(ip, serverPort);
          connected = Client.Connected ? true : false;
        } else
          connected = true;
      } catch (Exception ex)
      {
        throw new Exception(string.Format("Fatal exception,\nError was:{0},{1},{2}" + ex.ToString(), System.Diagnostics.EventLogEntryType.Error, 5999));
      }
      return connected;
    }

    public void SendProcessState()
    {
      var podIP = Config.GetValue<string>("AppSettings:PodIP");
      if (!IsConnected()) ConnectServer();
      if (IsConnected())
      {
        //check if ACS is running to send the NORUN comand or RUNNING Command
        Process[] pname = Process.GetProcessesByName("acs.exe");
        NetworkStream ClientStream = Client.GetStream();

        var coms = pname.Length == 0 ? new ClientComms(podIP, "NORUN") : new ClientComms(podIP, "RUNNING");
        var json = JsonConvert.SerializeObject(coms);
        var send = Encoding.ASCII.GetBytes(json);
        ClientStream.Write(send, 0, send.Length);
        ClientStream.Flush();
        byte[] MessageRec = new byte[1024];
        if (StatusSent == false) //read only once
        {
          int bytesread = ClientStream.Read(MessageRec, 0, MessageRec.Length);
          if (bytesread != 0)
          {
            var returnMessage = Encoding.ASCII.GetString(MessageRec, 0, bytesread);
            if (returnMessage != null)
            {
              var jrtn = JsonConvert.DeserializeObject<ServerComms>(returnMessage);
              if (jrtn.Function == "OK") //return from send status
              {
                StatusSent = true;
                //all good, log it
              } else
              {
                throw new Exception("Fatal: connection not ok");
              }
            }
          }
        }
      }
    }

    public bool IsConnected()
    {
      return (Client != null && Client.Connected) ? true : false;
    }


    public void Disconnect()
    {
      Client.Client.Shutdown(SocketShutdown.Both);
      Client = null;
    }

    public void SendGameStarted()
    {
      try
      {
        var podIP = Config.GetValue<string>("AppSettings:PodIP");
        if (!IsConnected()) ConnectServer();
        if (IsConnected())
        {

          NetworkStream ClientStream = Client.GetStream();
          var coms = new ClientComms(podIP, "GAMESTARTED");
          var json = JsonConvert.SerializeObject(coms);
          var send = Encoding.ASCII.GetBytes(json);
          ClientStream.Write(send, 0, send.Length);
          ClientStream.Flush();
          byte[] MessageRec = new byte[1024];

          int bytesread = ClientStream.Read(MessageRec, 0, MessageRec.Length);
          if (bytesread != 0)
          {
            var returnMessage = Encoding.ASCII.GetString(MessageRec, 0, bytesread);
            if (returnMessage != null)
            {
              var jrtn = JsonConvert.DeserializeObject<ServerComms>(returnMessage);
              if (jrtn.Function == "STARTGAME") //return from send status
              {
                //todo: guess thats the OK, will log it on debug
              } 
            }
          }
        }
      } catch (Exception ex)
      {
        //todo: log it
        throw ex;
      }
    }


    public AppSettings Listen(Object threadContext)
    {
      UserSelection us = (UserSelection)threadContext;
      byte[] MessageRec = new byte[1024];
      try
      {
        if (!IsConnected()) ConnectServer();
        if (IsConnected())
        {
          SendProcessState(); //send again because otherwise we are sitting duck, this is an issue with the server but hey i can get around it here
          NetworkStream ClientStream = Client.GetStream();
          int bytesread = ClientStream.Read(MessageRec, 0, MessageRec.Length);
          if (bytesread != 0)
          {
            var returnMessage = Encoding.ASCII.GetString(MessageRec, 0, bytesread);
            if (returnMessage != null)
            {
              var jrtn = JsonConvert.DeserializeObject<ServerComms>(returnMessage);
              {
                if (us != null)
                {
                  switch (jrtn.Function)
                  {
                    case "STARTGAME":                      
                      SendGameStarted();//send this so we dont get startgame command again, UNLESS User presses "start pods";
                      us.GameStatus = GameStatus.StartGame;
                      break;
                    case "ENDGAME":
                      us.GameStatus = GameStatus.EndGame;
                      break;
                    case "MPCONFIG":
                      if (jrtn.Payload != null)
                      {
                        var mpConfig = JsonConvert.DeserializeObject<MPConfig>(jrtn.Payload);
                        us.MPConfig = new AppSettings();
                        us.MPConfig.ServerIP = mpConfig.ServerIP;
                        us.MPConfig.ServerPort = mpConfig.ServerPort;
                        us.MPConfig.ServerName = mpConfig.ServerName;
                        us.MPConfig.PodIP = mpConfig.PodIP;
                        us.MPConfig.ServerHTTPPort = mpConfig.ServerHTTPPort;
                        us.MPConfig.CarList = mpConfig.CarList.ToList();
                        us.MPConfig.TrackName = mpConfig.TrackName;
                        us.MPConfig.TrackVariation = mpConfig.TrackVariant;

                        //save in race.ini
                        var iniFile = new IniFile(IniLazySingleTonService.Instance.ReadIniFileName("race.ini"));
                        if (iniFile != null)
                        {
                          iniFile["REMOTE"].Set("SERVER_IP", (us.MPConfig.ServerIP ?? "").ToLowerInvariant());
                          iniFile["REMOTE"].Set("SERVER_PORT", (us.MPConfig.ServerPort ?? "").ToLowerInvariant());
                          iniFile["REMOTE"].Set("SERVER_NAME", (us.MPConfig.ServerName ?? "").ToLowerInvariant());
                          iniFile["REMOTE"].Set("SERVER_HTTP_PORT", (us.MPConfig.ServerHTTPPort ?? "").ToLowerInvariant());
                          iniFile["REMOTE"].Set("NAME", (us.MPConfig.PodIP ?? "").ToLowerInvariant());
                          iniFile["REMOTE"].Set("ACTIVE", "1");
                          iniFile["REMOTE"].Set("GUID", Guid.NewGuid().ToString());
                          iniFile.Save();
                        }
                      }
                      break;
                  }
                  return us.MPConfig;
                }
              }
            }

          }
        }
        //else do nothing coz there is an ACS running      
      } catch (Exception ex)
      {
        int i = 0;
        i++;
        //log it, but dnt stop the app just because a connection failed
      }
      return null;
    }

  }
}

