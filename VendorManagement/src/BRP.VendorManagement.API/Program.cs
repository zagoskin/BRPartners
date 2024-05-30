using BRP.VendorManagement.API.Controllers.Trasformers;
using BRP.VendorManagement.API.SerializationConverters;
using BRP.VendorManagement.Application;
using BRP.VendorManagement.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Services
    .AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new ToKebabParameterTransformer()));
    })
    .AddJsonOptions(configure =>
    {
        configure.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using (var scope = app.Services.CreateScope())
    {
        app.SeedDatabase(scope.ServiceProvider);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
