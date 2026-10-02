namespace Unclaimable;

public sealed partial class Checker
{
    private bool CanObfuscationLengthsMatch(
        string source,
        int targetLength,
        bool preserveNonCompactCharacters,
        int editBudget)
    {
        var comparable = preserveNonCompactCharacters
            ? source.Length
            : CountCompactComparableCharacters(source);

        if (_obfuscationSensitivity >= ObfuscationSensitivity.High)
        {
            return targetLength >= 1
                   && targetLength <= source.Length + editBudget;
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
