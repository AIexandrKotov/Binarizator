using System;

namespace Binarizator
{
    /// <summary>
    /// Declares that this type can be used as T for Binarizator&lt;T>
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    public sealed class BinarizatorAttribute : Attribute
    {
        public BinarizatorAttribute() { }
    }
    /// <summary>
    /// Indicates reader method for binarizator
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class BinarizatorReaderAttribute : Attribute
    {
        public BinarizatorReaderAttribute() { }
    }
    /// <summary>
    /// Indicates writer method for binarizator
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class BinarizatorWriterAttribute : Attribute
    {
        public BinarizatorWriterAttribute() { }
    }
}
