using System.Text.Json;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.DTOs;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.ConfigurationSourceProcessor;

public class WebServiceConfigurationProcessor : IConfigurationSourceProcessor
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WebServiceConfigurationProcessor(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<OptionValueDto>> ProcessConfigurationAsync(
        string configuration,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(configuration))
        {
            return new List<OptionValueDto>();
        }

        try
        {
            var config = JsonSerializer.Deserialize<WebServiceConfig>(configuration);
            if (config == null || string.IsNullOrWhiteSpace(config.Url))
            {
                return new List<OptionValueDto>();
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(config.Url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var options = JsonSerializer.Deserialize<List<OptionValueDto>>(content);
            
            return options ?? new List<OptionValueDto>();
        }
        catch (Exception)
        {
            return new List<OptionValueDto>();
        }
    }

    private class WebServiceConfig
    {
        public string Url { get; set; } = string.Empty;
    }
}
