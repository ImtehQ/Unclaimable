namespace Unclaimable;

public sealed partial class Checker
{
    private static bool IsMediumVisualEquivalent(char original, char source, char target)
    {
        return (source == 'q' && target == 'o')
               || (original == '€' && target == 'e')
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
