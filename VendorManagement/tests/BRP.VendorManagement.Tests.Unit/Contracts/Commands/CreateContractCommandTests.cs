using BRP.ContractManagement.Application.Contracts;
using BRP.VendorManagement.Application.Contracts.Commands.Create;
using BRP.VendorManagement.Application.Vendors;
using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace BRP.VendorManagement.Tests.Unit.Contracts.Commands;

public class CreateContractCommandTests
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IUser _user;
    private readonly CreateContractCommandHandler _sut;
    private static readonly VendorId _vendorId = VendorId.Create(Guid.Parse("992a4f6a-4010-44ec-83b2-3718bb9e6e58"));
    public CreateContractCommandTests()
    {
        _vendorRepository = Substitute.For<IVendorRepository>();
        _contractRepository = Substitute.For<IContractRepository>();
        _user = Substitute.For<IUser>();
        _sut = new CreateContractCommandHandler(
            _vendorRepository,
            _contractRepository,
            _user);
    }

    [Fact]
    public async Task CreateContractCommandHandler_ShouldReturnCreatedContract_WhenDataIsValid()
    {
        // Arrange
        _user.Name.Returns("Test User");        
        _vendorRepository.ExistsByIdAsync(_vendorId).Returns(true);
        var expectedContract = ContractFactory.CreateValidContract(_user, _vendorId);
        var createContractCommand = ContractFactory.CreateFromContract(expectedContract);
        _contractRepository.AddContractAsync(Arg.Do<Contract>(x => expectedContract = x)).Returns(Task.CompletedTask);        

        // Act
        var result = await _sut.Handle(createContractCommand, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(expectedContract);
    }

    [Fact]
    public async Task CreateContractCommandHandler_ShouldReturnErrors_WhenDataIsInValid()
    {
        // Arrange
        _user.Name.Returns("Test User");
        _vendorRepository.ExistsByIdAsync(_vendorId).Returns(true);
        var invalidContract = ContractFactory.CreateInValidContract(_user, _vendorId);
        var createContractCommand = ContractFactory.CreateFromContract(invalidContract);        

        // Act
        var result = await _sut.Handle(createContractCommand, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().ContainEquivalentOf(ContractErrors.TitleIsRequired);
        result.Errors.Should().ContainEquivalentOf(ContractErrors.DeadLineMustBeInTheFuture);
        result.Errors.Should().ContainEquivalentOf(ContractErrors.EstimatedValueMustBeGreaterThanZero);
    }

    [Fact]
    public async Task CreateContractCommandHandler_ShouldReturnError_WhenVendorDoesNotExist()
    {
        // Arrange
        _user.Name.Returns("Test User");
        _vendorRepository.ExistsByIdAsync(_vendorId).Returns(false);
        var expectedContract = ContractFactory.CreateValidContract(_user, _vendorId);
        var createContractCommand = ContractFactory.CreateFromContract(expectedContract);
        _contractRepository.AddContractAsync(Arg.Do<Contract>(x => expectedContract = x)).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(createContractCommand, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().ContainEquivalentOf(new
        {
            Description = $"Vendor '{_vendorId.Value}' is invalid"
        });
    }
}
