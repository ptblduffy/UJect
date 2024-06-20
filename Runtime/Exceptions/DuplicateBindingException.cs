// Copyright (c) 2024 Eric Bennett McDuffee

using System;
using System.Linq;

namespace UJect.Exceptions
{
    public class DuplicateBindingException : ArgumentException
    {
        internal DuplicateBindingException(InjectionKey injectionKey) : base($"Dependency already bound for type \"{injectionKey.InjectedResourceType}\" with custom ID: \"{injectionKey.InjectedResourceName}\"")
        {
            
        }
        
        internal DuplicateBindingException(InjectionKey[] injectionKeys) : base(InjectionKeysString(injectionKeys))
        {
            
        }

        private static string InjectionKeysString(InjectionKey[] keys)
        {
            var typesString = string.Join(", ", keys.Select(k => k.InjectedResourceType.ToString()));
            var customId = keys[0].InjectedResourceName;
            return $"Dependency already bound for types \"{typesString}\" with custom ID: \"{customId}\"";
        }
    }
}
