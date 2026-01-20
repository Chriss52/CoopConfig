using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Repositories;
using FluentValidation;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Commands;

public class UpdateGeneralSettingsValidator : AbstractValidator<UpdateGeneralSettingsCommand>
{
    private readonly IServiceProvider _serviceProvider;

    public UpdateGeneralSettingsValidator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        RuleFor(x => x.Settings)
            .NotEmpty()
            .WithMessage("At least one setting must be provided");

        RuleForEach(x => x.Settings)
            .MustAsync(ValidateByDataType)
            .WithMessage("Invalid value for the setting's data type");
    }

    private async Task<bool> ValidateByDataType(GeneralSettingUpdateDto dto, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IGeneralSettingRepository>();

        var setting = await repository.GetByIdAsync(dto.ConfigurationId, cancellationToken);
        if (setting == null) return false;

        var dataTypeName = setting.DataType?.DataTypeName;

        return dataTypeName switch
        {
            "Numero" => ValidateNumero(dto.Value),
            "Booleano" => ValidateBooleano(dto.Value),
            "Email" => ValidateEmail(dto.Value),
            "Opciones" => await ValidateOpciones(dto.ConfigurationId, dto.Value, cancellationToken),
            "MultiOpciones" => await ValidateMultiOpciones(dto.ConfigurationId, dto.Value, cancellationToken),
            _ => !string.IsNullOrWhiteSpace(dto.Value)
        };
    }

    private bool ValidateNumero(string value)
    {
        return decimal.TryParse(value, out _);
    }

    private bool ValidateBooleano(string value)
    {
        return value.ToLower() == "true" || value.ToLower() == "false";
    }

    private bool ValidateEmail(string value)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(value);
            return addr.Address == value;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> ValidateOpciones(Guid configurationId, string value, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IGeneralSettingRepository>();
        var option = await repository.GetOptionByIdAsync(configurationId, cancellationToken);

        if (option == null) return false;

        return !string.IsNullOrWhiteSpace(value);
    }

    private async Task<bool> ValidateMultiOpciones(Guid configurationId, string value, CancellationToken cancellationToken)
    {
        var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0) return false;

        return values.All(v => !string.IsNullOrWhiteSpace(v.Trim()));
    }
}
