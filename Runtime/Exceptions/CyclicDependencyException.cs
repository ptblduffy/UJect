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