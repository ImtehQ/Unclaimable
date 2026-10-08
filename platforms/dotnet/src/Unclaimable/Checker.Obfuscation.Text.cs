namespace Unclaimable;

public sealed partial class Checker
{
    private bool TryMatchObfuscationText(
        string source,
        int sourceStart,
        string target,
        bool preserveNonCompactCharacters,
        int maxEdits,
        bool requireSourceEnd,
        HashSet<long> failed,
        out int endIndex)
    {
        failed.Clear();

        return TryMatchObfuscationTextCore(
            source,
            sourceStart,
            target,
            0,
            preserveNonCompactCharacters,
            maxEdits,
            0,
            false,
            requireSourceEnd,
            failed,
            out endIndex);
    }
}
