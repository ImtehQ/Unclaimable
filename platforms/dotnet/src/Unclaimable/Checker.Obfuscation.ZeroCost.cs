namespace Unclaimable;

public sealed partial class Checker
{
    private bool TryMatchObfuscationZeroCostTransitions(
        string source, int sourceIndex, string target, int targetIndex,
        bool preserveNonCompactCharacters, int maxEdits, int editsUsed,
        bool transformed, bool requireSourceEnd, HashSet<long> failed,
        out int endIndex)
    {
        var sourceCharacter = source[sourceIndex];
        var targetCharacter = target[targetIndex];

        if (!preserveNonCompactCharacters
            && IsCompactObfuscationIgnorable(sourceCharacter)
            && TryMatchObfuscationTextCore(
                source, sourceIndex + 1, target, targetIndex,
                preserveNonCompactCharacters, maxEdits, editsUsed,
                transformed, requireSourceEnd, failed, out endIndex))
        {
            return true;
        }

        bool changed;
        if (IsSingleCharacterObfuscationEquivalent(
                sourceCharacter, targetCharacter, _obfuscationSensitivity, out changed)
            && TryMatchObfuscationTextCore(
                source, sourceIndex + 1, target, targetIndex + 1,
                preserveNonCompactCharacters, maxEdits, editsUsed,
                transformed || changed, requireSourceEnd, failed, out endIndex))
        {
            return true;
        }

        int consumed;
        if (_obfuscationSensitivity >= ObfuscationSensitivity.High
            && TryGetMultiCharacterObfuscation(
                source, sourceIndex, targetCharacter, _obfuscationSensitivity, out consumed)
            && TryMatchObfuscationTextCore(
                source, sourceIndex + consumed, target, targetIndex + 1,
                preserveNonCompactCharacters, maxEdits, editsUsed,
                true, requireSourceEnd, failed, out endIndex))
        {
            return true;
        }

        endIndex = -1;
        return false;
    }
}
