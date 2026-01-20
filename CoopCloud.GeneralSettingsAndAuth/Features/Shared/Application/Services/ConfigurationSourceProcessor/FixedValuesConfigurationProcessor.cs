using System.Text.Json;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.DTOs;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.ConfigurationSourceProcessor;

public class FixedValuesConfigurationProcessor : IConfigurationSourceProcessor
{
    public Task<List<OptionValueDto>> ProcessConfigurationAsync(
        string configuration,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(configuration))
        {
            return Task.FromResult(new List<OptionValueDto>());
        }

        try
        {
            var options = JsonSerializer.Deserialize<List<OptionValueDto>>(configuration);
            return Task.FromResult(options ?? new List<OptionValueDto>());
        }
        catch (JsonException)
        {
            return Task.FromResult(new List<OptionValueDto>());
        }
    }
}
