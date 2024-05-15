// Copyright (c) 2024 Eric Bennett McDuffee
using UJect.Utilities;

namespace UJect
{
    public interface IInitializable
    {
        [LibraryEntryPoint]
        void Initialize(DiContainer diContainer);
    }
}
