using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.API;

public sealed class FluentValidationEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (context.Arguments is null || context.Arguments.Count == 0)
            return await next(context);

        List<FluentValidation.Results.ValidationFailure>? failures = null;

        foreach (var argument in context.Arguments)
        {
            if (argument is null)
                continue;

            var argumentType = argument.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validator is null)
                continue;

            var validationContextType = typeof(ValidationContext<>).MakeGenericType(argumentType);
            var validationContext = (IValidationContext)Activator.CreateInstance(validationContextType, argument)!;

            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                failures ??= new List<FluentValidation.Results.ValidationFailure>();
                failures.AddRange(result.Errors);
            }
        }

        if (failures is { Count: > 0 })
        {
            var errors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());

            return Results.ValidationProblem(errors, "Se encontraron errores de validación");
        }

        return await next(context);
    }
}
