using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.Webhooks;
using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Application.Repositories;
using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nubeteck.Extensions.Web;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.API;

public class WebhooksSlice : ISlice
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IWebHookConfigurationRepository, WebHookConfigurationRepository>();
        services.AddScoped<IWebHookExecutionHistoryRepository, WebHookExecutionHistoryRepository>();
        services.AddScoped<IWebHookService, WebHookService>();
    }

    public void RegisterRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/webhooks")
            .WithTags("Webhooks")
            .RequireAuthorization();

        // GET /api/webhooks - Get all webhooks
        group.MapGet("/", async (IWebHookConfigurationRepository repository, CancellationToken cancellationToken) =>
        {
            var webhooks = await repository.GetAllAsync(cancellationToken);
            return Results.Ok(webhooks);
        })
        .WithName("GetAllWebhooks")
        .WithDescription("Gets all webhook configurations");

        // GET /api/webhooks/{id} - Get webhook by ID
        group.MapGet("/{id:guid}", async (Guid id, IWebHookConfigurationRepository repository, CancellationToken cancellationToken) =>
        {
            var webhook = await repository.GetByIdAsync(id, cancellationToken);
            return webhook is not null ? Results.Ok(webhook) : Results.NotFound();
        })
        .WithName("GetWebhookById")
        .WithDescription("Gets a webhook configuration by ID");

        // POST /api/webhooks/{webhookId}/test - Test a webhook
        group.MapPost("/{webhookId:guid}/test", async (
            Guid webhookId,
            TestWebhookRequest request,
            IWebHookService webhookService,
            CancellationToken cancellationToken) =>
        {
            var result = await webhookService.ExecuteWebhookAsync(webhookId, request.TestPayload ?? new { test = true }, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("TestWebhook")
        .WithDescription("Tests a webhook with a test payload");

        // GET /api/webhooks/{webhookId}/history - Get webhook execution history
        group.MapGet("/{webhookId:guid}/history", async (
            Guid webhookId,
            int page,
            int pageSize,
            IWebHookExecutionHistoryRepository repository,
            CancellationToken cancellationToken) =>
        {
            var (items, totalCount) = await repository.GetByWebhookIdAsync(webhookId, page, pageSize, cancellationToken);
            return Results.Ok(new { Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize });
        })
        .WithName("GetWebhookHistory")
        .WithDescription("Gets execution history for a webhook");
    }
}

public record TestWebhookRequest(object? TestPayload);
