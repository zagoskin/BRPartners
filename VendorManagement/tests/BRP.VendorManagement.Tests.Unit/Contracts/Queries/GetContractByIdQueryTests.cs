using BRP.ContractManagement.Application.Contracts;
using BRP.VendorManagement.Application.Contracts.Queries.GetById;
using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.ContractAggregate.ValueObjects;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using ErrorOr;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace BRP.VendorManagement.Tests.Unit.Contracts.Queries;
public class GetContractByIdQueryTests
{
    private readonly GetContractByIdQueryHandler _sut;
    private readonly IContractRepository _contractRepository;
    private static readonly ContractId _contractId = ContractId.Create(Guid.Parse("992a4f6a-4010-44ec-83b2-3718bb9e6e58"));
    private static readonly VendorId _vendorId = VendorId.Create(Guid.Parse("11111111-4010-44ec-83b2-3718bb9e6e58"));
    private readonly IUser _user;
    public GetContractByIdQueryTests()
    {
        _contractRepository = Substitute.For<IContractRepository>();
        _sut = new GetContractByIdQueryHandler(_contractRepository);
        _user = Substitute.For<IUser>();
    }

    [Fact]
    public async Task GetContractByIdQueryHandler_ShouldReturnContract_WhenContractExists()
    {
        // Arrange        
        _user.Name.Returns("Test User");
        var contract = ContractFactory.CreateValidContract(_user, _vendorId, contractId: _contractId);
        _contractRepository.GetContractByIdAsync(_contractId).Returns(contract);

        // Act
        var result = await _sut.Handle(new GetContractByIdQuery(_contractId.Value), CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(contract);
    }

    [Fact]
    public async Task GetContractByIdQueryHandler_ShouldReturnNotFoundError_WhenContractDoesNotExists()
    {
        // Arrange                        
        _contractRepository.GetContractByIdAsync(_contractId).ReturnsNull();

        // Act
        var result = await _sut.Handle(new GetContractByIdQuery(_contractId.Value), CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Type.Should().Be(ErrorType.NotFound);
    }
}
