namespace Unclaimable;

public sealed partial class Checker
{
    private static bool TryMatchExtremeSymbolSequence(
        string source,
        int index,
        char target,
        out int consumed)
    {
        if ((MatchesObfuscationSequence(source, index, "/\\") && target == 'a')
            || (MatchesObfuscationSequence(source, index, "|3") && target == 'b')
            || (MatchesObfuscationSequence(source, index, "|)") && target == 'd')
            || (MatchesObfuscationSequence(source, index, "|=") && target == 'f')
            || (MatchesObfuscationSequence(source, index, "|<") && target == 'k')
            || (MatchesObfuscationSequence(source, index, "|_") && target == 'l')
            || (MatchesObfuscationSequence(source, index, "()") && target == 'o')
            || (MatchesObfuscationSequence(source, index, "|*") && target == 'p')
            || (MatchesObfuscationSequence(source, index, "|2") && target == 'r')
            || (MatchesObfuscationSequence(source, index, "\\/") && target == 'v')
            || (MatchesObfuscationSequence(source, index, "><") && target == 'x'))
        {
            consumed = 2;
            return true;
        }

        if ((MatchesObfuscationSequence(source, index, "|-|") && target == 'h')
            || (MatchesObfuscationSequence(source, index, "|\\|") && target == 'n')
            || (MatchesObfuscationSequence(source, index, "|_|") && target == 'u'))
        {
            consumed = 3;
            return true;
        }

        if ((MatchesObfuscationSequence(source, index, "|\\/|") && target == 'm')
            || (MatchesObfuscationSequence(source, index, "\\/\\/") && target == 'w'))
        {
            consumed = 4;
            return true;
        }

        consumed = 0;
        return false;
    }
}
