// Copyright (c) 2024 Eric Bennett McDuffee
using System;

namespace UJect.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
        internal readonly string CustomId;

        public InjectAttribute(string customId = null)
        {
            CustomId = customId;
        }
    }
}
