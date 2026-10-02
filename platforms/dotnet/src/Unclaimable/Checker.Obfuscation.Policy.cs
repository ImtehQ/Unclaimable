namespace Unclaimable;

public sealed partial class Checker
{
    private int GetObfuscationEditBudget(ReservedEntry entry, int targetLength)
    {
        if (_obfuscationSensitivity == ObfuscationSensitivity.Low || targetLength < 4)
        {
            return 0;
        }

        if (_obfuscationSensitivity == ObfuscationSensitivity.Medium)
        {
            if (targetLength < 5)
            {
                return 0;
            }

            return entry.SafePartial ? 1 : 0;
        }

        if (_obfuscationSensitivity == ObfuscationSensitivity.High)
        {
            return targetLength >= 5 ? 1 : 0;
        }

        return targetLength >= 7 ? 2 : 1;
    }

    private static bool IsSensitiveObfuscationCategory(string category)
    {
        switch (category)
        {
            case "authentication":
            case "communications":
            case "developer":
            case "finance":
            case "governance":
            case "identity":
            case "infrastructure":
            case "legal":
            case "moderation":
            case "official":
            case "operations":
            case "placeholders":
            case "profanity":
            case "roles":
            case "security":
            case "support":
            case "system":
                return true;
            default:
                return false;
        }
    }

    private bool HasDirectObfuscationPotential(string value)
    {
        foreach (var character in value)
        {
            if (ConfusableNormalizer.TryGetObfuscationSubstitutions(character, out _))
            {
                return true;
            }

            if (_obfuscationSensitivity >= ObfuscationSensitivity.Medium
                && (character == 'q'
                    || character == '€'
                    || character == '¢'
                    || character == '§'
                    || character == '¥'))
            {
                return true;
            }

            if (_obfuscationSensitivity >= ObfuscationSensitivity.High
                && (character == '('
                    || character == '['
                    || character == '£'))
            {
                return true;
            }
        }

        return _obfuscationSensitivity >= ObfuscationSensitivity.High
               && (value.IndexOf("rn", StringComparison.Ordinal) >= 0
                   || value.IndexOf("vv", StringComparison.Ordinal) >= 0
                   || value.IndexOf("cl", StringComparison.Ordinal) >= 0);
    }
}
