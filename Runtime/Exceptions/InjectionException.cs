// Copyright (c) 2024 Eric Bennett McDuffee

using System;

namespace UJect.Exceptions
{
    public class InjectionException : InvalidOperationException
    {
        public InjectionException(Type injectedType, string message) : base($"Error injecting {injectedType}: {message}")
        {
        }
    }
}
