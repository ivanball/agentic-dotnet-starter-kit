// Architecture fitness tests: the executable form of the layer rules in CLAUDE.md.
// Adapt the namespace strings to your solution, then prove each test can fail by
// temporarily breaking its rule. A fitness test that cannot fail is decoration.
//
// Package: NetArchTest.Rules
using NetArchTest.Rules;

namespace YourApp.ArchitectureTests;

public class LayerRulesTests
{
    private static string Offenders(TestResult result) =>
        string.Join(", ", result.FailingTypeNames ?? Enumerable.Empty<string>());

    [Fact]
    public void Rule1_Domain_depends_only_on_SharedKernel_and_System()
    {
        var result = Types.InAssembly(typeof(YourApp.Domain.SomeAggregate).Assembly)
            .Should()
            .OnlyHaveDependenciesOn(
                "YourApp.SharedKernel",
                "YourApp.Domain",
                "System")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain grew a dependency: " + Offenders(result));
    }

    [Fact]
    public void Rule2_Domain_does_not_reference_EntityFrameworkCore()
    {
        var result = Types.InAssembly(typeof(YourApp.Domain.SomeAggregate).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, "EF Core leaked into Domain: " + Offenders(result));
    }

    [Fact]
    public void Rule3_Application_does_not_depend_on_Infrastructure_or_Presentation()
    {
        var result = Types.InAssembly(typeof(YourApp.Application.SomeHandler).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "YourApp.Infrastructure",
                "YourApp.Presentation")
            .GetResult();

        Assert.True(result.IsSuccessful, "Application reached outward: " + Offenders(result));
    }

    [Fact]
    public void Rule4_Handlers_are_internal()
    {
        var result = Types.InAssembly(typeof(YourApp.Application.SomeHandler).Assembly)
            .That()
            .ImplementInterface(typeof(YourApp.Application.Abstractions.ICommandHandler<,>))
            .Or()
            .ImplementInterface(typeof(YourApp.Application.Abstractions.IQueryHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult();

        Assert.True(result.IsSuccessful, "Public handlers found: " + Offenders(result));
    }

    [Fact]
    public void Rule5_Only_the_composition_root_touches_Infrastructure()
    {
        // Repeat per assembly that must NOT see Infrastructure (everything except the Host).
        var result = Types.InAssembly(typeof(YourApp.SharedKernel.Entity).Assembly)
            .ShouldNot()
            .HaveDependencyOn("YourApp.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, "Infrastructure leaked: " + Offenders(result));
    }
}
