# Binarizator

A simple library that adds generic extensions for binary read-write

## Supported types

Completely copies the basic behavior for the following types:
- sbyte, byte, char, short, ushort, int, uint, long, ulong
- float, double, decimal
- byte[]

Own behaviour:
- string (read-writes as UTF-16 bytes)

Extensions:
- enum
- ValueTuples, T[], List&lt;T>, Dictionary&lt;TKey, TValue>, IEnumerable&lt;T>

And also you can set behavior for your own types and even for other built-in types.

## Example
```csharp
internal class Program
{
    public enum Signal
    {
        Positive = -1, Neutral = 0, Negative = 1
    }

    [Binarizator]
    public struct SignalSet
    {
        public Signal Signal;
        public double Frequency;
        public (long, long) Repeats;

        public SignalSet(Signal signal, double frequency, (long, long) repeats)
        {
            Signal = signal;
            Frequency = frequency;
            Repeats = repeats;
        }

        [BinarizatorReader]
        public static SignalSet Read(BinaryReader br)
        {
            return new SignalSet(br.ReadValue<Signal>(), br.ReadValue<double>(), br.ReadValue<(long, long)>());
        }

        [BinarizatorWriter]
        public static void Write(BinaryWriter bw, SignalSet signal)
        {
            bw.WriteValue(signal.Signal);
            bw.WriteValue(signal.Frequency);
            bw.WriteValue(signal.Repeats);
        }

        public override string ToString()
        {
            return $"({Signal}, {Frequency}, {Repeats.Item1}..{Repeats.Item2})";
        }
    }

    static void Main(string[] args)
    {
        var ms = new MemoryStream();
        var bw = new BinaryWriter(ms);

        bw.WriteValue(new byte[] { 1, 2, 3, 4 });
        bw.WriteValue(new SignalSet[]{
            new SignalSet(Signal.Positive, 22.8, (1, 4)),
            new SignalSet(Signal.Negative, 324, (-10, 10)),
            new SignalSet(Signal.Neutral, double.NaN, (long.MinValue, long.MaxValue)),
        });

        ms.Position = 0;
        var br = new BinaryReader(ms);
        Console.WriteLine(string.Join(" ", br.ReadValue<byte[]>()));
        Console.WriteLine(string.Join("\n", br.ReadValue<IEnumerable<SignalSet>>()));

        Console.ReadLine();
    }
}
```