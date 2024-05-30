using BRP.VendorManagement.Domain.Common.Abstractions;

namespace BRP.VendorManagement.Application.Services;
internal sealed class FakeUserService : IUser
{
    public string? Name => "the.manager@brp.com";
}
