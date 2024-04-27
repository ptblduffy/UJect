// Copyright (c) 2024 Eric Bennett McDuffee

namespace UJect.Factories
{
    public interface IInstanceFactory<TImpl>
    {
        TImpl CreateInstance();
    }
}