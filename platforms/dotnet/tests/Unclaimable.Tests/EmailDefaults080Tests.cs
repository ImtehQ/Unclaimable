using Unclaimable.Email;
using Xunit;

namespace Unclaimable.Tests;

public sealed class EmailDefaults080Tests
{
    private const Rule V080ProtectedIdentityRules =
        Rule.CountryNames
        | Rule.PopularCityNames
        | Rule.CelebrityNames
        | Rule.Nationalities
        | Rule.Currencies
        | Rule.Religions
        | Rule.Landmarks
        | Rule.Events
        | Rule.Awards
        | Rule.FictionalCharacters
        | Rule.Franchises
        | Rule.Professions
        | Rule.Military;

    [Fact]
    public void EmailLocalPartKeepsThe080ProtectedIdentityDefaults()
    {
        var options = new EmailOptions();

        Assert.Equal(
            V080ProtectedIdentityRules,
            options.LocalPartOptions.EnabledOptionalRules & V080ProtectedIdentityRules);

        Assert.Equal(
            Rule.None,
            options.LocalPartOptions.DisabledRules & V080ProtectedIdentityRules);
    }

    [Fact]
    public void EmailLocalPartStillAppliesLowCollisionProtectedIdentityRulesByDefault()
    {
        var result = new EmailChecker()
            .CheckExistingAddress("amsterdam@example.com");

        Assert.False(result.IsAllowed);
        Assert.Equal(EmailFailureKind.ReservedLocalPart, result.FailureKind);
        Assert.NotNull(result.LocalPartResult);
        Assert.Equal(MatchKind.PopularCityName, result.LocalPartResult!.MatchKind);
    }

    [Fact]
    public void EmailLocalPartAllowsHighCollisionIdentityTermsByDefault()
    {
        var result = new EmailChecker()
            .CheckExistingAddress("physician@example.com");

        Assert.True(result.IsAllowed);
    }

    [Fact]
    public void EmailLocalPartCanOptBackIntoHighCollisionIdentityTerms()
    {
        var options = new EmailOptions();
        options.LocalPartOptions.IncludeHighCollisionIdentityTerms = true;

        var result = new EmailChecker(options)
            .CheckExistingAddress("physician@example.com");

        Assert.False(result.IsAllowed);
        Assert.Equal(EmailFailureKind.ReservedLocalPart, result.FailureKind);
        Assert.NotNull(result.LocalPartResult);
        Assert.Equal(MatchKind.Exact, result.LocalPartResult!.MatchKind);
    }

    [Fact]
    public void EmailSpecificSyntaxAdjustmentsRemainSeparateFromIdentityDefaults()
    {
        var options = new EmailOptions();

        Assert.True((options.LocalPartOptions.DisabledRules & Rule.BlockedCharacters) != 0);
        Assert.True((options.LocalPartOptions.DisabledRules & Rule.Whitespace) != 0);
        Assert.Equal(Pattern.None, options.LocalPartOptions.EnabledPatterns);
    }
}
