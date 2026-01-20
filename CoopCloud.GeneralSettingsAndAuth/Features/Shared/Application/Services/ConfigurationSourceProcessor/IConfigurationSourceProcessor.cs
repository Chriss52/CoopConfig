using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.DTOs;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.ConfigurationSourceProcessor;

public interface IConfigurationSourceProcessor
{
    Task<List<OptionValueDto>> ProcessConfigurationAsync(
        string configuration,
        CancellationToken cancellationToken = default
    );
}
