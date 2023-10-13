using System.Threading.Tasks;

namespace RaceClient.Navigation
{
  public interface IActivable
  {
    Task ActivateAsync(object parameter);
  }
}
