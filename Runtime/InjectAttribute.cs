using System;

namespace UJect
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class InjectAttribute : Attribute
    {
        internal readonly string CustomId;
        public InjectAttribute(string customId=null)
        {
            CustomId = customId;
        }
    }
}