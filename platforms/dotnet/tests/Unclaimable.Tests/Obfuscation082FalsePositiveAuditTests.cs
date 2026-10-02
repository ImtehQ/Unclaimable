using Xunit;

namespace Unclaimable.Tests;

public sealed class Obfuscation082FalsePositiveAuditTests
{
    [Theory]
    [InlineData("quentin7")]
    [InlineData("aquarium7")]
    [InlineData("liquid7")]
    [InlineData("quality7")]
    [InlineData("unique7")]
    [InlineData("query7")]
    [InlineData("equalizer7")]
    [InlineData("sequoia7")]
    [InlineData("quickfox7")]
    public void DefaultMediumKeepsOrdinaryQAndAlphanumericIdentifiersClaimable(string value)
    {
        Assert.True(new Checker(new Options()).IsClaimable(value), value);
    }

    [Theory]
    [InlineData("veltri")]
    [InlineData("veltrax")]
    [InlineData("velrtix")]
    public void MediumDoesNotApplyPlainOneEditMatchingToOrdinaryReservations(string value)
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Medium
        };
        DisableBuiltInReservations(options);
        options.Reserve("veltrix", ReservedMatchMode.WholeIdentifier);

        Assert.True(new Checker(options).IsClaimable(value), value);
    }

    [Fact]
    public void MediumKeepsLowercaseQDistinctFromO()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.Medium
        };
        DisableBuiltInReservations(options);
        options.Reserve("cooper", ReservedMatchMode.WholeIdentifier);

        var checker = new Checker(options);

        Assert.True(checker.IsClaimable("cqoper"));

        var visual = checker.Check("cQoper");
        Assert.True(visual.IsReserved);
        Assert.Equal("cooper", visual.MatchedValue);
    }

    [Theory]
    [InlineData("pieml")]
    [InlineData("piemle")]
    [InlineData("pieme7")]
    [InlineData("adrnin")]
    public void DefaultMediumKeepsKnownNearMissFalsePositiveRegressionsClaimable(string value)
    {
        Assert.True(new Checker(new Options()).IsClaimable(value), value);
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
