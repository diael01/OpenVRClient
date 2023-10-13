using Microsoft.Extensions.Configuration;
using System;

namespace RaceClient.Services
{
  public class LazySingletonService<T> where T : class, new()
  {
    protected LazySingletonService() { }

    protected static Lazy<T> instance = new Lazy<T>(() => new T());

    public static T Instance
    {
      get { return instance.Value; }
    }

    public IConfiguration Config { get; set; }
  }
}
