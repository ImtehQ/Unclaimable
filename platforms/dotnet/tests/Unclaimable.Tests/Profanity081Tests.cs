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

    [Theory]
    [InlineData("myb00bname", "boob")]
    [InlineData("xxpenisxx", "penis")]
    [InlineData("myp3n1sname", "penis")]
    [InlineData("xxv4g1n4xx", "vagina")]
    [InlineData("mypiemelname", "piemel")]
    public void CuratedExplicitTermsBlockWrappedAndObfuscatedWrappedForms(string value, string expected)
    {
        var result = new Checker(new Options()).Check(value);

        Assert.True(result.IsReserved, value);
        Assert.Equal("profanity", result.Category);
        Assert.Equal(expected, result.MatchedValue);
        Assert.Equal(MatchKind.Partial, result.MatchKind);
    }

    [Theory]
    [InlineData("cocktail")]
    [InlineData("penelope")]
    [InlineData("janus")]
    [InlineData("dickens")]
    public void CollisionProneExplicitTermsRemainExactOnlyByDefault(string value)
    {
        Assert.True(new Checker(new Options()).IsClaimable(value), value);
    }

    [Fact]
    public void RemovingLanguagesDoesNotDisableGlobalExplicitProfanity()
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
    public void GeneralLocalizedProfanityStillRespectsLanguageSelection()
    {
        var defaultChecker = new Checker(new Options());
        Assert.True(defaultChecker.IsClaimable("godverdomme"));

        var dutch = new Options();
        dutch.AddLanguage(Language.Dutch);

        Assert.Equal("profanity", new Checker(dutch).Check("godverdomme").Category);
    }

    [Fact]
    public void CompatibilityProfanityToggleStillDisablesGlobalExplicitProfanity()
    {
        var options = new Options
        {
            ProfanityMatching = false
        };

        Assert.True(new Checker(options).IsClaimable("penis"));
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
