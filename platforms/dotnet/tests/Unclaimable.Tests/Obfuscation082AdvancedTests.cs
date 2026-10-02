using Xunit;

namespace Unclaimable.Tests;

public sealed class Obfuscation082AdvancedTests
{
    [Fact]
    public void HighTreatsRepeatedInsertedRunAsOneBoundedEdit()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };
        options.DisablePattern(Pattern.Repeated);
        options.Reserve("veltrix", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check("velxxxtrix");

        Assert.True(result.IsReserved);
        Assert.Equal("veltrix", result.MatchedValue);
    }

    [Fact]
    public void ExtremeSupportsMultiCharacterSymbolShapes()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Extreme
        };
        options.DisableRule(Rule.BlockedCharacters);
        options.DisablePattern(Pattern.AsciiArt);
        options.Reserve("bex", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check("|3e><");

        Assert.True(result.IsReserved);
        Assert.Equal("bex", result.MatchedValue);
    }


    [Fact]
    public void ExtremeSupportsThreeCharacterSymbolShapes()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Extreme
        };
        options.DisableRule(Rule.BlockedCharacters);
        options.DisablePattern(Pattern.AsciiArt);
        options.Reserve("hut", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check(@"|-||_|7");

        Assert.True(result.IsReserved);
        Assert.Equal("hut", result.MatchedValue);
    }

    [Fact]
    public void ExtremeSupportsFourCharacterSymbolShapes()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Extreme
        };
        options.DisableRule(Rule.BlockedCharacters);
        options.DisablePattern(Pattern.AsciiArt);
        options.Reserve("mx", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check(@"|\/|><");

        Assert.True(result.IsReserved);
        Assert.Equal("mx", result.MatchedValue);
    }

    [Fact]
    public void WrappedMediumObfuscationUsesCuratedPartialMatching()
    {
        var options = new Options();
        options.AllowCharacters("-");

        var result = new Checker(options).Check("xxp-1-m-3-Lxx");

        Assert.True(result.IsReserved);
        Assert.Equal("profanity", result.Category);
        Assert.Equal(MatchKind.Partial, result.MatchKind);
    }

    [Fact]
    public void WholeIdentifierObfuscationCanIgnoreAnAllowedTrailingSeparator()
    {
        var options = new Options();
        options.AllowCharacters("-");
        options.DisableRule(Rule.TrailingSeparator);
        options.Reserve("admin", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check("4dmin-");

        Assert.True(result.IsReserved);
        Assert.Equal("admin", result.MatchedValue);
        Assert.Equal(MatchKind.Obfuscated, result.MatchKind);
    }

    [Fact]
    public void HighSupportsAdditionalSingleCharacterVisualShapes()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };
        options.DisableRule(Rule.BlockedCharacters);
        options.Reserve("legal", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check("£egal");

        Assert.True(result.IsReserved);
        Assert.Equal("legal", result.MatchedValue);
        Assert.Equal(MatchKind.Obfuscated, result.MatchKind);
    }

    [Fact]
    public void MediumComposesSeparatorsLeetspeakAndOneEdit()
    {
        var options = new Options();
        options.AllowCharacters("-");

        var result = new Checker(options).Check("p-1-m-3-L");

        Assert.True(result.IsReserved);
        Assert.Equal("profanity", result.Category);
    }
}
