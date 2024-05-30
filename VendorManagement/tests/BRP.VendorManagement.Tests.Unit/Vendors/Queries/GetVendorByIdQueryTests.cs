using BRP.VendorManagement.Application.Vendors;
using BRP.VendorManagement.Application.Vendors.Queries.GetById;
using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using ErrorOr;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace BRP.VendorManagement.Tests.Unit.Vendors.Queries;

public class GetVendorByIdQueryTests
{
    private readonly GetVendorByIdQueryHandler _sut;
    private readonly IVendorRepository _vendorRepository;
    private static readonly VendorId _vendorId = VendorId.Create(Guid.Parse("992a4f6a-4010-44ec-83b2-3718bb9e6e58"));
    private readonly IUser _user;
    public GetVendorByIdQueryTests()
    {
        _vendorRepository = Substitute.For<IVendorRepository>();
        _sut = new GetVendorByIdQueryHandler(_vendorRepository);
        _user = Substitute.For<IUser>();
    }

    [Fact]
    public async Task GetVendorByIdQueryHandler_ShouldReturnVendor_WhenVendorExists()
    {
        // Arrange        
        _user.Name.Returns("Test User");
        var vendor = VendorFactory.CreateValidVendor(_vendorId);
        _vendorRepository.GetVendorByIdAsync(_vendorId).Returns(vendor);

        // Act
        var result = await _sut.Handle(new GetVendorByIdQuery(_vendorId.Value), CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(vendor);
    }

    [Fact]
    public async Task GetVendorByIdQueryHandler_ShouldReturnNotFoundError_WhenVendorDoesNotExists()
    {
        // Arrange                        
        _vendorRepository.GetVendorByIdAsync(_vendorId).ReturnsNull();

        // Act
        var result = await _sut.Handle(new GetVendorByIdQuery(_vendorId.Value), CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Type.Should().Be(ErrorType.NotFound);
    }
}
