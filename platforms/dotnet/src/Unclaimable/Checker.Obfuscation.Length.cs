namespace Unclaimable;

public sealed partial class Checker
{
    private bool CanObfuscationLengthsMatch(
        string source,
        int targetLength,
        bool preserveNonCompactCharacters,
        int editBudget,
        bool directPotential)
    {
        var comparable = preserveNonCompactCharacters
            ? source.Length
            : CountCompactComparableCharacters(source);

        if (_obfuscationSensitivity >= ObfuscationSensitivity.High)
        {
            if (directPotential)
            {
                // Multi-character visual aliases can legitimately collapse the source,
                // so only apply the broad upper bound when such evidence is present.
                return targetLength >= 1
                       && targetLength <= source.Length + editBudget;
            }

            // With no obfuscation evidence, High/Extreme behaves like a bounded
            // plain edit search. Reject impossible target lengths before recursion.
            return targetLength >= Math.Max(1, comparable - editBudget)
                   && targetLength <= comparable + editBudget;
        }

        return targetLength >= Math.Max(1, comparable - editBudget)
               && targetLength <= source.Length + editBudget;
    }

    private static int CountCompactComparableCharacters(string value)
    {
        var count = 0;
        foreach (var character in value)
        {
            if (!IsCompactObfuscationIgnorable(character))
            {
                count++;
            }
        }

        return count;
    }
}
