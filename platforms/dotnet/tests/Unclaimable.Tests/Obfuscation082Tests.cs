using Xunit;

namespace Unclaimable.Tests;

public sealed class Obfuscation082Tests
{
    [Fact]
    public void MediumSensitivityIsDefault()
    {
        Assert.Equal(ObfuscationSensitivity.Medium, new Options().ObfuscationSensitivity);
    }

    [Fact]
    public void InvalidSensitivityIsRejected()
    {
        var options = new Options
        {
            ObfuscationSensitivity = (ObfuscationSensitivity)99
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            nameof(Options.ObfuscationSensitivity),
            () => new Checker(options));
    }

    [Theory]
    [InlineData("BQQb", "boob")]
    [InlineData("bQ0b", "boob")]
    public void MediumBlocksHighConfidenceVisualEvasions(string value, string expected)
    {
        var result = new Checker(new Options()).Check(value);

        Assert.True(result.IsReserved, value);
        Assert.Equal("profanity", result.Category);
        Assert.Equal(expected, result.MatchedValue);
    }

    [Fact]
    public void MediumComposesLeetspeakWithOneEditForProtectedTerms()
    {
        var result = new Checker(new Options()).Check("p1m3L");

        Assert.True(result.IsReserved);
        Assert.Equal("profanity", result.Category);
        Assert.Contains(result.MatchedValue, new[] { "piemel", "pimmel" });
    }

    [Fact]
    public void MediumDoesNotFuzzyMatchPlainAlphabeticTypos()
    {
        var checker = new Checker(new Options());

        Assert.True(checker.IsClaimable("pieml"));
        Assert.True(checker.IsClaimable("piemle"));
    }

    [Fact]
    public void MixedUnicodeAndAsciiVisualsCompose()
    {
        var result = new Checker(new Options()).Check("B\u041EQb");

        Assert.True(result.IsReserved);
        Assert.Equal("boob", result.MatchedValue);
    }

    [Fact]
    public void LowKeepsLegacyLikeEditBehavior()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Low
        };

        var checker = new Checker(options);

        Assert.True(checker.IsClaimable("BQQb"));
        Assert.True(checker.IsClaimable("p1m3L"));

        var positiveControl = checker.Check("p13m3l");
        Assert.True(positiveControl.IsReserved);
        Assert.Equal("piemel", positiveControl.MatchedValue);
    }
}
