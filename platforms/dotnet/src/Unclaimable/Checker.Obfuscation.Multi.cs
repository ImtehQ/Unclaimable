namespace Unclaimable;

public sealed partial class Checker
{
    private static bool TryGetMultiCharacterObfuscation(
        string source,
        int index,
        char target,
        ObfuscationSensitivity sensitivity,
        out int consumed)
    {
        if (sensitivity >= ObfuscationSensitivity.High)
        {
            if (MatchesObfuscationSequence(source, index, "rn") && target == 'm'
                || MatchesObfuscationSequence(source, index, "vv") && target == 'w'
                || MatchesObfuscationSequence(source, index, "cl") && target == 'd')
            {
                consumed = 2;
                return true;
            }
        }

        if (sensitivity >= ObfuscationSensitivity.Extreme
            && TryMatchExtremeSymbolSequence(source, index, target, out consumed))
        {
            return true;
        }

        consumed = 0;
        return false;
    }

    private static bool MatchesObfuscationSequence(string value, int index, string sequence)
    {
        if (index < 0 || index + sequence.Length > value.Length)
        {
            return false;
        }

        for (var offset = 0; offset < sequence.Length; offset++)
        {
            if (value[index + offset] != sequence[offset])
            {
                return false;
            }
        }

        return true;
    }
}
