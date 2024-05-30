using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.ContractAggregate.Events;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using ErrorOr;
using FluentAssertions;
using NSubstitute;

namespace BRP.VendorManagement.Tests.Unit.Contracts.Aggregate;
public class ContractTests
{
    private readonly IUser _user;
    private static readonly VendorId _vendorId = VendorId.Create(Guid.Parse("992a4f6a-4010-44ec-83b2-3718bb9e6e58"));
    public ContractTests()
    {
        _user = Substitute.For<IUser>();
    }

    [Fact]
    public void ContractCreate_ShouldReturnContract_WhenDataIsValid()
    {
        // Arrange
        _user.Name.Returns("Test User");
        var expectedContract = ContractFactory.CreateValidContract(_user, _vendorId);

        // Act
        var createResult = Contract.Create(
            _user,
            expectedContract.Title,
            expectedContract.VendorId,
            expectedContract.DeadLineUtc,
            expectedContract.EstimatedValue,
            id: expectedContract.Id);

        // Assert
        createResult.IsError.Should().BeFalse();
        createResult.Value.Should().BeEquivalentTo(expectedContract, options =>
           options
            .For(c => c.ContractHistories)
            .Exclude(ch => ch.Id));
    }

    [Fact]
    public void ContractCreate_ShouldReturnRaiseContractCreatedDomainEvent_WhenInstantiated()
    {
        // Arrange
        _user.Name.Returns("Test User");
        var expectedContract = ContractFactory.CreateValidContract(_user, _vendorId);
        var expectedDomainEvent = new ContractCreatedDomainEvent(expectedContract.Id.Value, _user.Name!);

        // Act
        var createResult = Contract.Create(
            _user,
            expectedContract.Title,
            expectedContract.VendorId,
            expectedContract.DeadLineUtc,
            expectedContract.EstimatedValue,
            id: expectedContract.Id);

        // Assert
        createResult.IsError.Should().BeFalse();
        createResult.Value.DomainEvents.Should().ContainEquivalentOf(expectedDomainEvent);
    }

    [Fact]
    public void ContractCreate_ShouldReturnErrors_WhenDataIsInValid()
    {
        // Arrange
        _user.Name.Returns("Test User");
        var unExpectedContract = ContractFactory.CreateInValidContract(_user, _vendorId);

        // Act
        var createResult = Contract.Create(
            _user,
            unExpectedContract.Title,
            unExpectedContract.VendorId,
            unExpectedContract.DeadLineUtc,
            unExpectedContract.EstimatedValue,
            id: unExpectedContract.Id);

        // Assert        
        createResult.IsError.Should().BeTrue();
        createResult.Value.Should().NotBeEquivalentTo(unExpectedContract);
        createResult.Errors.Should().NotBeEmpty();
        createResult.Errors.Should().ContainEquivalentOf(ContractErrors.TitleIsRequired);
        createResult.Errors.Should().ContainEquivalentOf(ContractErrors.DeadLineMustBeInTheFuture);
        createResult.Errors.Should().ContainEquivalentOf(ContractErrors.EstimatedValueMustBeGreaterThanZero);
    }
}
