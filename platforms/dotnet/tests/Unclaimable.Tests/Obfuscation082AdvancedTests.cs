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
        DisableBuiltInReservations(options);
        options.DisableRule(Rule.BlockedCharacters);
        options.DisablePattern(Pattern.AsciiArt);
        options.Reserve("ma", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check(@"|\/|4");

        Assert.True(result.IsReserved);
        Assert.Equal("ma", result.MatchedValue);
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
    public void MediumSupportsSelectedCurrencyStyleVisuals()
    {
        var options = new Options();
        options.DisableRule(Rule.BlockedCharacters);
        options.Reserve("euro", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check("€uro");

        Assert.True(result.IsReserved);
        Assert.Equal("euro", result.MatchedValue);
        Assert.Equal(MatchKind.Obfuscated, result.MatchKind);
    }

    [Fact]
    public void ExtremeAddsBroadLetterShapeEquivalenceWithoutChangingHigh()
    {
        var highOptions = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };
        DisableBuiltInReservations(highOptions);
        highOptions.Reserve("lima", ReservedMatchMode.WholeIdentifier);

        var extremeOptions = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Extreme
        };
        DisableBuiltInReservations(extremeOptions);
        extremeOptions.Reserve("lima", ReservedMatchMode.WholeIdentifier);

        Assert.True(new Checker(highOptions).IsClaimable("iima"));

        var result = new Checker(extremeOptions).Check("iima");
        Assert.True(result.IsReserved);
        Assert.Equal("lima", result.MatchedValue);
    }

    [Fact]
    public void HighCanUseAPlainSubstitutionAsItsSingleEdit()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };
        options.Reserve("veltrix", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check("veltrax");

        Assert.True(result.IsReserved);
        Assert.Equal("veltrix", result.MatchedValue);
    }

    [Fact]
    public void EmptyCustomReservationIsIgnored()
    {
        var options = new Options();
        options.AdditionalReserved.Add("   ");

        var checker = new Checker(options);

        Assert.True(checker.IsClaimable("ordinarycandidate"));
    }


    [Theory]
    [InlineData("§ilo", "silo")]
    public void MediumCoversAdditionalHighConfidenceVisualSymbols(string value, string reserved)
    {
        var options = new Options();
        options.DisableRule(Rule.BlockedCharacters);
        options.Reserve(reserved, ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check(value);

        Assert.True(result.IsReserved);
        Assert.Equal(reserved, result.MatchedValue);
    }

    [Fact]
    public void HighCoversBracketStyleCVisuals()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };
        options.DisableRule(Rule.BlockedCharacters);
        options.DisablePattern(Pattern.AsciiArt);
        options.Reserve("cat", ReservedMatchMode.WholeIdentifier);

        var result = new Checker(options).Check("[at");

        Assert.True(result.IsReserved);
        Assert.Equal("cat", result.MatchedValue);
    }

    [Theory]
    [InlineData("infosex", "security")]
    [InlineData("systen", "system")]
    public void HighAppliesSingleEditProtectionToSensitiveBuiltInCategories(
        string value,
        string expectedCategory)
    {
        var result = new Checker(new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        }).Check(value);

        Assert.True(result.IsReserved, value);
        Assert.Equal(expectedCategory, result.Category);
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
    private static void DisableBuiltInReservations(Options options)
    {
        foreach (var category in Enum.GetValues<Category>())
        {
            options.DisableCategory(category);
        }

        options.DisableRule(
            Rule.CountryNames |
            Rule.PopularCityNames |
            Rule.CelebrityNames |
            Rule.Nationalities |
            Rule.Currencies |
            Rule.Religions |
            Rule.Landmarks |
            Rule.Events |
            Rule.Awards |
            Rule.FictionalCharacters |
            Rule.Franchises |
            Rule.Professions |
            Rule.Military);
    }

}
