using Xunit;

namespace Unclaimable.Tests;

public sealed class Obfuscation082SensitivityTests
{
    [Fact]
    public void HighAddsMultiCharacterVisualMatching()
    {
        var medium = new Checker(new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Medium
        });
        var high = new Checker(new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        });

        Assert.True(medium.IsClaimable("adrnin"));

        var result = high.Check("adrnin");
        Assert.True(result.IsReserved);
        Assert.Equal("admin", result.MatchedValue);
    }

    [Fact]
    public void HighAddsOneEditMatchingForOrdinaryReservations()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };
        options.Reserve("veltrix", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check("veltri");

        Assert.True(result.IsReserved);
        Assert.Equal("veltrix", result.MatchedValue);
        Assert.Equal("custom", result.Category);
    }

    [Theory]
    [InlineData("pieml")]
    [InlineData("pieemel")]
    [InlineData("piemle")]
    public void HighBlocksSingleEditAlphabeticEvasions(string value)
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };

        var result = new Checker(options).Check(value);

        Assert.True(result.IsReserved, value);
        Assert.Equal("profanity", result.Category);
    }

    [Fact]
    public void ExtremeAllowsTwoEditsForLongReservations()
    {
        var highOptions = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };
        highOptions.Reserve("zavtrix", ReservedMatchMode.WholeIdentifier);

        var extremeOptions = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Extreme
        };
        extremeOptions.Reserve("zavtrix", ReservedMatchMode.WholeIdentifier);

        Assert.True(new Checker(highOptions).IsClaimable("zavti"));

        var result = new Checker(extremeOptions).Check("zavti");
        Assert.True(result.IsReserved);
        Assert.Equal("zavtrix", result.MatchedValue);
    }

    [Fact]
    public void MatchingNoLongerDependsOnThirtyTwoCandidateLimit()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Low
        };
        options.DisablePattern(Pattern.Repeated);
        options.Reserve("allllllb", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check("a111111b");

        Assert.True(result.IsReserved);
        Assert.Equal("allllllb", result.MatchedValue);
    }

    [Fact]
    public void DisablingObfuscationStillDisablesSensitivityLayers()
    {
        var options = new Options();
        options.DisableRule(Rule.ObfuscationMatching);

        Assert.True(new Checker(options).IsClaimable("p1m3L"));
    }
}
