using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;

namespace Binarizator
{
    public static class BinarizatorCommon
    {
        #region integer numerics
        private static
            (Func<BinaryReader, sbyte>, Action<BinaryWriter, sbyte>)
            dr_sbyte = (br => br.ReadSByte(), (bw, o) => bw.Write(o));
        private static
            (Func<BinaryReader, byte>, Action<BinaryWriter, byte>)
            dr_byte = (br => br.ReadByte(), (bw, o) => bw.Write(o));
        private static
            (Func<BinaryReader, char>, Action<BinaryWriter, char>)
            dr_char = (br => br.ReadChar(), (bw, o) => bw.Write(o));
        private static
            (Func<BinaryReader, short>, Action<BinaryWriter, short>)
            dr_short = (br => br.ReadInt16(), (bw, o) => bw.Write(o));
        private static
            (Func<BinaryReader, ushort>, Action<BinaryWriter, ushort>)
            dr_ushort = (br => br.ReadUInt16(), (bw, o) => bw.Write(o));
        private static
            (Func<BinaryReader, int>, Action<BinaryWriter, int>)
            dr_int = (br => br.ReadInt32(), (bw, o) => bw.Write(o));
        private static
            (Func<BinaryReader, uint>, Action<BinaryWriter, uint>)
            dr_uint = (br => br.ReadUInt32(), (bw, o) => bw.Write(o));
        private static
            (Func<BinaryReader, long>, Action<BinaryWriter, long>)
            dr_long = (br => br.ReadInt64(), (bw, o) => bw.Write(o));
        private static
            (Func<BinaryReader, ulong>, Action<BinaryWriter, ulong>)
            dr_ulong = (br => br.ReadUInt64(), (bw, o) => bw.Write(o));
        #endregion

        #region float numerics
        private static
            (Func<BinaryReader, float>, Action<BinaryWriter, float>)
            dr_float = (br => br.ReadSingle(), (bw, o) => bw.Write(o));
        private static
            (Func<BinaryReader, double>, Action<BinaryWriter, double>)
            dr_double = (br => br.ReadDouble(), (bw, o) => bw.Write(o));
        private static
            (Func<BinaryReader, decimal>, Action<BinaryWriter, decimal>)
            dr_decimal = (br => br.ReadDecimal(), (bw, o) => bw.Write(o));
        #endregion

        #region string
        private static Encoding Encoding = Encoding.Unicode;
        private static string ReadString(BinaryReader br)
        {
            var count = br.ReadInt32();
            return Encoding.GetString(br.ReadBytes(count));
        }
        private static void WriteString(BinaryWriter bw, string s)
        {
            var bytes = Encoding.GetBytes(s);
            bw.Write(bytes.Length);
            bw.Write(bytes);
        }
        private static
            (Func<BinaryReader, string>, Action<BinaryWriter, string>)
            dr_string = (ReadString, WriteString);
        #endregion

        private static
            Dictionary<Type, (Delegate, Delegate)> delegates = new Dictionary<Type, (Delegate, Delegate)>();

        /// <summary>
        /// Определить методы чтения и записи для типа T
        /// </summary>
        public static void MakeForType<T>(Func<BinaryReader, T> reader, Action<BinaryWriter, T> writer)
        {
            delegates.Add(typeof(T), (reader, writer));
        }

        /// <summary>
        /// Определить методы чтения и записи для типа type
        /// </summary>
        /// <param name="reader">Делегает типа Func&lt;BinaryReader, type></param>
        /// <param name="writer">Делегает типа Action&lt;BinaryWriter, type></param>
        public static void MakeForType(Type type, Delegate reader, Delegate writer)
        {
            delegates.Add(type, (reader, writer));
        }

        private static T CreateDelegate<T>(this MethodInfo methodInfo) where T: Delegate
        {
            return (T)methodInfo.CreateDelegate(typeof(T));
        }

