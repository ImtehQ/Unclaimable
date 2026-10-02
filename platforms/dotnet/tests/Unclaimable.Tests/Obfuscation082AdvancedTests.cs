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
    public void MediumComposesSeparatorsLeetspeakAndOneEdit()
    {
        var options = new Options();
        options.AllowCharacters("-");

        var result = new Checker(options).Check("p-1-m-3-L");

        Assert.True(result.IsReserved);
        Assert.Equal("profanity", result.Category);
    }
}
