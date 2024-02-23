using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Binarizator
{

    /// <summary>
    /// Класс, дающий более удобный интерфейс для бинарного чтения и записи
    /// </summary>
    /// <typeparam name="T">Поддерживаются: все базовые числовые типы, строки, одноранговые массивы, списки, словари и последовательности. Поддерживается вложенность типов. Используйте атрибут BinarizatorAttribute для определения методов чтения и записи для пользовательских классов. Используйте BinarizatorCommon.MakeForType для того же для встроенных типов</typeparam>
    public static class Binarizator<T>
    {
        static Binarizator()
        {
            (reader, writer) = BinarizatorCommon.GetInterface<T>();
        }

        private static Func<BinaryReader, T> reader;
        private static Action<BinaryWriter, T> writer;

        /// <summary>
        /// Записывает значение в поток
        /// </summary>
        public static void WriteValue(BinaryWriter bw, T value)
        {
            writer(bw, value);
        }
        /// <summary>
        /// Записывает массив значений в поток
        /// </summary>
        public static void WriteArray(BinaryWriter bw, T[] values)
        {
            bw.Write(values.Length);
            for (var i = 0; i < values.Length; i++)
                writer(bw, values[i]);
        }
        /// <summary>
        /// Записывает список значений в поток
        /// </summary>
        public static void WriteList(BinaryWriter bw, IList<T> values)
        {
            bw.Write(values.Count);
            for (var i = 0; i < values.Count; i++)
                writer(bw, values[i]);
        }
        /// <summary>
        /// Записывает последовательность значений в поток
        /// </summary>
        public static void WriteEnumerable(BinaryWriter bw, IEnumerable<T> values)
        {
            if (bw.BaseStream.CanSeek)
            {
                var count = 0;
                var countposition = bw.BaseStream.Position;
                bw.Write(0);
                foreach (var x in values)
                {
                    WriteValue(bw, x);
                    count++;
                }
                var newposition = bw.BaseStream.Position;
                bw.BaseStream.Position = countposition;
                bw.Write(count);
                bw.BaseStream.Position = newposition;
            }
            else
            {
                bw.Write(values.Count());
                foreach (var x in values)
                    WriteValue(bw, x);
            }
        }
        /// <summary>
        /// Читает значение из потока
        /// </summary>
        public static T ReadValue(BinaryReader br)
        {
            return reader(br);
        }
        /// <summary>
        /// Читает массив значений из потока
        /// </summary>
        public static T[] ReadArray(BinaryReader br)
        {
            var count = br.ReadInt32();
            var ret = new T[count];
            for (var i = 0; i < count; i++)
                ret[i] = reader(br);
            return ret;
        }
        /// <summary>
        /// Читает список значений из потока
        /// </summary>
        public static List<T> ReadList(BinaryReader br)
        {
            var count = br.ReadInt32();
            var ret = new List<T>(count);
            for (var i = 0; i < count; i++)
                ret.Add(reader(br));
            return ret;
        }
        /// <summary>
        /// Читает последовательность из потока
        /// </summary>
        public static IEnumerable<T> ReadEnumerable(BinaryReader br) => ReadEnumerable(br, br.ReadInt32());
        /// <summary>
        /// Читает последовательность из count элементов из потока
        /// </summary>
        public static IEnumerable<T> ReadEnumerable(BinaryReader br, int count)
        {
            for (var i = 0; i < count; i++)
                yield return reader(br);
        }
    }
}
