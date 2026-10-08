namespace Unclaimable;

public sealed partial class Checker
{
    private bool TryMatchZeroCostObfuscation(
        string value,
        ObfuscationIndex index,
        bool preserveNonCompactCharacters,
        out ReservedEntry? match,
        out MatchKind kind,
        out int? start,
        out int? length)
    {
        var entries = preserveNonCompactCharacters
            ? index.ExactEntries
            : index.CompactEntries;

        foreach (var pair in entries)
        {
            if (!CanObfuscationLengthsMatch(
                    value, pair.Key.Length, preserveNonCompactCharacters, 0, directPotential: true))
            {
                continue;
            }

            int end;
            if (!TryMatchObfuscationText(
                    value, 0, pair.Key, preserveNonCompactCharacters, 0, true, out end))
            {
                continue;
            }

            match = pair.Value;
            kind = MatchKind.Obfuscated;
            start = 0;
            length = end;
            return true;
        }

        if (_partialMatching)
        {
            foreach (var partial in index.PartialEntries)
            {
                var target = preserveNonCompactCharacters ? partial.Exact : partial.Compact;
                if (value.Length <= target.Length)
                {
                    continue;
                }

                for (var sourceStart = 0; sourceStart < value.Length; sourceStart++)
                {
                    int end;
                    if (!TryMatchObfuscationText(
                            value, sourceStart, target,
                            preserveNonCompactCharacters, 0, false, out end))
                    {
                        continue;
                    }

                    match = partial.Entry;
                    kind = MatchKind.Partial;
                    start = sourceStart;
                    length = Math.Max(1, end - sourceStart);
                    return true;
                }
            }
        }

        match = null;
        kind = MatchKind.None;
        start = null;
        length = null;
        return false;
    }
}
