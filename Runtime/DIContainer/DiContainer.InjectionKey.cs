using System;

namespace UJect
{
    public sealed partial class DiContainer
    {
        internal struct InjectionKey
        {
            public readonly Type   InjectedResourceType;
            private readonly string injectedResourceName;

            public InjectionKey(Type injectedResourceType, string injectedResourceName)
            {
                this.InjectedResourceType = injectedResourceType;
                this.injectedResourceName = injectedResourceName;
            }
            
            public InjectionKey(Type injectedResourceType)
            {
                this.InjectedResourceType = injectedResourceType;
                this.injectedResourceName = null;
            }

            public bool Equals(InjectionKey other)
            {
                return InjectedResourceType == other.InjectedResourceType && injectedResourceName == other.injectedResourceName;
            }

            public override bool Equals(object obj)
            {
                return obj is InjectionKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (InjectedResourceType.GetHashCode() * 397) ^ (injectedResourceName?.GetHashCode() ?? 0);
                }
            }

            public override string ToString()
            {
                return $"{injectedResourceName}:{InjectedResourceType}";
            }
        }
    }
}