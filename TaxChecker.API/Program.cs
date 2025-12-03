using TaxChecker.Infrastructure.Database;
using TaxChecker.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
DatabaseInitializer.EnsureDatabase(connectionString);
builder.Services.AddInfrastructure(connectionString);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

DatabaseInitializer.ApplyMigrations(app.Services);
DatabaseInitializer.SeedData(connectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
