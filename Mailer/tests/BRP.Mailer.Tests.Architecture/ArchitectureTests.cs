using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using BRP.Mailer.API;
using Xunit.Abstractions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace BRP.Mailer.Tests.Arch;

public class ArchitectureTests
{
    private readonly ITestOutputHelper _outputHelper;

    private static readonly Architecture _architecture = new ArchLoader()
        .LoadAssemblies(typeof(IMailerAssemblyMarker).Assembly)
        .Build();

    public ArchitectureTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void TypesInDomain_ShouldNotReference_TypesInInfrastructure()
    {
        // Arrange
        var domainTypes = Types()
            .That()
            .ResideInNamespace("BRP.Mailer.API.Domain.*", useRegularExpressions: true)
            .As("OrderProcessing Domain Types");

        var infrastructureTypes = Types()
            .That()
            .ResideInNamespace("BRP.Mailer.API.Infrastructure.*", useRegularExpressions: true)
            .As("OrderProcessing Infrastructure Types");

        // Act & Assert

        PrintTypes(domainTypes, infrastructureTypes);
        domainTypes
            .Should()
            .NotDependOnAny(infrastructureTypes)
            .Check(_architecture);
    }

    [Fact]
    public void TypesInDomain_ShouldNotReference_TypesInApplication()
    {
        // Arrange
        var domainTypes = Types()
            .That()
            .ResideInNamespace("BRP.Mailer.API.Domain.*", useRegularExpressions: true)
            .As("OrderProcessing Domain Types");

        var applicationTypes = Types()
            .That()
            .ResideInNamespace("BRP.Mailer.API.Application.*", useRegularExpressions: true)
            .As("OrderProcessing Application Types");

        // Act & Assert

        PrintTypes(domainTypes, applicationTypes);
        domainTypes
            .Should()
            .NotDependOnAny(applicationTypes)
            .Check(_architecture);
    }

    [Fact]
    public void TypesInApplication_ShouldNotReference_TypesInInfrastructure()
    {
        // Arrange
        var applicationTypes = Types()
            .That()
            .ResideInNamespace("BRP.Mailer.API.Application.*", useRegularExpressions: true)
            .As("OrderProcessing Application Types");

        var infrastructureTypes = Types()
            .That()
            .ResideInNamespace("BRP.Mailer.API.Infrastructure.*", useRegularExpressions: true)
            .As("OrderProcessing Infrastructure Types");

        // Act & Assert

        PrintTypes(applicationTypes, infrastructureTypes);
        applicationTypes
            .Should()
            .NotDependOnAny(infrastructureTypes)
            .Check(_architecture);
    }

    private void PrintTypes(GivenTypesConjunctionWithDescription domainTypes, GivenTypesConjunctionWithDescription infrastructureTypes)
    {
        _outputHelper.WriteLine(domainTypes.Description);
        foreach (var type in domainTypes.GetObjects(_architecture))
        {
            _outputHelper.WriteLine($"Domain Type: {type.FullName}");
            foreach (var dependency in type.Dependencies)
            {
                var targetType = dependency.Target;
                if (infrastructureTypes.GetObjects(_architecture).Contains(targetType))
                {
                    _outputHelper.WriteLine($"    Depends on Infrastructure Type: {targetType.FullName}");
                }
            }
        }

        _outputHelper.WriteLine(infrastructureTypes.Description);
        foreach (var type in infrastructureTypes.GetObjects(_architecture))
        {
            _outputHelper.WriteLine($"Infrastructure Type: {type.FullName}");
        }
    }
}
