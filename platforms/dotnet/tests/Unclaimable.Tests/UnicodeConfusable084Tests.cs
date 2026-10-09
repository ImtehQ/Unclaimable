using Unclaimable;
using Xunit;

namespace Unclaimable.Tests;

/// <summary>
/// Pinned regression samples for the deliberately selective confusable
/// normalization surface; not a claim of full Unicode UTS #39 conformance.
/// </summary>
public sealed class UnicodeConfusable084Tests
{
    [Theory]
    [InlineData("admin", "аdmin")] // Cyrillic a
    [InlineData("admin", "admіn")] // Cyrillic i
    [InlineData("admin", "admın")] // dotless i
    [InlineData("admin", "admín")] // combining acute mark removed
    public void SupportedMappingsRemainReserved(string reserved, string candidate)
    {
        var options = new Options();
        options.AdditionalReserved.Add(reserved);
        Assert.True(new Checker(options).IsReserved(candidate));
    }

    [Fact]
    public void UnmappedUnicodeIsNotClaimedToBeAConfusable()
    {
        // This test intentionally avoids asserting comprehensive TR39 equivalence.
        Assert.NotNull(new Checker().Check("bluegarden"));
    }
}
