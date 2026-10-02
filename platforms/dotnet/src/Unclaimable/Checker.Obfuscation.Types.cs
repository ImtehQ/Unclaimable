namespace Unclaimable;

public sealed partial class Checker
{
    private sealed class ObfuscationTarget
    {
        public ObfuscationTarget(string rawText, ReservedEntry entry)
        {
            RawText = rawText;
            Entry = entry;

            bool changed;
            FoldedText = ConfusableNormalizer.CreateSkeleton(
                rawText,
                includeAsciiObfuscation: false,
                out changed);
            FoldedChanged = changed;
        }

        public string RawText { get; }
        public string FoldedText { get; }
        public bool FoldedChanged { get; }
        public ReservedEntry Entry { get; }
    }

    private sealed class ObfuscationSource
    {
        public ObfuscationSource(string text, string? visualText, bool foldedChanged)
        {
            Text = text;
            VisualText = visualText;
            FoldedChanged = foldedChanged;
        }

        public string Text { get; }
        public string? VisualText { get; }
        public bool FoldedChanged { get; }

        public char GetVisualCharacter(int index) =>
            VisualText is not null && index >= 0 && index < VisualText.Length
                ? VisualText[index]
                : Text[index];
    }
}
