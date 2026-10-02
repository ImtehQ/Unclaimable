namespace Unclaimable;

public sealed partial class Checker
{
    private bool TryMatchObfuscationTextCore(
        string source, int sourceIndex, string target, int targetIndex,
        bool preserveNonCompactCharacters, int maxEdits, int editsUsed,
        bool transformed, bool requireSourceEnd, HashSet<long> failed,
        out int endIndex)
    {
        var key = GetObfuscationStateKey(sourceIndex, targetIndex, editsUsed, transformed);
        if (failed.Contains(key))
        {
            endIndex = -1;
            return false;
        }

        if (targetIndex == target.Length)
        {
            return TryFinishObfuscationMatch(
                source, sourceIndex, preserveNonCompactCharacters,
                editsUsed, transformed, requireSourceEnd, failed, key, out endIndex);
        }

        if (sourceIndex >= source.Length)
        {
            return TryMatchObfuscationMissingTail(
                source, sourceIndex, target, targetIndex,
                preserveNonCompactCharacters, maxEdits, editsUsed,
                transformed, requireSourceEnd, failed, key, out endIndex);
        }

        if (TryMatchObfuscationZeroCostTransitions(
                source, sourceIndex, target, targetIndex,
                preserveNonCompactCharacters, maxEdits, editsUsed,
                transformed, requireSourceEnd, failed, out endIndex))
        {
            return true;
        }

        if (editsUsed < maxEdits
            && TryMatchObfuscationEditTransitions(
                source, sourceIndex, target, targetIndex,
                preserveNonCompactCharacters, maxEdits, editsUsed,
                transformed, requireSourceEnd, failed, out endIndex))
        {
            return true;
        }

        failed.Add(key);
        endIndex = -1;
        return false;
    }

    private static long GetObfuscationStateKey(
        int sourceIndex, int targetIndex, int editsUsed, bool transformed) =>
        (long)sourceIndex
        | ((long)targetIndex << 8)
        | ((long)editsUsed << 16)
        | (transformed ? 1L << 24 : 0L);
}
