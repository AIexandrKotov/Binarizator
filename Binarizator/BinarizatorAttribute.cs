using System;

namespace Binarizator
{
    /// <summary>
    /// Указывает, что данный тип может использоваться в бинаризаторе
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    public sealed class BinarizatorAttribute : Attribute
    {
        public BinarizatorAttribute() { }
    }
    /// <summary>
    /// Указывает на метод чтения для бинаризатора
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class BinarizatorReaderAttribute : Attribute
    {
        public BinarizatorReaderAttribute() { }
    }
    /// <summary>
    /// Указывает на метод записи для бинаризатора
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class BinarizatorWriterAttribute : Attribute
    {
        public BinarizatorWriterAttribute() { }
    }
}
