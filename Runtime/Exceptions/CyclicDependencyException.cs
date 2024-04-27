// Copyright (c) 2024 Eric Bennett McDuffee

using System;

namespace UJect.Exceptions
{
    public class CyclicDependencyException : InvalidOperationException
    {
        internal CyclicDependencyException(string message): base(message)
        {
        }
    }
}