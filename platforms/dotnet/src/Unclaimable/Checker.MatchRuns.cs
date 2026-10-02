namespace Unclaimable;

public sealed partial class Checker
{
    private bool TrySkipRepeatedRun(
        string source,
        int sourceIndex,
        string target,
        int targetIndex,
        bool preserve,
        int maxEdits,
        int edits,
        bool requireEnd,
        HashSet<long> failed,
        out int end)
    {
        if (_obfuscationSensitivity < ObfuscationSensitivity.High
            || sourceIndex + 1 >= source.Length)
        {
            end = -1;
            return false;
        }

        var character = source[sourceIndex];
        var next = sourceIndex + 1;
        while (next < source.Length && source[next] == character)
        {
            next++;
        }

        if (next - sourceIndex < 2)
        {
            end = -1;
            return false;
        }

        return TryMatchObfuscationTextCore(
            source, next, target, targetIndex,
            preserve, maxEdits, edits + 1, true,
            requireEnd, failed, out end);
    }
}
