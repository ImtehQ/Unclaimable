using Unclaimable.Email;
using Xunit;

namespace Unclaimable.Tests;

public sealed class Regression083Tests
{
    [Fact]
    public void SupplementaryCombiningMarkDoesNotProduceMalformedUtf16()
    {
        var value = "bluegarden\U0001D165";

        var result = new Checker().Check(value);

        Assert.True(result.IsClaimable);
    }

    [Fact]
    public void SupplementaryCombiningMarkInEmailLocalPartDoesNotThrow()
    {
        var result = new EmailChecker()
            .CheckExistingAddress("bluegarden\U0001D165@example.com");

        Assert.True(result.IsAllowed);
    }

    [Theory]
    [InlineData("samplebrand.com", "samp1ebrand.com.evil.com", DomainLookalikeKind.Confusable)]
    [InlineData("samplebrand.com", "www.samp1ebrand.com", DomainLookalikeKind.Confusable)]
    [InlineData("service.com", "s3rv1ce.com", DomainLookalikeKind.Confusable)]
    [InlineData("amazon.com", "arnazon.com", DomainLookalikeKind.Confusable)]
    public void ProtectedDomainLookalikesAreMatchedByRelevantLabels(
        string protectedDomain,
        string candidateDomain,
        DomainLookalikeKind expectedKind)
    {
        var options = new EmailOptions();
        options.ProtectedDomains.Add(protectedDomain);

        var result = new EmailChecker(options)
            .CheckExistingAddress("bluegarden@" + candidateDomain);

        Assert.False(result.IsAllowed);
        Assert.Equal(expectedKind, result.DomainLookalikeKind);
        Assert.Equal(protectedDomain, result.MatchedProtectedDomain);
    }

    [Fact]
    public void ProtectedDomainWithServicePrefixUsesRegistrantLabel()
    {
        var options = new EmailOptions();
        options.ProtectedDomains.Add("www.brand.com");

        var result = new EmailChecker(options)
            .CheckExistingAddress("bluegarden@www.google.com");

        Assert.True(result.IsAllowed);
        Assert.Equal(DomainLookalikeKind.None, result.DomainLookalikeKind);
    }

    [Theory]
    [InlineData("try")]
    [InlineData("sol")]
    [InlineData("won")]
    [InlineData("eth")]
    [InlineData("ces")]
    [InlineData("private")]
    [InlineData("major")]
    [InlineData("officer")]
    [InlineData("army")]
    [InlineData("navy")]
    [InlineData("doctor")]
    [InlineData("nurse")]
    [InlineData("teacher")]
    [InlineData("police")]
    [InlineData("dollar")]
    [InlineData("pound")]
    [InlineData("php")]
    [InlineData("usd")]
    [InlineData("rub")]
    [InlineData("rand")]
    [InlineData("pilot")]
    [InlineData("engineer")]
    [InlineData("judge")]
    [InlineData("captain")]
    [InlineData("general")]
    [InlineData("soldier")]
    public void HighCollisionIdentityTermsAreClaimableByDefault(string value)
    {
        Assert.True(new Checker().IsClaimable(value), value);
    }

    [Fact]
    public void HighCollisionIdentityTermsCanBeOptedBackIn()
    {
        var checker = new Checker(new Options
        {
            IncludeHighCollisionIdentityTerms = true
        });

        var result = checker.Check("doctor");

        Assert.True(result.IsReserved);
        Assert.Equal("profession", result.Category);
    }

    [Fact]
    public void HighObfuscationDoesNotTreatAnUnlimitedRunAsOneEdit()
    {
        var options = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };
        options.DisablePattern(Pattern.Repeated);

