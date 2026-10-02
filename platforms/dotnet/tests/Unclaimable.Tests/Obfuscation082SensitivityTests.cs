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
        options.Reserve("example", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check("exampl");

        Assert.True(result.IsReserved);
        Assert.Equal("example", result.MatchedValue);
        Assert.Equal("custom", result.Category);
    }

    [Fact]
    public void ExtremeAllowsTwoEditsForLongReservations()
    {
        var highOptions = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };
        highOptions.Reserve("example", ReservedMatchMode.WholeIdentifier);

        var extremeOptions = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Extreme
        };
        extremeOptions.Reserve("example", ReservedMatchMode.WholeIdentifier);

        Assert.True(new Checker(highOptions).IsClaimable("exmpe"));

        var result = new Checker(extremeOptions).Check("exmpe");
        Assert.True(result.IsReserved);
        Assert.Equal("example", result.MatchedValue);
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
