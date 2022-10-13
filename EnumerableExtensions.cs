using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    public static T[] ToFlatArray<T>(params object[] enumerableItems)
    {
        return Concat<T>(enumerableItems).ToArray();
    }


    public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] enumerableItems)
    {
        return enumerableItems.SelectMany(x => x);
    }

    public static IEnumerable<T> Concat<T>(params object[] enumerableItems)
    {
        return enumerableItems.SelectMany(x =>
        {
            return x switch
            {
                IEnumerable<T> a => a,
                T c => ToEnumeralbe(c),
                _ => throw new ArgumentException($"type miss match. expect type is [{typeof(T).FullName}]. actual type is [{x.GetType().FullName}]"),
            };
        });
    }

    public static IEnumerable<T> Concat<T>(this T source, IEnumerable<T> values)
    {
        return ToEnumeralbe(source).Concat(values);
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T value)
    {
        return source.Concat(ToEnumeralbe(value));
    }

    public static IEnumerable<T> ToEnumeralbe<T>(this T source)
    {
        yield return source;
    }
}