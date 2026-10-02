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
        out int endIndex) =>
        TryMatchObfuscationTextCore(
            source,
            sourceStart,
            target,
            0,
            preserveNonCompactCharacters,
            maxEdits,
            0,
            false,
            requireSourceEnd,
            new HashSet<long>(),
            out endIndex);
}
