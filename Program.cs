using Microsoft.EntityFrameworkCore;
using AnimalCrossingAPI.Models;
using AnimalCrossingAPI.Data;
using Microsoft.OpenApi.Models;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.AspNetCore.Authorization;
using AnimalCrossingAPI.Auth;

namespace AnimalCrossingAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AnimalCrossingDbContext>();

            builder.Services.AddAuthentication().AddJwtBearer();
            builder.Services.AddAuthorization(opts =>
            {
                opts.AddPolicy("RequireWriteFishScope", policy =>
                {
                    policy.Requirements.Add(new ScopeRequirement("write:fish"));
                });
                opts.AddPolicy("CreatedByAuthorization", policy =>
                {
                    policy.Requirements.Add(new CreatedByRequirement());
                });

            });

            builder.Services.AddControllers();
            builder.Services.AddSingleton<IAuthorizationHandler, CreatedByAuthorizationHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>();
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Animal Crossing Fish API",
                    Version = "v1",
                    Description = "A simple API for the fish of Animal Crossing"
                });

                var security = new OpenApiSecurityScheme
                {
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization Header",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                opts.AddSecurityDefinition(security.Reference.Id, security);
                opts.AddSecurityRequirement(new OpenApiSecurityRequirement { { security, Array.Empty<string>() } });

                opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AnimalCrossingApi.xml"));
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                SeedData.Initialize(services);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
