using System.Reflection;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Infrastructure.Security;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Behaviors;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.API;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Services;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Nubeteck.Extensions.Security;
using Nubeteck.Extensions.Web;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.PostProcess = document =>
    {
        document.Info = new NSwag.OpenApiInfo
        {
            Version = "v1",
            Title = "CoopCloud.GeneralSettingsAndAuth API",
            Description = "API for CoopCloud.GeneralSettingsAndAuth application",
            Contact = new NSwag.OpenApiContact
            {
                Name = "Development Team",
                Email = "dev@example.com"
            }
        };
    };

    config.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT token"
    });

    config.OperationProcessors.Add(new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientApp", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// MediatR + Validation Pipeline
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped<FluentValidationEndpointFilter>();

// JWT Configuration
builder.Configuration.GetJWTConfiguration();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.AddJwt();
builder.Services.AddScoped<ICredentialProvider, DBAuthenticationProvider>();

// Audit services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditUserService, AuditUserService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AuditInterceptor>();

// Webhook Services
builder.Services.AddHttpClient("WebHooks", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "CoopCloud-WebHooks/1.0");
});

// Background Services
builder.Services.AddHostedService<CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.Webhooks.WebHookHistoryCleanupService>();

// Auto-register Slices
builder.Services.RegisterSlices(builder.Configuration, typeof(Program).Assembly);

// Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
));

// Mapster
builder.Services.AddMapster();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "CoopCloud.GeneralSettingsAndAuth API Documentation";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/v1/swagger.json";
        config.DocExpansion = "list";
    });
}

// Exception Handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

        if (exceptionFeature?.Error is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(f => f.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await Results.ValidationProblem(errors, "Se encontraron errores de validación").ExecuteAsync(context);
            return;
        }

        await Results.Problem("Ha ocurrido un error inesperado", statusCode: StatusCodes.Status500InternalServerError).ExecuteAsync(context);
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("ClientApp");
app.UseJwt();

// Map endpoints
app.MapSliceEndpoints();

await app.RunAsync();
