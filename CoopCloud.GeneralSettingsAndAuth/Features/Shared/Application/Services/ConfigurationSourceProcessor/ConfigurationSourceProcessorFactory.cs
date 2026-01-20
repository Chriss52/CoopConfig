using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.ConfigurationSourceProcessor;

public class ConfigurationSourceProcessorFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _context;

    public ConfigurationSourceProcessorFactory(IHttpClientFactory httpClientFactory, AppDbContext context)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
    }

    public IConfigurationSourceProcessor GetProcessor(Guid sourceId)
    {
        // GUIDs fijos de Source según el archivo adjunto
        var webServiceSourceId = Guid.Parse("40000000-0000-0000-0000-000000000001");
        var storedProcedureSourceId = Guid.Parse("40000000-0000-0000-0000-000000000002");
        var fixedValuesSourceId = Guid.Parse("40000000-0000-0000-0000-000000000003");

        if (sourceId == webServiceSourceId)
        {
            return new WebServiceConfigurationProcessor(_httpClientFactory);
        }
        else if (sourceId == storedProcedureSourceId)
        {
            return new StoredProcedureConfigurationProcessor(_context);
        }
        else if (sourceId == fixedValuesSourceId)
        {
            return new FixedValuesConfigurationProcessor();
        }
        
        throw new NotSupportedException($"Source ID {sourceId} is not supported");
    }
}
