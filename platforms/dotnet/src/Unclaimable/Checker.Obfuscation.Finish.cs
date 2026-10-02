namespace Unclaimable;

public sealed partial class Checker
{
    private static bool TryFinishObfuscationMatch(
        string source,
        int sourceIndex,
        bool preserveNonCompactCharacters,
        int editsUsed,
        bool transformed,
        bool requireSourceEnd,
        HashSet<long> failed,
        long stateKey,
        out int endIndex)
    {
        var end = sourceIndex;
        if (requireSourceEnd)
        {
            while (end < source.Length
                   && !preserveNonCompactCharacters
                   && IsCompactObfuscationIgnorable(source[end]))
            {
                end++;
            }

            if (end != source.Length)
            {
                failed.Add(stateKey);
                endIndex = -1;
                return false;
            }
        }

        if (transformed || editsUsed > 0)
        {
            endIndex = end;
            return true;
        }

        failed.Add(stateKey);
        endIndex = -1;
        return false;
    }
}
