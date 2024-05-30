using BRP.VendorManagement.Application.Vendors;
using BRP.VendorManagement.Application.Vendors.Queries.List;
using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.VendorAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace BRP.VendorManagement.Tests.Unit.Vendors.Queries;
public class ListVendorsQueryTests
{
    private readonly ListVendorsQueryHandler _sut;
    private readonly IVendorRepository _vendorRepository;
    private readonly IUser _user;
    private static readonly VendorId _vendorId1 = VendorId.Create(Guid.Parse("11111111-4010-44ec-83b2-3718bb9e6e58"));
    private static readonly VendorId _vendorId2 = VendorId.Create(Guid.Parse("22222222-4010-44ec-83b2-3718bb9e6e58"));
    public ListVendorsQueryTests()
    {
        _vendorRepository = Substitute.For<IVendorRepository>();
        _sut = new ListVendorsQueryHandler(_vendorRepository);
        _user = Substitute.For<IUser>();
    }

    [Fact]
    public async Task ListVendorsQueryHandler_ShouldReturnListOfVendors_WhenVendorsExists()
    {
        // Arrange                
        var vendor1 = VendorFactory.CreateValidVendor(_vendorId1);
        var vendor2 = VendorFactory.CreateValidVendor(_vendorId2);
        _vendorRepository.ListVendorsAsync().Returns(new List<Vendor> { vendor1, vendor2 });

        // Act
        var result = await _sut.Handle(new ListVendorsQuery(), CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().ContainEquivalentOf(vendor1);
        result.Value.Should().ContainEquivalentOf(vendor2);
    }

    [Fact]
    public async Task ListVendorsQueryHandler_ShouldReturnEmptyList_WhenNoVendorsExists()
    {
        // Arrange        
        _vendorRepository.ListVendorsAsync().Returns(new List<Vendor>());

        // Act
        var result = await _sut.Handle(new ListVendorsQuery(), CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}
