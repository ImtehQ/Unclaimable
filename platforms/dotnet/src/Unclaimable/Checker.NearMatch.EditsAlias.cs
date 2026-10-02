namespace Unclaimable;

public sealed partial class Checker
{
    private bool TryMatchObfuscationEditTransitions(
        string source, int si, string target, int ti, bool preserve,
        int maxEdits, int edits, bool requireEnd, HashSet<long> failed,
        out int end)
    {
        if (TryNearMatchEdits(
                source, si, target, ti, preserve,
                maxEdits, edits, requireEnd, failed, out end))
        {
            return true;
        }

        if (TrySkipRepeatedRun(
                source, si, target, ti, preserve,
                maxEdits, edits, requireEnd, failed, out end))
        {
            return true;
        }

        return TryNearMatchTranspose(
            source, si, target, ti, preserve, maxEdits, edits + 1,
            requireEnd, failed, out end);
    }
}
