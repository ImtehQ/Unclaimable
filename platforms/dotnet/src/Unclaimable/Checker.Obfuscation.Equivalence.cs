namespace Unclaimable;

public sealed partial class Checker
{
    private static bool IsSingleCharacterObfuscationEquivalent(
        char sourceCharacter,
        char targetCharacter,
        ObfuscationSensitivity sensitivity,
        out bool transformed)
    {
        var source = char.ToLowerInvariant(sourceCharacter);
        var target = char.ToLowerInvariant(targetCharacter);

        if (source == target)
        {
            transformed = false;
            return true;
        }

        string[]? substitutions;
        if (ConfusableNormalizer.TryGetObfuscationSubstitutions(source, out substitutions))
        {
            foreach (var substitution in substitutions!)
            {
                if (substitution.Length == 1 && substitution[0] == target)
                {
                    transformed = true;
                    return true;
                }
            }
        }

        if (sensitivity >= ObfuscationSensitivity.Medium
            && IsMediumVisualEquivalent(sourceCharacter, source, target))
        {
            transformed = true;
            return true;
        }

        if (sensitivity >= ObfuscationSensitivity.High
            && IsHighVisualEquivalent(sourceCharacter, target))
        {
            transformed = true;
            return true;
        }

        transformed = false;
        return false;
    }

    private static bool IsCompactObfuscationIgnorable(char character) =>
        !char.IsLetterOrDigit(character) && !char.IsSurrogate(character);
}
