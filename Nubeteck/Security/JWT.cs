using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nubeteck.Extensions.Security;

public static class JWT
{
    public static WebApplicationBuilder AddJwt(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<JwtTokenGenerator>();
        builder.Services.AddSingleton<JwtUtils>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddHttpContextAccessor();

        var jwtSettings = builder.Configuration.GetJWTConfiguration();
        var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

        builder.Services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(config =>
        {
            config.RequireHttpsMetadata = false;
            config.SaveToken = true;
            config.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
            };
        });

        // Authorization policies
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
            .AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));

        // Permission-based authorization for minimal APIs
        builder.Services.AddPermissionBasedAuthorization();
        builder.Services.ConfigurePermissionPolicies();

        builder.Services.AddAuthorization();
        return builder;
    }

    public static IEndpointRouteBuilder UseJwt(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapPost("/login", async (LoginRequest req, ICredentialProvider provider, JwtTokenGenerator generator) =>
        {
            try
            {
                if (await provider.IsValidAsync(req.Email, req.Password))
                {
                    var userInfo = await provider.GetUserInfoAsync(req.Email);
                    return Results.Ok(generator.Generate(userInfo));
                }
                return Results.Unauthorized();
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error creating token: {ex.Message}");
            }
        });

        return app;
    }
}
