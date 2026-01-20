using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Repositories;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.ConfigurationSourceProcessor;
using MediatR;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Queries;

public class GetGeneralSettingsOptionByIdHandler
    : IRequestHandler<GetGeneralSettingsOptionByIdQuery, GeneralSettingsOptionDto?>
{
    private readonly IGeneralSettingRepository _generalSettingsRepository;
    private readonly ConfigurationSourceProcessorFactory _processorFactory;

    public GetGeneralSettingsOptionByIdHandler(
        IGeneralSettingRepository generalSettingsRepository,
        ConfigurationSourceProcessorFactory processorFactory)
    {
        _generalSettingsRepository = generalSettingsRepository;
        _processorFactory = processorFactory;
    }

    public async Task<GeneralSettingsOptionDto?> Handle(
        GetGeneralSettingsOptionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var option = await _generalSettingsRepository.GetOptionByIdAsync(
            request.ConfigurationId,
            cancellationToken);

        if (option == null || option.Source == null)
        {
            return null;
        }

        var processor = _processorFactory.GetProcessor(option.SourceId);
        var options = await processor.ProcessConfigurationAsync(
            option.Configuration ?? string.Empty,
            cancellationToken);

        return new GeneralSettingsOptionDto(
            option.ConfigurationId,
            option.SourceId,
            option.Source.SourceName,
            options,
            option.AllowMultiple ?? false
        );
    }
}
