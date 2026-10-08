namespace Unclaimable;

public sealed partial class Checker
{
    private bool TryMatchObfuscationIndex(
        string value,
        ObfuscationIndex index,
        bool preserveNonCompactCharacters,
        out ReservedEntry? match,
        out MatchKind matchKind,
        out int? matchStartIndex,
        out int? matchLength)
    {
        var directEntries = preserveNonCompactCharacters
            ? index.ExactEntries
            : index.CompactEntries;

        var directPotential = HasDirectObfuscationPotential(value);
        var failedStates = new HashSet<long>();

        if (directPotential
            && TryMatchZeroCostObfuscation(
                value,
                index,
                preserveNonCompactCharacters,
                failedStates,
                out match,
                out matchKind,
                out matchStartIndex,
                out matchLength))
        {
            return true;
        }

        foreach (var pair in directEntries)
        {
            var editBudget = GetObfuscationEditBudget(pair.Value, pair.Key.Length);
            if (editBudget == 0 && !directPotential)
            {
                continue;
            }

            if (!CanObfuscationLengthsMatch(
                    value,
                    pair.Key.Length,
                    preserveNonCompactCharacters,
                    editBudget,
                    directPotential))
            {
                continue;
            }

            int endIndex;
            if (!TryMatchObfuscationText(
                    value,
                    sourceStart: 0,
                    pair.Key,
                    preserveNonCompactCharacters,
                    editBudget,
                    requireSourceEnd: true,
                    failedStates,
                    out endIndex))
            {
                continue;
            }

            match = pair.Value;
            matchKind = MatchKind.Obfuscated;
            matchStartIndex = 0;
            matchLength = endIndex;
            return true;
        }

        if (_partialMatching
            && TryMatchObfuscationPartials(
                value,
                index.PartialEntries,
                preserveNonCompactCharacters,
                directPotential,
                failedStates,
                out match,
                out matchStartIndex,
                out matchLength))
        {
            matchKind = MatchKind.Partial;
            return true;
        }

        match = null;
        matchKind = MatchKind.None;
        matchStartIndex = null;
        matchLength = null;
        return false;
    }
}
