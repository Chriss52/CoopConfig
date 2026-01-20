using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Commands;
using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Queries;
using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Repositories;
using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Infrastructure.Repositories;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.ConfigurationSourceProcessor;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nubeteck.Extensions.Web;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.API;

public class GeneralSettingsSlice : ISlice
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IGeneralSettingRepository, GeneralSettingRepository>();
        services.AddScoped<ConfigurationSourceProcessorFactory>();
    }

    public void RegisterRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/general-settings")
            .WithTags("General Settings")
            .RequireAuthorization();

        // GET /api/general-settings
        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetAllGeneralSettingsQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetAllSettings")
        .WithDescription("Gets all general settings")
        .Produces<List<GeneralSettingDto>>();

        // GET /api/general-settings-options/{configurationId}
        group.MapGet("/options/{configurationId:guid}", async (Guid configurationId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetGeneralSettingsOptionByIdQuery(configurationId), cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetGeneralSettingsOptionById")
        .WithDescription("Gets options for a specific general setting")
        .Produces<GeneralSettingsOptionDto>()
        .Produces(StatusCodes.Status404NotFound);

        // PUT /api/general-settings
        group.MapPut("/", async (UpdateGeneralSettingsRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            await mediator.Send(new UpdateGeneralSettingsCommand(request.Settings), cancellationToken);
            return Results.Ok();
        })
        .WithName("UpdateGeneralSettings")
        .WithDescription("Updates multiple general settings at once")
        .Produces(StatusCodes.Status200OK);

        // POST /api/general-settings/{configurationId}/reset
        group.MapPost("/{configurationId:guid}/reset", async (Guid configurationId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            await mediator.Send(new ResetGeneralSettingCommand(configurationId), cancellationToken);
            return Results.Ok();
        })
        .WithName("ResetGeneralSetting")
        .WithDescription("Resets a general setting to its default value")
        .Produces(StatusCodes.Status200OK);

        // POST /api/general-settings/reset-all
        group.MapPost("/reset-all", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            await mediator.Send(new ResetAllGeneralSettingsCommand(), cancellationToken);
            return Results.Ok();
        })
        .WithName("ResetAllGeneralSettings")
        .WithDescription("Resets all general settings to their default values")
        .Produces(StatusCodes.Status200OK);
    }
}

public record UpdateGeneralSettingsRequest(List<GeneralSettingUpdateDto> Settings);
