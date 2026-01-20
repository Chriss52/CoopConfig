using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using FluentValidation;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido")
            .MaximumLength(256).WithMessage("El correo electrónico no puede exceder 256 caracteres");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es requerido")
            .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder 50 caracteres")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("El nombre de usuario solo puede contener letras, números y guiones bajos");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es requerido")
            .MaximumLength(200).WithMessage("El nombre completo no puede exceder 200 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("El número de teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new RegisterRequestValidator());
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida");
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new LoginRequestValidator());
    }
}

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("El token de actualización es requerido");
    }
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new RefreshTokenRequestValidator());
    }
}

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("La contraseña actual es requerida");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La nueva contraseña es requerida")
            .MinimumLength(8).WithMessage("La nueva contraseña debe tener al menos 8 caracteres")
            .MaximumLength(100).WithMessage("La nueva contraseña no puede exceder 100 caracteres")
            .Matches("[A-Z]").WithMessage("La nueva contraseña debe contener al menos una letra mayúscula")
            .Matches("[a-z]").WithMessage("La nueva contraseña debe contener al menos una letra minúscula")
            .Matches("[0-9]").WithMessage("La nueva contraseña debe contener al menos un número")
            .NotEqual(x => x.CurrentPassword).WithMessage("La nueva contraseña debe ser diferente a la actual");
    }
}

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El ID de usuario es requerido");

        RuleFor(x => x.Request).SetValidator(new ChangePasswordRequestValidator());
    }
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido")
            .MaximumLength(256).WithMessage("El correo electrónico no puede exceder 256 caracteres");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es requerido")
            .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder 50 caracteres")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("El nombre de usuario solo puede contener letras, números y guiones bajos");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es requerido")
            .MaximumLength(200).WithMessage("El nombre completo no puede exceder 200 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("El número de teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new CreateUserRequestValidator());
    }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido")
            .MaximumLength(256).WithMessage("El correo electrónico no puede exceder 256 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Username)
            .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder 50 caracteres")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("El nombre de usuario solo puede contener letras, números y guiones bajos")
            .When(x => !string.IsNullOrEmpty(x.Username));

        RuleFor(x => x.FullName)
            .MaximumLength(200).WithMessage("El nombre completo no puede exceder 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("El número de teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El ID de usuario es requerido");

        RuleFor(x => x.Request).SetValidator(new UpdateUserRequestValidator());
    }
}

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del rol es requerido")
            .MaximumLength(100).WithMessage("El nombre del rol no puede exceder 100 caracteres");
    }
}

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new CreateRoleRequestValidator());
    }
}

public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("El nombre del rol no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Name));
    }
}

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("El ID del rol es requerido");

        RuleFor(x => x.Request).SetValidator(new UpdateRoleRequestValidator());
    }
}

public class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequest>
{
    public CreatePermissionRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del permiso es requerido")
            .MaximumLength(100).WithMessage("El nombre del permiso no puede exceder 100 caracteres");

        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("La clave del permiso es requerida")
            .MaximumLength(50).WithMessage("La clave del permiso no puede exceder 50 caracteres")
            .Matches("^[a-z0-9._-]+$").WithMessage("La clave del permiso solo puede contener letras minúsculas, números, puntos, guiones y guiones bajos");
    }
}

public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new CreatePermissionRequestValidator());
    }
}

public class UpdatePermissionRequestValidator : AbstractValidator<UpdatePermissionRequest>
{
    public UpdatePermissionRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("El nombre del permiso no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Key)
            .MaximumLength(50).WithMessage("La clave del permiso no puede exceder 50 caracteres")
            .Matches("^[a-z0-9._-]+$").WithMessage("La clave del permiso solo puede contener letras minúsculas, números, puntos, guiones y guiones bajos")
            .When(x => !string.IsNullOrEmpty(x.Key));
    }
}

public class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionCommandValidator()
    {
        RuleFor(x => x.PermissionId)
            .NotEmpty().WithMessage("El ID del permiso es requerido");

        RuleFor(x => x.Request).SetValidator(new UpdatePermissionRequestValidator());
    }
}

public class AssignRolesToUserRequestValidator : AbstractValidator<AssignRolesToUserRequest>
{
    public AssignRolesToUserRequestValidator()
    {
        RuleFor(x => x.RoleIds)
            .NotNull().WithMessage("La lista de roles es requerida");
    }
}

public class AssignRolesToUserCommandValidator : AbstractValidator<AssignRolesToUserCommand>
{
    public AssignRolesToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El ID de usuario es requerido");

        RuleFor(x => x.Request).SetValidator(new AssignRolesToUserRequestValidator());
    }
}

public class AssignPermissionsToRoleRequestValidator : AbstractValidator<AssignPermissionsToRoleRequest>
{
    public AssignPermissionsToRoleRequestValidator()
    {
        RuleFor(x => x.PermissionIds)
            .NotNull().WithMessage("La lista de permisos es requerida");
    }
}

public class AssignPermissionsToRoleCommandValidator : AbstractValidator<AssignPermissionsToRoleCommand>
{
    public AssignPermissionsToRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("El ID del rol es requerido");

        RuleFor(x => x.Request).SetValidator(new AssignPermissionsToRoleRequestValidator());
    }
}

public class ValidateTokenRequestValidator : AbstractValidator<ValidateTokenRequest>
{
    public ValidateTokenRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("El token es requerido");
    }
}
