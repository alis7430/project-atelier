using System;
using System.Collections.Generic;

/// <summary>
/// IList Shuffling extension
/// Using Fisher–Yates Shuffle
/// </summary>
public static class ShufflingExtension
{
    public static void Shuffle<T>(this IList<T> list, IRandom random)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        if (random == null)
            throw new ArgumentNullException(nameof(random));

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Range(0, i + 1); // automatically call random.next()

            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static T RandomElement<T>(this IList<T> list, IRandom random)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("List is null or empty.");

        if (random == null)
            throw new ArgumentNullException(nameof(random));

        return list[random.Range(0, list.Count)];
    }
}