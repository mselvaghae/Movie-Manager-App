using TrackerDeFavorisApi.Models;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.AspNetCore.Identity;
using TrackerDeFavorisApi.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Globalization;

namespace TrackerDeFavorisApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();


        string? JWTKeyFromConfig = builder.Configuration["JWTKey"];
        if (string.IsNullOrWhiteSpace(JWTKeyFromConfig))
        {
            throw new InvalidAppSettingsException("Clé JWT non trouvée");
        }

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(10), // Temps de tolérance pour la date d'expiration
                    ValidateLifetime = true,              // Vérifie la date d'expiration
                    ValidateIssuerSigningKey = true,      // Vérifie la signature
                    ValidAudience = "localhost:5152",     // Qui peut utiliser le token, ici c'est notre API
                    ValidIssuer = "localhost:5152",       // Qui émet le token, ici c'est notre API
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(JWTKeyFromConfig)
                    ),
                    RoleClaimType = ClaimTypes.Role // Dans quel claim est stocké le role (utilisé pour vérifier l'accessibilité de certaines routes)
                };
            }
        );



        builder.Services.AddSingleton<PasswordHasher<UserInfo>>();
        builder.Services.AddSingleton<JwtService>();
        builder.Services.AddAuthorization();



        builder.Services.AddDbContext<ApplicationContext>();

        builder.Services.AddSingleton<OmdbService>();

        builder.Services.AddHttpClient();



        builder.Services.AddSwaggerGen(option => {
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                }
            });
        });



        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            });

        }

        app.MapControllers();

        app.Run();
    }
}
