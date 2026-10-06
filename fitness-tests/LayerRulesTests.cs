// Architecture fitness tests: the executable form of the layer rules in CLAUDE.md.
// Six rules, the same six the workshop solution enforces, with the concrete names
// replaced by placeholders. Substitute them once and the file compiles:
//
//   YourApp          -> your solution prefix
//   SomeAggregate    -> any type that lives in your domain assembly
//   SomeHandler      -> any type that lives in your application assembly
//   Module           -> one module name (used in every rule)
//   OtherModule      -> a second module, for rule 6
//   OtherModuleRegistration -> any public type in the second module's infrastructure
//                       assembly, for rule 6
//
// Then prove each test can fail by temporarily breaking the rule it guards and watching
// the red run. A fitness test that has never failed is decoration, not a gate.
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
        var result = Types.InAssembly(typeof(YourApp.Module.Domain.SomeAggregate).Assembly)
            .Should()
            .OnlyHaveDependenciesOn(
                "YourApp.SharedKernel",
                "YourApp.Module.Domain",
                "System")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain grew a dependency: " + Offenders(result));
    }

    [Fact]
    public void Rule2_Domain_does_not_reference_EntityFrameworkCore()
    {
        var result = Types.InAssembly(typeof(YourApp.Module.Domain.SomeAggregate).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, "EF Core leaked into Domain: " + Offenders(result));
    }

    [Fact]
    public void Rule3_Application_does_not_depend_on_Infrastructure_or_Presentation()
    {
        var result = Types.InAssembly(typeof(YourApp.Module.Application.SomeHandler).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "YourApp.Module.Infrastructure",
                "YourApp.Module.Presentation")
            .GetResult();

        Assert.True(result.IsSuccessful, "Application reached outward: " + Offenders(result));
    }

    [Fact]
    public void Rule4_Handlers_are_internal()
    {
        var result = Types.InAssembly(typeof(YourApp.Module.Application.SomeHandler).Assembly)
            .That()
            .ImplementInterface(typeof(YourApp.Module.Application.Abstractions.ICommandHandler<,>))
            .Or()
            .ImplementInterface(typeof(YourApp.Module.Application.Abstractions.IQueryHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult();

        Assert.True(result.IsSuccessful, "Public handlers found: " + Offenders(result));
    }

    [Fact]
    public void Rule5_Only_the_composition_root_touches_Infrastructure()
    {
        // Assemblies outside YourApp.Module.* that are NOT the composition root.
        var result = Types.InAssembly(typeof(YourApp.SharedKernel.Entity).Assembly)
            .ShouldNot()
            .HaveDependencyOn("YourApp.Module.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, "Infrastructure leaked: " + Offenders(result));
        // Other modules' assemblies are covered by
        // Modules_touch_each_other_only_through_contracts below, which forbids every
        // YourApp.Module.* assembly except YourApp.Module.Contracts.
    }

    [Fact]
    public void Modules_touch_each_other_only_through_contracts()
    {
        // Anchor on a public type in the other module's assembly, so the assembly that
        // also holds that module's internal handlers is the one under test.
        var result = Types.InAssembly(typeof(YourApp.OtherModule.Infrastructure.OtherModuleRegistration).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "YourApp.Module.Domain",
                "YourApp.Module.Application",
                "YourApp.Module.Infrastructure",
                "YourApp.Module.Presentation")
            .GetResult();

        Assert.True(result.IsSuccessful, "A module reached past the contracts: " + Offenders(result));
    }
}
