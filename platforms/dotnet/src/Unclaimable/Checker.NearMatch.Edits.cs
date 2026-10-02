namespace Unclaimable;

public sealed partial class Checker
{
    private bool TryNearMatchEdits(
        string source, int si, string target, int ti, bool preserve,
        int maxEdits, int edits, bool requireEnd, HashSet<long> failed,
        out int end)
    {
        var next = edits + 1;

        if (TryMatchObfuscationTextCore(source, si + 1, target, ti,
            preserve, maxEdits, next, true, requireEnd, failed, out end))
            return true;

        if (TryMatchObfuscationTextCore(source, si, target, ti + 1,
            preserve, maxEdits, next, true, requireEnd, failed, out end))
            return true;

        if (TryMatchObfuscationTextCore(source, si + 1, target, ti + 1,
            preserve, maxEdits, next, true, requireEnd, failed, out end))
            return true;

        end = -1;
        return false;
    }
}
