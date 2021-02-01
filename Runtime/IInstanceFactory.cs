using System;

namespace UJect
{
    public interface IInstanceFactory<TImpl> : IDisposable
    {
        TImpl CreateInstance();
    }
}