        internal static (Func<BinaryReader, T>, Action<BinaryWriter, T>) GetInterface<T>()
        {
            var type = typeof(T);

            if (delegates.TryGetValue(type, out var ret))
            {
                return ((Func<BinaryReader, T>)ret.Item1, (Action<BinaryWriter, T>)ret.Item2);
            }
            if (type.IsArray)
            {
                var array_element_type = type.GetElementType();
                var array_element_methods = typeof(Binarizator<>).MakeGenericType(new Type[1] { array_element_type }).GetMethods(BindingFlags.Public | BindingFlags.Static);
                var reader = array_element_methods.Where(x => x.Name == "ReadArray").FirstOrDefault().CreateDelegate<Func<BinaryReader, T>>();
                var writer = array_element_methods.Where(x => x.Name == "WriteArray").FirstOrDefault().CreateDelegate<Action<BinaryWriter, T>>();
                return (reader, writer);
            }
            if (type.IsEnum)
            {
                var enum_element_type = type.GetEnumUnderlyingType();
                var enum_element_methods = typeof(Binarizator<>).MakeGenericType(enum_element_type).GetMethods(BindingFlags.Public | BindingFlags.Static);
                var enum_element_reader = enum_element_methods.Where(x => x.Name == "ReadValue").FirstOrDefault();
                var enum_element_writer = enum_element_methods.Where(x => x.Name == "WriteValue").FirstOrDefault();

                var reader = new DynamicMethod("ReadEnum", type, new Type[1] { typeof(BinaryReader) }, true);
                var reader_il = reader.GetILGenerator();
                reader_il.DeclareLocal(enum_element_type);

                reader_il.Emit(OpCodes.Ldarg_0);
                reader_il.Emit(OpCodes.Call, enum_element_reader);
                reader_il.Emit(OpCodes.Ret);

                var writer = new DynamicMethod("WriteEnum", typeof(void), new Type[2] { typeof(BinaryWriter), enum_element_type }, true);
                var writer_il = writer.GetILGenerator();

                writer_il.Emit(OpCodes.Ldarg_0);
                writer_il.Emit(OpCodes.Ldarg_1);
                writer_il.Emit(OpCodes.Call, enum_element_writer);
                writer_il.Emit(OpCodes.Ret);

                return (reader.CreateDelegate<Func<BinaryReader, T>>(), writer.CreateDelegate<Action<BinaryWriter, T>>());
            }
            if (type.IsGenericType)
            {
                var generic_type = type.GetGenericTypeDefinition();
                if (generic_type == typeof(List<>))
                {
                    var list_element_type = type.GetGenericArguments()[0];
                    var list_element_methods = typeof(Binarizator<>).MakeGenericType(new Type[1] { list_element_type }).GetMethods(BindingFlags.Public | BindingFlags.Static);
                    var reader = list_element_methods.Where(x => x.Name == "ReadList").FirstOrDefault().CreateDelegate<Func<BinaryReader, T>>();
                    var writer = list_element_methods.Where(x => x.Name == "WriteList").FirstOrDefault().CreateDelegate<Action<BinaryWriter, T>>();
                    return (reader, writer);
                }
                else if (generic_type == typeof(Dictionary<,>))
                {
                    var dictionary_elements_type = type.GetGenericArguments();
                    var dictionary_element_methods = typeof(BinarizatorExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static);
                    var reader = dictionary_element_methods.Where(x => x.Name == "ReadDictionary").FirstOrDefault().MakeGenericMethod(dictionary_elements_type).CreateDelegate<Func<BinaryReader, T>>();
                    var writer = dictionary_element_methods.Where(x => x.Name == "WriteDictionary").FirstOrDefault().MakeGenericMethod(dictionary_elements_type).CreateDelegate<Action<BinaryWriter, T>>();
                    return (reader, writer);
                }

                var generic_interfaces = generic_type.GetInterfaces().Where(x => x.IsConstructedGenericType).Select(x => x.GetGenericTypeDefinition());
                if (generic_type == typeof(IEnumerable<>) || generic_interfaces.Contains(typeof(IEnumerable<>)))
                {
                    var enumerable_element_type = type.GetGenericArguments()[0];
                    var enumerable_element_methods = typeof(Binarizator<>).MakeGenericType(new Type[1] { enumerable_element_type }).GetMethods(BindingFlags.Public | BindingFlags.Static);
                    var reader = enumerable_element_methods.Where(x => x.Name == "ReadEnumerable" && x.GetParameters().Length == 1).FirstOrDefault().CreateDelegate<Func<BinaryReader, T>>();
                    var writer = enumerable_element_methods.Where(x => x.Name == "WriteEnumerable").FirstOrDefault().CreateDelegate<Action<BinaryWriter, T>>();
                    return (reader, writer);
                }

                if (generic_type.Name.StartsWith("ValueTuple`"))
                {
                    var tuple_elements_type = type.GetGenericArguments();
                    var tuple_elements_count = tuple_elements_type.Length;
                    var tuple_element_methods = typeof(BinarizatorExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static);
                    var reader = tuple_element_methods.Where(x => x.Name == "ReadTuple" && x.GetGenericArguments().Length == tuple_elements_count).FirstOrDefault().MakeGenericMethod(tuple_elements_type).CreateDelegate<Func<BinaryReader, T>>();
                    var writer = tuple_element_methods.Where(x => x.Name == "WriteTuple" && x.GetGenericArguments().Length == tuple_elements_count).FirstOrDefault().MakeGenericMethod(tuple_elements_type).CreateDelegate<Action<BinaryWriter, T>>();
                    return (reader, writer);
                }
            }
            if (type.GetCustomAttribute<BinarizatorAttribute>() != null)
            {
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var reader = methods.Where(x =>
                {
                    var parameters = x.GetParameters();
                    var has_attribute = x.GetCustomAttribute<BinarizatorReaderAttribute>() != null;
                    return has_attribute && parameters.Length == 1 && parameters[0].ParameterType == typeof(BinaryReader) && x.ReturnType == typeof(T);
                }).FirstOrDefault();
                var writer = methods.Where(x =>
                {
                    var parameters = x.GetParameters();
                    var has_attribute = x.GetCustomAttribute<BinarizatorWriterAttribute>() != null;
                    return has_attribute && parameters.Length == 2 && parameters[0].ParameterType == typeof(BinaryWriter) && parameters[1].ParameterType == typeof(T) && x.ReturnType == typeof(void);
                }).FirstOrDefault();

                if (reader == null)
                    throw new ArgumentException($"`static {type.Name} reader(BinaryReader br)` not found");
                if (writer == null)
                    throw new ArgumentException($"`static void writer(BinaryWriter bw, {type.Name} o)` not found");

                return (reader.CreateDelegate<Func<BinaryReader, T>>(), writer.CreateDelegate<Action<BinaryWriter, T>>());
            }
            
            throw new ArgumentException($"Read-Write interface not found for `{type}`");
        }
        
        static BinarizatorCommon()
        {
            foreach (var dd in typeof(BinarizatorCommon).GetFields(BindingFlags.NonPublic | BindingFlags.Static).Where(x => x.Name.StartsWith("dr_")).Select(x => x.GetValue(null)))
            {
                var type = ((ITuple)dd)[0].GetType().GetGenericArguments()[1];
                delegates[type] = ((Delegate)((ITuple)dd)[0], (Delegate)((ITuple)dd)[1]);
            }
        }
    }
}
