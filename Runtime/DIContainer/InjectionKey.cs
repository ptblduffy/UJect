using System;

namespace UJect
{
    internal struct InjectionKey : IComparable<InjectionKey>
    {
        public readonly Type   InjectedResourceType;
        public readonly string InjectedResourceName;

        public InjectionKey(Type injectedResourceType, string injectedResourceName = null)
        {
            InjectedResourceType      = injectedResourceType;
            this.InjectedResourceName = injectedResourceName;
        }

        public bool Equals(InjectionKey other)
        {
            return InjectedResourceType == other.InjectedResourceType && InjectedResourceName == other.InjectedResourceName;
        }

        public override bool Equals(object obj)
        {
            return obj is InjectionKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (InjectedResourceType.GetHashCode() * 397) ^ (InjectedResourceName?.GetHashCode() ?? 0);
            }
        }

        public override string ToString()
        {
            return $"({InjectedResourceType}{(InjectedResourceName != null ? $": \"{InjectedResourceName}\"" : "")})";
        }

        public int CompareTo(InjectionKey other)
        {
            var fullNameCompare = string.Compare(InjectedResourceType.FullName, other.InjectedResourceType.FullName, StringComparison.Ordinal);
            if (fullNameCompare != 0)
            {
                return fullNameCompare;
            }
            return string.Compare(InjectedResourceName, other.InjectedResourceName, StringComparison.Ordinal);
        }
    }
}