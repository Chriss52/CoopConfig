using System.Data;
using System.Text.Json;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.ConfigurationSourceProcessor;

public class StoredProcedureConfigurationProcessor : IConfigurationSourceProcessor
{
    private readonly AppDbContext _context;

    public StoredProcedureConfigurationProcessor(AppDbContext context)
    {
        _context = context;
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
            var config = JsonSerializer.Deserialize<StoredProcedureConfig>(configuration);
            if (config == null || string.IsNullOrWhiteSpace(config.ProcedureName))
            {
                return new List<OptionValueDto>();
            }

            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync(cancellationToken);

            using var command = connection.CreateCommand();
            command.CommandText = config.ProcedureName;
            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var options = new List<OptionValueDto>();

            while (await reader.ReadAsync(cancellationToken))
            {
                var label = reader.GetString(0);
                var value = reader.GetString(1);
                options.Add(new OptionValueDto(label, value));
            }

            return options;
        }
        catch (Exception)
        {
            return new List<OptionValueDto>();
        }
    }

    private class StoredProcedureConfig
    {
        public string ProcedureName { get; set; } = string.Empty;
    }
}
