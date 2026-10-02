namespace Unclaimable;

public sealed partial class Checker
{
    private bool TryNearMatchTranspose(
        string source, int si, string target, int ti, bool preserve,
        int maxEdits, int nextEdits, bool requireEnd, HashSet<long> failed,
        out int end)
    {
        if (si + 1 >= source.Length || ti + 1 >= target.Length)
        {
            end = -1;
            return false;
        }

        bool firstChanged;
        bool secondChanged;
        if (!IsSingleCharacterObfuscationEquivalent(
                source[si], target[ti + 1], _obfuscationSensitivity, out firstChanged)
            || !IsSingleCharacterObfuscationEquivalent(
                source[si + 1], target[ti], _obfuscationSensitivity, out secondChanged))
        {
            end = -1;
            return false;
        }

        return TryMatchObfuscationTextCore(
            source, si + 2, target, ti + 2,
            preserve, maxEdits, nextEdits, true,
            requireEnd, failed, out end);
    }
}
