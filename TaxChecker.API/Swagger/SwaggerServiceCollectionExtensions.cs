using Microsoft.OpenApi;

namespace TaxChecker.API.Swagger
{
    public static class SwaggerServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TaxChecker API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("RoleAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "X-Role",
                    Description = "Specify 'User' or 'Admin'"
                });

                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("RoleAuth", document)] = []
                });
            });

            return services;
        }
    }
}
