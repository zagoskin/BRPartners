using BRP.VendorManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BRP.VendorManagement.Infrastructure;
public static class Setup
{
    public static void SeedDatabase(this IApplicationBuilder app, IServiceProvider serviceProvider)
    {        
        var context = serviceProvider.GetRequiredService<VendorManagementDbContext>();
        context.Database.EnsureCreated();        
    }
}
