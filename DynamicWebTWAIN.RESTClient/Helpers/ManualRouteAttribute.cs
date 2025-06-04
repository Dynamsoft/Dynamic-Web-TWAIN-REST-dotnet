using System;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Attribute to indicate that a method is a manual route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ManualRouteAttribute : Attribute
    {
        public string Verb { get; private set; }
        public string Path { get; private set; }

        public ManualRouteAttribute(string verb, string path)
        {
            this.Verb = verb;
            this.Path = path;
        }
    }

    /// <summary>
    /// Attribute to indicate that a method is a generated route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class GeneratedRouteAttribute : Attribute
    {
        public string Verb { get; private set; }
        public string Path { get; private set; }

        public GeneratedRouteAttribute(string verb, string path)
        {
            this.Verb = verb;
            this.Path = path;
        }
    }

    /// <summary>
    /// Attribute to indicate that a method is a .NET specific route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DotNetSpecificRouteAttribute : Attribute
    {
    }
}
