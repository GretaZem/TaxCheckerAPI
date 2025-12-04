using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using TaxChecker.API.Security;
using TaxChecker.API.Swagger;
using TaxChecker.Application.DependencyInjection;
using TaxChecker.Infrastructure.Database;
using TaxChecker.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
DatabaseInitializer.EnsureDatabase(connectionString);
builder.Services.AddInfrastructure(connectionString);

builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddSwagger();

builder.Services.AddAuthentication("HeaderScheme")
    .AddScheme<AuthenticationSchemeOptions, HeaderAuthenticationHandler>("HeaderScheme", options => { });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes("HeaderScheme")
        .RequireAuthenticatedUser()
        .RequireRole(AppRoles.All) 
        .Build();

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(AppRoles.Admin));
});

var app = builder.Build();

DatabaseInitializer.ApplyMigrations(app.Services);
DatabaseInitializer.SeedData(connectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
