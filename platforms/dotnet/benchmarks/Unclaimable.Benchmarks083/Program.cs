using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Unclaimable;
using Unclaimable.Email;

BenchmarkSwitcher
    .FromAssembly(typeof(Core083Benchmarks).Assembly)
    .Run(args);

[MemoryDiagnoser]
[SimpleJob(warmupCount: 2, iterationCount: 6)]
public class Core083Benchmarks
{
    private Checker _checker = null!;
    private Checker _highChecker = null!;

    [GlobalSetup]
    public void Setup()
    {
        _checker = new Checker();

        var high = new Options
        {
            ObfuscationSensitivity = ObfuscationSensitivity.High
        };
        high.DisablePattern(Pattern.Repeated);
        _highChecker = new Checker(high);
    }

    [Benchmark(Baseline = true)]
    public Result CheckOrdinaryAccepted() =>
        _checker.Check("zqvxpioneer");

    [Benchmark]
    public bool IsClaimableOrdinaryAccepted() =>
        _checker.IsClaimable("zqvxpioneer");

    [Benchmark]
    public Result CheckExactReserved() =>
        _checker.Check("admin");

    [Benchmark]
    public bool IsClaimableExactReserved() =>
        _checker.IsClaimable("admin");

    [Benchmark]
    public Result CheckObfuscation() =>
        _checker.Check("N1ke");

    [Benchmark]
    public Result CheckHighNearMatchMiss() =>
        _highChecker.Check("admmmin");
}

[MemoryDiagnoser]
[SimpleJob(warmupCount: 2, iterationCount: 6)]
public class Email083Benchmarks
{
    private EmailChecker _checker = null!;

    [GlobalSetup]
    public void Setup()
    {
        var options = new EmailOptions();
        options.ProtectedDomains.Add("examplebrand.com");
        _checker = new EmailChecker(options);
    }

    [Benchmark(Baseline = true)]
    public EmailResult OrdinaryEmail() =>
        _checker.CheckExistingAddress("bluegarden@example.net");

    [Benchmark]
    public EmailResult ExactProtectedDomain() =>
        _checker.CheckExistingAddress("bluegarden@examplebrand.com");

    [Benchmark]
    public EmailResult ProtectedDomainLookalike() =>
        _checker.CheckExistingAddress("bluegarden@examp1ebrand.com");
}
