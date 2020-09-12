using System;

namespace Assets.Scripts.DI
{
    /// <summary>
    /// Attribute to mark properties in classes where dependency injection is needed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class InjectionAttribute : Attribute { }
}
