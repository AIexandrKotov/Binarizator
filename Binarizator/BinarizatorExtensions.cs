﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Binarizator
{
    public static class BinarizatorExtensions
    {
        /// <summary>
        /// Записывает значение в поток
        /// </summary>
        public static void WriteValue<T>(this BinaryWriter bw, T value) => Binarizator<T>.WriteValue(bw, value);
        /// <summary>
        /// Читает значение из потока
        /// </summary>
        public static T ReadValue<T>(this BinaryReader br) => Binarizator<T>.ReadValue(br);

        /// <summary>
        /// Записывает массив значений в поток
        /// </summary>
        public static void WriteArray<T>(this BinaryWriter bw, T[] array) => Binarizator<T>.WriteArray(bw, array);
        /// <summary>
        /// Читает массив значений из потока
        /// </summary>
        public static T[] ReadArray<T>(this BinaryReader br) => Binarizator<T>.ReadArray(br);

        /// <summary>
        /// Записывает список значений в поток
        /// </summary>
        public static void WriteList<T>(this BinaryWriter bw, IList<T> list) => Binarizator<T>.WriteList(bw, list);
        /// <summary>
        /// Читает список значений из потока
        /// </summary>
        public static List<T> ReadList<T>(this BinaryReader br) => Binarizator<T>.ReadList(br);

        /// <summary>
        /// Записывает словарь в поток
        /// </summary>
        public static void WriteDictionary<TKey, TValue>(this BinaryWriter bw, IDictionary<TKey, TValue> dictionary)
        {
            bw.Write(dictionary.Count);
            foreach (var kv in dictionary)
            {
                Binarizator<TKey>.WriteValue(bw, kv.Key);
                Binarizator<TValue>.WriteValue(bw, kv.Value);
            }
        }
        /// <summary>
        /// Читает словарь из потока
        /// </summary>
        public static Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(this BinaryReader br)
        {
            var count = br.ReadInt32();
            var ret = new Dictionary<TKey, TValue>(count);
            for (var i = 0; i < count; i++)
                ret.Add(Binarizator<TKey>.ReadValue(br), Binarizator<TValue>.ReadValue(br));
            return ret;
        }

        /// <summary>
        /// Записывает последовательность элементов в поток
        /// </summary>
        public static void WriteEnumerable<T>(this BinaryWriter bw, IEnumerable<T> values) => Binarizator<T>.WriteEnumerable(bw, values);
        /// <summary>
        /// Читает последовательность из count элементов из потока
        /// </summary>
        public static IEnumerable<T> ReadEnumerable<T>(this BinaryReader br, int count) => Binarizator<T>.ReadEnumerable(br, count);
        /// <summary>
        /// Читает последовательность из потока
        /// </summary>
        public static IEnumerable<T> ReadEnumerable<T>(this BinaryReader br) => Binarizator<T>.ReadEnumerable(br);

        /// <summary>
        /// Записывает кортеж в поток
        /// </summary>
        public static void WriteTuple<T1>(this BinaryWriter bw, ValueTuple<T1> tuple)
        {
            Binarizator<T1>.WriteValue(bw, tuple.Item1);
        }
        /// <summary>
        /// Записывает кортеж в поток
        /// </summary>
        public static void WriteTuple<T1, T2>(this BinaryWriter bw, ValueTuple<T1, T2> tuple)
        {
            Binarizator<T1>.WriteValue(bw, tuple.Item1);
            Binarizator<T2>.WriteValue(bw, tuple.Item2);
        }
        /// <summary>
        /// Записывает кортеж в поток
        /// </summary>
        public static void WriteTuple<T1, T2, T3>(this BinaryWriter bw, ValueTuple<T1, T2, T3> tuple)
        {
            Binarizator<T1>.WriteValue(bw, tuple.Item1);
            Binarizator<T2>.WriteValue(bw, tuple.Item2);
            Binarizator<T3>.WriteValue(bw, tuple.Item3);
        }
        /// <summary>
        /// Записывает кортеж в поток
        /// </summary>
        public static void WriteTuple<T1, T2, T3, T4>(this BinaryWriter bw, ValueTuple<T1, T2, T3, T4> tuple)
        {
            Binarizator<T1>.WriteValue(bw, tuple.Item1);
            Binarizator<T2>.WriteValue(bw, tuple.Item2);
            Binarizator<T3>.WriteValue(bw, tuple.Item3);
            Binarizator<T4>.WriteValue(bw, tuple.Item4);
        }
        /// <summary>
        /// Записывает кортеж в поток
        /// </summary>
        public static void WriteTuple<T1, T2, T3, T4, T5>(this BinaryWriter bw, ValueTuple<T1, T2, T3, T4, T5> tuple)
        {
            Binarizator<T1>.WriteValue(bw, tuple.Item1);
            Binarizator<T2>.WriteValue(bw, tuple.Item2);
            Binarizator<T3>.WriteValue(bw, tuple.Item3);
            Binarizator<T4>.WriteValue(bw, tuple.Item4);
            Binarizator<T5>.WriteValue(bw, tuple.Item5);
        }
        /// <summary>
        /// Записывает кортеж в поток
        /// </summary>
        public static void WriteTuple<T1, T2, T3, T4, T5, T6>(this BinaryWriter bw, ValueTuple<T1, T2, T3, T4, T5, T6> tuple)
        {
            Binarizator<T1>.WriteValue(bw, tuple.Item1);
            Binarizator<T2>.WriteValue(bw, tuple.Item2);
            Binarizator<T3>.WriteValue(bw, tuple.Item3);
            Binarizator<T4>.WriteValue(bw, tuple.Item4);
            Binarizator<T5>.WriteValue(bw, tuple.Item5);
            Binarizator<T6>.WriteValue(bw, tuple.Item6);
        }
        /// <summary>
        /// Записывает кортеж в поток
        /// </summary>
        public static void WriteTuple<T1, T2, T3, T4, T5, T6, T7>(this BinaryWriter bw, ValueTuple<T1, T2, T3, T4, T5, T6, T7> tuple)
        {
            Binarizator<T1>.WriteValue(bw, tuple.Item1);
            Binarizator<T2>.WriteValue(bw, tuple.Item2);
            Binarizator<T3>.WriteValue(bw, tuple.Item3);
            Binarizator<T4>.WriteValue(bw, tuple.Item4);
            Binarizator<T5>.WriteValue(bw, tuple.Item5);
            Binarizator<T6>.WriteValue(bw, tuple.Item6);
            Binarizator<T7>.WriteValue(bw, tuple.Item7);
        }
        /// <summary>
        /// Записывает кортеж в поток
        /// </summary>
        public static void WriteTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(this BinaryWriter bw, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple) where TRest : struct
        {
            Binarizator<T1>.WriteValue(bw, tuple.Item1);
            Binarizator<T2>.WriteValue(bw, tuple.Item2);
            Binarizator<T3>.WriteValue(bw, tuple.Item3);
            Binarizator<T4>.WriteValue(bw, tuple.Item4);
            Binarizator<T5>.WriteValue(bw, tuple.Item5);
            Binarizator<T6>.WriteValue(bw, tuple.Item6);
            Binarizator<T7>.WriteValue(bw, tuple.Item7);
            Binarizator<TRest>.WriteValue(bw, tuple.Rest);
        }
        /// <summary>
        /// Читает кортеж из потока
        /// </summary>
        public static ValueTuple<T1> ReadTuple<T1>(this BinaryReader br)
        {
            return new ValueTuple<T1>(Binarizator<T1>.ReadValue(br));
        }
        /// <summary>
        /// Читает кортеж из потока
        /// </summary>
        public static ValueTuple<T1, T2> ReadTuple<T1, T2>(this BinaryReader br)
        {
            return new ValueTuple<T1, T2>(
                Binarizator<T1>.ReadValue(br),
                Binarizator<T2>.ReadValue(br));
        }
        /// <summary>
        /// Читает кортеж из потока
        /// </summary>
        public static ValueTuple<T1, T2, T3> ReadTuple<T1, T2, T3>(this BinaryReader br)
        {
            return new ValueTuple<T1, T2, T3>(
                Binarizator<T1>.ReadValue(br),
                Binarizator<T2>.ReadValue(br),
                Binarizator<T3>.ReadValue(br));
        }
        /// <summary>
        /// Читает кортеж из потока
        /// </summary>
        public static ValueTuple<T1, T2, T3, T4> ReadTuple<T1, T2, T3, T4>(this BinaryReader br)
        {
            return new ValueTuple<T1, T2, T3, T4>(
                Binarizator<T1>.ReadValue(br),
                Binarizator<T2>.ReadValue(br),
                Binarizator<T3>.ReadValue(br),
                Binarizator<T4>.ReadValue(br));
        }
        /// <summary>
        /// Читает кортеж из потока
        /// </summary>
        public static ValueTuple<T1, T2, T3, T4, T5> ReadTuple<T1, T2, T3, T4, T5>(this BinaryReader br)
        {
            return new ValueTuple<T1, T2, T3, T4, T5>(
                Binarizator<T1>.ReadValue(br),
                Binarizator<T2>.ReadValue(br),
                Binarizator<T3>.ReadValue(br),
                Binarizator<T4>.ReadValue(br),
                Binarizator<T5>.ReadValue(br));
        }
        /// <summary>
        /// Читает кортеж из потока
        /// </summary>
        public static ValueTuple<T1, T2, T3, T4, T5, T6> ReadTuple<T1, T2, T3, T4, T5, T6>(this BinaryReader br)
        {
            return new ValueTuple<T1, T2, T3, T4, T5, T6>(
                Binarizator<T1>.ReadValue(br),
                Binarizator<T2>.ReadValue(br),
                Binarizator<T3>.ReadValue(br),
                Binarizator<T4>.ReadValue(br),
                Binarizator<T5>.ReadValue(br),
                Binarizator<T6>.ReadValue(br));
        }
        /// <summary>
        /// Читает кортеж из потока
        /// </summary>
        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> ReadTuple<T1, T2, T3, T4, T5, T6, T7>(this BinaryReader br)
        {
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(
                Binarizator<T1>.ReadValue(br),
                Binarizator<T2>.ReadValue(br),
                Binarizator<T3>.ReadValue(br),
                Binarizator<T4>.ReadValue(br),
                Binarizator<T5>.ReadValue(br),
                Binarizator<T6>.ReadValue(br),
                Binarizator<T7>.ReadValue(br));
        }
        /// <summary>
        /// Читает кортеж из потока
        /// </summary>
        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> ReadTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(this BinaryReader br) where TRest: struct
        {
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
                Binarizator<T1>.ReadValue(br),
                Binarizator<T2>.ReadValue(br),
                Binarizator<T3>.ReadValue(br),
                Binarizator<T4>.ReadValue(br),
                Binarizator<T5>.ReadValue(br),
                Binarizator<T6>.ReadValue(br),
                Binarizator<T7>.ReadValue(br),
                Binarizator<TRest>.ReadValue(br));
        }
    }
}
