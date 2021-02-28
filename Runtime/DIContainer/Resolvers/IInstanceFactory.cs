using System;

namespace UJect.Factories
{
    public interface IInstanceFactory<TImpl>
    {
        TImpl CreateInstance();
    }
}