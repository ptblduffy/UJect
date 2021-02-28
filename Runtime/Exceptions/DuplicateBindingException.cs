using System;

namespace UJect.Exceptions
{
    public class DuplicateBindingException : ArgumentException
    {
        internal DuplicateBindingException(InjectionKey injectionKey) : base($"Dependency already bound for type \"{injectionKey.InjectedResourceType}\" with custom ID: \"{injectionKey.InjectedResourceName}\"")
        {
            
        }
    }
}