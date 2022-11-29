using Uject.Utilities;

namespace UJect
{
    public interface IInitializable
    {
        [LibraryEntryPoint]
        void Initialize(DiContainer diContainer);
    }
}