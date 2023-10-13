using JetBrains.Annotations;

namespace RaceClient.DataFile
{
  public interface IDataFile
  {
    [NotNull]
    string Name { get; }

    [CanBeNull]
    string Filename { get; }

    void Initialize([CanBeNull] IDataReadWrapper data, [NotNull] string name, [CanBeNull] string filename);
  }

  public interface IDataReadWrapper
  {
    bool IsEmpty { get; }

    bool IsPacked { get; }

    [NotNull]
    T GetFile<T>([NotNull] string name) where T : IDataFile, new();

    [CanBeNull]
    string GetData([NotNull] string name);
  }

  public interface IDataWrapper : IDataReadWrapper
  {
    [CanBeNull]
    string Location { get; }

    bool Contains([NotNull] string name);
    void Refresh([CanBeNull] string name);
    void SetData([NotNull] string name, [CanBeNull] string data, bool recycleOriginal = false);
    void Delete([NotNull] string name, bool recycleOriginal = false);
  }

  public interface ISyntaxErrorsCatcher
  {
    void Catch(DataFileBase file, int line);
  }

}
