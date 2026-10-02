namespace Unclaimable;

public sealed partial class Checker
{
    private bool TryMatchObfuscationMissingTail(
        string source, int sourceIndex, string target, int targetIndex,
        bool preserveNonCompactCharacters, int maxEdits, int editsUsed,
        bool transformed, bool requireSourceEnd, HashSet<long> failed,
        long stateKey, out int endIndex)
    {
        if (editsUsed < maxEdits
            && TryMatchObfuscationTextCore(
                source, sourceIndex, target, targetIndex + 1,
                preserveNonCompactCharacters, maxEdits, editsUsed + 1,
                true, requireSourceEnd, failed, out endIndex))
        {
            return true;
        }

        failed.Add(stateKey);
        endIndex = -1;
        return false;
    }
}
