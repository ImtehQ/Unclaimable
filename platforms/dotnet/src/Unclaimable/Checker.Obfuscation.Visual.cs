namespace Unclaimable;

public sealed partial class Checker
{
    private string NormalizeCaseAwareObfuscationInput(string? originalValue, string normalizedValue)
    {
        if (_obfuscationSensitivity < ObfuscationSensitivity.Medium || originalValue is null)
        {
            return normalizedValue;
        }

        var trimmed = originalValue.Trim();
        if (IsAsciiString(trimmed) && trimmed.IndexOf('Q') < 0)
        {
            return normalizedValue;
        }

        var visual = IsAsciiString(trimmed)
            ? trimmed
            : trimmed.Normalize(System.Text.NormalizationForm.FormKC);
        if (visual.IndexOf('Q') < 0)
        {
            return normalizedValue;
        }

        var builder = new System.Text.StringBuilder(visual.Length);
        foreach (var character in visual)
        {
            builder.Append(character == 'Q' ? '0' : character);
        }

        return NormalizeExact(builder.ToString()) ?? normalizedValue;
    }


    private static bool IsMediumVisualEquivalent(char original, char source, char target)
    {
        return (original == '€' && target == 'e')
               || (original == '¢' && target == 'c')
               || (original == '§' && target == 's')
               || (original == '¥' && target == 'y');
    }

    private static bool IsHighVisualEquivalent(char original, char target)
    {
        return (original == '6' && target == 'b')
               || (original == '9' && target == 'q')
               || (original == '2' && target == 's')
               || ((original == '(' || original == '[') && target == 'c')
               || (original == '£' && target == 'l')
               || ((original == '{' || original == '<') && target == 'c');
    }
}
