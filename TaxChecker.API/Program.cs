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
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