        Assert.True(new Checker(options).IsClaimable("admmmin"));
    }

    [Theory]
    [InlineData("user2")]
    [InlineData("user１")]
    [InlineData("user²")]
    [InlineData("user①")]
    public void NumbersRuleUsesCompatibilityNormalizedDigits(string value)
    {
        var options = new Options();
        options.EnableRule(Rule.Numbers);

        var result = new Checker(options).Check(value);

        Assert.True(result.IsReserved);
        Assert.Equal(MatchKind.NumbersNotAllowed, result.MatchKind);
    }

    [Fact]
    public void PatternExceptionsUseTheSameTrimmedNormalizationAsMatching()
    {
        var options = new Options();
        options.DisableRule(Rule.Whitespace);
        options.AllowIdentifierForPattern("aaa", Pattern.Repeated);

        Assert.True(new Checker(options).IsClaimable(" aaa "));
    }

    [Theory]
    [InlineData("wood.com", "vvood.com")]
    [InlineData("cloud.com", "cloucl.com")]
    [InlineData("brand.co.uk", "br4nd.co.uk")]
    public void AdditionalProtectedDomainVisualFormsAreDetected(
        string protectedDomain,
        string candidateDomain)
    {
        var options = new EmailOptions();
        options.ProtectedDomains.Add(protectedDomain);

        var result = new EmailChecker(options)
            .CheckExistingAddress("bluegarden@" + candidateDomain);

        Assert.False(result.IsAllowed);
        Assert.Equal(DomainLookalikeKind.Confusable, result.DomainLookalikeKind);
        Assert.Equal(protectedDomain, result.MatchedProtectedDomain);
    }

    [Fact]
    public void ProtectedDomainWithCommonSecondLevelSuffixAllowsRealSubdomains()
    {
        var options = new EmailOptions();
        options.ProtectedDomains.Add("brand.co.uk");

        var checker = new EmailChecker(options);

        Assert.True(checker.CheckExistingAddress("bluegarden@brand.co.uk").IsAllowed);
        Assert.True(checker.CheckExistingAddress("bluegarden@mail.brand.co.uk").IsAllowed);
    }

    [Fact]
    public void SupplementaryPrivateUseCharacterIsRejectedInEmailLocalPart()
    {
        var result = new EmailChecker()
            .CheckExistingAddress("bluegarden\U000F0000@example.com");

        Assert.False(result.IsAllowed);
        Assert.Equal(EmailFailureKind.InvalidFormat, result.FailureKind);
    }

    [Theory]
    [InlineData("userA")]
    [InlineData("userⅫ")]
    [InlineData("user\U00010107")]
    public void NumbersRuleDoesNotRejectNonDecimalNumericLookalikes(string value)
    {
        var options = new Options();
        options.EnableRule(Rule.Numbers);

        var result = new Checker(options).Check(value);

        Assert.NotEqual(MatchKind.NumbersNotAllowed, result.MatchKind);
    }

    [Fact]
    public void NumbersRuleRejectsCompatibilityFractionContainingDecimalDigits()
    {
        var options = new Options();
        options.EnableRule(Rule.Numbers);

        var result = new Checker(options).Check("user½");

        Assert.Equal(MatchKind.NumbersNotAllowed, result.MatchKind);
    }

    [Theory]
    [InlineData("ééé")]
    [InlineData("éaéa")]
    public void RepeatedPatternSupportsNonAsciiTextElements(string value)
    {
        var result = new Checker().Check(value);

        Assert.Equal(MatchKind.RepeatedPattern, result.MatchKind);
    }

    [Fact]
    public void RepeatedAsciiFastPathIsCaseInsensitive()
    {
        var result = new Checker().Check("AaAa");

        Assert.Equal(MatchKind.RepeatedPattern, result.MatchKind);
    }

    [Fact]
    public void AllowedRepeatedAsciiCharacterBypassesDirectRun()
    {
        var options = new Options();
        options.AllowRepeatedCharacters("a");

        var result = new Checker(options).Check("AAA");

        Assert.NotEqual(MatchKind.RepeatedPattern, result.MatchKind);
    }

    [Fact]
    public void AsciiArtPatternCompactsWhitespaceOnlyWhenNeeded()
    {
        var options = new Options();
        options.DisableRule(Rule.Whitespace | Rule.BlockedCharacters);

        var result = new Checker(options).Check("8 = = = D");

        Assert.Equal(MatchKind.AsciiArt, result.MatchKind);
    }

    [Theory]
    [InlineData("zqxé")]
    [InlineData("zqx\U00010400")]
    public void UnicodeScalarsPassNormalPolicyScanningWithoutMalformedUtf16(string value)
    {
        var result = new Checker().Check(value);

        Assert.NotEqual(MatchKind.InvalidCharacters, result.MatchKind);
    }

    [Fact]
    public void RuleExceptionUsesTrimmedCompatibilityNormalizedIdentifier()
    {
        var options = new Options();
        options.EnableRule(Rule.Numbers);
        options.DisableRule(Rule.Whitespace);
        options.DisablePattern(Pattern.NumericOnly | Pattern.Repeated);
        options.AllowIdentifierForRule(" １２３ ", Rule.Numbers);

        var result = new Checker(options).Check(" 123 ");

        Assert.NotEqual(MatchKind.NumbersNotAllowed, result.MatchKind);
    }

    [Fact]
    public void UserReservationStartingWithInternalPrefixRemainsLiteral()
    {
        const string value = "__unclaimable_internal_currency_rule__:qzunique";

        var options = new Options();
        options.DisableRule(Rule.MaximumLength | Rule.BlockedCharacters);
        options.DisablePattern(Pattern.Repeated);
        options.Reserve(value, ReservedMatchMode.Exact);

        var checker = new Checker(options);

        Assert.True(checker.Check(value).IsReserved);
        Assert.True(checker.IsClaimable("qzunique"));
    }
}
