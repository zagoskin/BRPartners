using BRP.Mailer.API.Application;
using BRP.Mailer.API.Infrastructure;
using BRP.Mailer.API.Endpoints;

// Nesse projeto escolhi usar pastas no lugar de projetos como para simbolizar um projeto minimo, que não espera ter muita mudança
// As pastas seguem o padrão Clean mas o fato da arquitetura ser respeitada depende do desenvolvedor, mas poderia usar um test de arquitetura
// tipo ArchUnit para pegar se alguem esta indo fora do padrão

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapListEmailsEndpoint();

app.Run();
