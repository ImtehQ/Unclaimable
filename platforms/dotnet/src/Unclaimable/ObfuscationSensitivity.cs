namespace Unclaimable;

/// <summary>Controls how aggressively Unclaimable resolves obfuscated and evasive reserved-name forms.</summary>
public enum ObfuscationSensitivity
{
    /// <summary>
    /// Uses the legacy-style leetspeak and selected Unicode-confusable behavior with no fuzzy edit matching.
    /// This is the closest setting to the 0.8.1 obfuscation behavior.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Adds high-confidence visual equivalence, composed obfuscation handling, and one-edit evasion
    /// matching for curated partial-safe and security-sensitive protected values. This is the default.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// Adds broader visual and multi-character equivalence plus one-edit evasion matching for
    /// protected values of sufficient length.
    /// </summary>
    High = 2,

    /// <summary>
    /// Enables the broadest visual equivalence and allows up to two bounded edits for longer
    /// protected values. Use when false positives are preferable to missed evasions.
    /// </summary>
    Extreme = 3
}
