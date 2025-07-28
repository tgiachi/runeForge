using Runeforge.Engine.Interfaces.DataLoaders;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IDataLoaderService : IRuneforgeStartableService
{
    void AddDataLoader<TDataLoader, TDataType>() where TDataLoader : IDataLoader where TDataType : class;
}
