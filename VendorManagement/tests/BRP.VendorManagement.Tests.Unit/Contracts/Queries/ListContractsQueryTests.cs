using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BRP.ContractManagement.Application.Contracts;
using BRP.VendorManagement.Application.Contracts.Queries.GetById;
using BRP.VendorManagement.Application.Contracts.Queries.List;
using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.ContractAggregate.ValueObjects;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using ErrorOr;
using FluentAssertions;
using NSubstitute;

namespace BRP.VendorManagement.Tests.Unit.Contracts.Queries;
public class ListContractsQueryTests
{
    private readonly ListContractsQueryHandler _sut;
    private readonly IContractRepository _contractRepository;    
    private static readonly VendorId _vendorId1 = VendorId.Create(Guid.Parse("11111111-4010-44ec-83b2-3718bb9e6e58"));
    private static readonly VendorId _vendorId2 = VendorId.Create(Guid.Parse("22222222-4010-44ec-83b2-3718bb9e6e58"));
    private readonly IUser _user;
    public ListContractsQueryTests()
    {
        _contractRepository = Substitute.For<IContractRepository>();
        _sut = new ListContractsQueryHandler(_contractRepository);
        _user = Substitute.For<IUser>();
    }

    [Fact]
    public async Task ListContractsQueryHandler_ShouldReturnListOfContracts_WhenContractsExists()
    {
        // Arrange        
        _user.Name.Returns("Test User");
        var contract1 = ContractFactory.CreateValidContract(_user, _vendorId1);
        var contract2 = ContractFactory.CreateValidContract(_user, _vendorId2);
        _contractRepository.ListContractsAsync().Returns(new List<Contract> { contract1, contract2 });

        // Act
        var result = await _sut.Handle(new ListContractsQuery(), CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().ContainEquivalentOf(contract1);
        result.Value.Should().ContainEquivalentOf(contract2);
    }

    [Fact]
    public async Task ListContractsQueryHandler_ShouldReturnEmptyList_WhenNoContractsExists()
    {
        // Arrange        
        _contractRepository.ListContractsAsync().Returns(new List<Contract>());

        // Act
        var result = await _sut.Handle(new ListContractsQuery(), CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}
