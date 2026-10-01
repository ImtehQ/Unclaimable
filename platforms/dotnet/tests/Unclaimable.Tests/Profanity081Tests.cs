using Xunit;

namespace Unclaimable.Tests;

public sealed class Profanity081Tests
{
    [Theory]
    [InlineData("boobs")]
    [InlineData("piemel")]
    [InlineData("pimmel")]
    [InlineData("vagin")]
    [InlineData("pene")]
    [InlineData("vagine")]
    [InlineData("pênis")]
    [InlineData("pochwa")]
    [InlineData("vajina")]
    [InlineData("payudara")]
    [InlineData("vagína")]
    [InlineData("dươngvật")]
    [InlineData("pénisz")]
    [InlineData("tuttar")]
    [InlineData("țâțe")]
    public void ProfanityFromEverySupportedLanguageIsReservedByDefault(string value)
    {
        var result = new Checker(new Options()).Check(value);

        Assert.True(result.IsReserved, value);
        Assert.Equal("profanity", result.Category);
    }

    [Theory]
    [InlineData("p3n1s", "penis")]
    [InlineData("b00bs", "boobs")]
    [InlineData("v4g1n4", "vagina")]
    [InlineData("t1ts", "tits")]
    public void ExplicitTermsUseExistingObfuscationPipeline(string value, string expected)
    {
        var result = new Checker(new Options()).Check(value);

        Assert.True(result.IsReserved, value);
        Assert.Equal("profanity", result.Category);
        Assert.Equal(expected, result.MatchedValue);
        Assert.Equal(MatchKind.Obfuscated, result.MatchKind);
    }

    [Fact]
    public void RemovingLanguagesDoesNotDisableProfanity()
    {
        var options = new Options();
        foreach (var language in Enum.GetValues<Language>())
        {
            options.RemoveLanguage(language);
        }

        var checker = new Checker(options);

        Assert.Empty(options.Languages);
        Assert.Equal("profanity", checker.Check("piemel").Category);
        Assert.Equal("profanity", checker.Check("pimmel").Category);
        Assert.Equal("profanity", checker.Check("pene").Category);
    }

    [Fact]
    public void ProfanityCanStillBeDisabledByCategory()
    {
        var options = new Options();
        options.DisableCategory(Category.Profanity);

        var checker = new Checker(options);

        Assert.True(checker.IsClaimable("penis"));
        Assert.True(checker.IsClaimable("piemel"));
        Assert.True(checker.IsClaimable("pimmel"));
    }

    [Fact]
    public void ProfanityCanStillBeDisabledByRule()
    {
        var options = new Options();
        options.DisableRule(Rule.Profanity);

        var checker = new Checker(options);

        Assert.True(checker.IsClaimable("penis"));
        Assert.True(checker.IsClaimable("piemel"));
        Assert.True(checker.IsClaimable("pimmel"));
    }
}
