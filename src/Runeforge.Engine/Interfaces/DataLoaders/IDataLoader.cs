namespace Runeforge.Engine.Interfaces.DataLoaders;

public interface IDataLoader
{
    Task LoadDataAsync(object data, CancellationToken cancellationToken = default);
}
