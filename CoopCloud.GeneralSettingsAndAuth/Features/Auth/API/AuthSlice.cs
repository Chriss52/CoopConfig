using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.Clients;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nubeteck.Extensions.Security;
using Nubeteck.Extensions.Web;
using LoginRequest = CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs.LoginRequest;
using LoginResponse = CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs.LoginResponse;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.API;

public class AuthSlice : ISlice
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Services are already registered in Program.cs
    }

    public void RegisterRoutes(IEndpointRouteBuilder app)
    {
        RegisterAuthEndpoints(app);
        RegisterUserEndpoints(app);
        RegisterRoleEndpoints(app);
        RegisterPermissionEndpoints(app);
        RegisterClientEndpoints(app);
    }

    private void RegisterAuthEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        // POST /api/auth/register - Public endpoint
        group.MapPost("/register", async (RegisterRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RegisterUserCommand(request), cancellationToken);
            return Results.Created($"/api/users/{result.Id}", result);
        })
        .WithName("Register")
        .WithDescription("Register a new user account")
        .Produces<UserDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /api/auth/login - Public endpoint
        group.MapPost("/login", async (LoginRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new LoginCommand(request), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("Login")
        .WithDescription("Authenticate user and get JWT token")
        .Produces<LoginResponse>()
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /api/auth/refresh-token - Public endpoint
        group.MapPost("/refresh-token", async (RefreshTokenRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RefreshTokenCommand(request), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("RefreshToken")
        .WithDescription("Refresh JWT token using refresh token")
        .Produces<LoginResponse>()
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /api/auth/logout - Authenticated endpoint
        group.MapPost("/logout", async (RefreshTokenRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new LogoutCommand(request.RefreshToken), cancellationToken);
            return result ? Results.Ok(new { Message = "Sesión cerrada exitosamente" }) : Results.BadRequest(new { Message = "Token inválido" });
        })
        .WithName("Logout")
        .WithDescription("Logout and revoke refresh token")
        .RequireAuthorization()
        .Produces<object>()
        .Produces(StatusCodes.Status400BadRequest);

        // POST /api/auth/change-password - Authenticated endpoint
        group.MapPost("/change-password", async (ChangePasswordRequest request, IMediator mediator, ICurrentUserService currentUser, CancellationToken cancellationToken) =>
        {
            var userId = currentUser.UserId;
            if (!userId.HasValue || userId == Guid.Empty)
                return Results.Unauthorized();

            await mediator.Send(new ChangePasswordCommand(userId.Value, request), cancellationToken);
            return Results.Ok(new { Message = "Contraseña actualizada exitosamente" });
        })
        .WithName("ChangePassword")
        .WithDescription("Change current user password")
        .RequireAuthorization()
        .Produces<object>()
        .Produces(StatusCodes.Status401Unauthorized);

        // GET /api/auth/me - Authenticated endpoint
        group.MapGet("/me", async (IMediator mediator, ICurrentUserService currentUser, CancellationToken cancellationToken) =>
        {
            var userId = currentUser.UserId;
            if (!userId.HasValue || userId == Guid.Empty)
                return Results.Unauthorized();

            var result = await mediator.Send(new GetCurrentUserQuery(userId.Value), cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetCurrentUser")
        .WithDescription("Get current authenticated user profile")
        .RequireAuthorization()
        .Produces<UserDetailDto>()
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/auth/validate-token - Public endpoint for other services
        group.MapPost("/validate-token", async (ValidateTokenRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new ValidateTokenQuery(request), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("ValidateToken")
        .WithDescription("Validate a JWT token and return user info (for use by other services)")
        .Produces<ValidateTokenResponse>();
    }

    private void RegisterUserEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        // GET /api/users
        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetAllUsersQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetAllUsers")
        .WithDescription("Get all users")
        .Produces<IEnumerable<UserDto>>();

        // GET /api/users/{id}
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetUserById")
        .WithDescription("Get user by ID")
        .Produces<UserDetailDto>()
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/users
        group.MapPost("/", async (CreateUserRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new CreateUserCommand(request), cancellationToken);
            return Results.Created($"/api/users/{result.Id}", result);
        })
        .WithName("CreateUser")
        .WithDescription("Create a new user")
        .Produces<UserDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // PUT /api/users/{id}
        group.MapPut("/{id:guid}", async (Guid id, UpdateUserRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new UpdateUserCommand(id, request), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("UpdateUser")
        .WithDescription("Update user information")
        .Produces<UserDto>()
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /api/users/{id}
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteUserCommand(id), cancellationToken);
            return result ? Results.Ok(new { Message = "Usuario eliminado exitosamente" }) : Results.NotFound();
        })
        .WithName("DeleteUser")
        .WithDescription("Delete a user (soft delete)")
        .Produces<object>()
        .Produces(StatusCodes.Status404NotFound);

        // PUT /api/users/{id}/roles
        group.MapPut("/{id:guid}/roles", async (Guid id, AssignRolesToUserRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new AssignRolesToUserCommand(id, request), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("AssignRolesToUser")
        .WithDescription("Assign roles to a user")
        .Produces<UserDto>()
        .Produces(StatusCodes.Status404NotFound);
    }

    private void RegisterRoleEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles")
            .WithTags("Roles")
            .RequireAuthorization();

        // GET /api/roles
        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetAllRolesQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetAllRoles")
        .WithDescription("Get all roles")
        .Produces<IEnumerable<RoleDto>>();

        // GET /api/roles/{id}
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetRoleById")
        .WithDescription("Get role by ID with permissions and users")
        .Produces<RoleDetailDto>()
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/roles
        group.MapPost("/", async (CreateRoleRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new CreateRoleCommand(request), cancellationToken);
            return Results.Created($"/api/roles/{result.Id}", result);
        })
        .WithName("CreateRole")
        .WithDescription("Create a new role")
        .Produces<RoleDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // PUT /api/roles/{id}
        group.MapPut("/{id:guid}", async (Guid id, UpdateRoleRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new UpdateRoleCommand(id, request), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("UpdateRole")
        .WithDescription("Update role information")
        .Produces<RoleDto>()
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /api/roles/{id}
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteRoleCommand(id), cancellationToken);
            return result ? Results.Ok(new { Message = "Rol eliminado exitosamente" }) : Results.NotFound();
        })
        .WithName("DeleteRole")
        .WithDescription("Delete a role (soft delete)")
        .Produces<object>()
        .Produces(StatusCodes.Status404NotFound);

        // PUT /api/roles/{id}/permissions
        group.MapPut("/{id:guid}/permissions", async (Guid id, AssignPermissionsToRoleRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new AssignPermissionsToRoleCommand(id, request), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("AssignPermissionsToRole")
        .WithDescription("Assign permissions to a role")
        .Produces<RoleDto>()
        .Produces(StatusCodes.Status404NotFound);
    }

    private void RegisterPermissionEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/permissions")
            .WithTags("Permissions")
            .RequireAuthorization();

        // GET /api/permissions
        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetAllPermissionsQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetAllPermissions")
        .WithDescription("Get all permissions")
        .Produces<IEnumerable<PermissionDto>>();

        // POST /api/permissions
        group.MapPost("/", async (CreatePermissionRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new CreatePermissionCommand(request), cancellationToken);
            return Results.Created($"/api/permissions/{result.Id}", result);
        })
        .WithName("CreatePermission")
        .WithDescription("Create a new permission")
        .Produces<PermissionDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // PUT /api/permissions/{id}
        group.MapPut("/{id:guid}", async (Guid id, UpdatePermissionRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new UpdatePermissionCommand(id, request), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("UpdatePermission")
        .WithDescription("Update permission information")
        .Produces<PermissionDto>()
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /api/permissions/{id}
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeletePermissionCommand(id), cancellationToken);
            return result ? Results.Ok(new { Message = "Permiso eliminado exitosamente" }) : Results.NotFound();
        })
        .WithName("DeletePermission")
        .WithDescription("Delete a permission (soft delete)")
        .Produces<object>()
        .Produces(StatusCodes.Status404NotFound);
    }

    private void RegisterClientEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clients")
            .WithTags("OAuth Clients")
            .RequireAuthorization();

        // GET /api/clients
        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetAllClientsQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetAllClients")
        .WithDescription("Get all registered OAuth clients")
        .Produces<IEnumerable<ClientDto>>();

        // GET /api/clients/{id}
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetClientByIdQuery(id), cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetClientById")
        .WithDescription("Get OAuth client by ID")
        .Produces<ClientDetailDto>()
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/clients
        group.MapPost("/", async (CreateClientRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new CreateClientCommand(request), cancellationToken);
            return Results.Created($"/api/clients/{result.Id}", result);
        })
        .WithName("CreateClient")
        .WithDescription("Register a new OAuth client application")
        .Produces<ClientDetailDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // PUT /api/clients/{id}
        group.MapPut("/{id:guid}", async (Guid id, UpdateClientRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new UpdateClientCommand(id, request), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("UpdateClient")
        .WithDescription("Update OAuth client information")
        .Produces<ClientDto>()
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /api/clients/{id}
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteClientCommand(id), cancellationToken);
            return result ? Results.Ok(new { Message = "Cliente eliminado exitosamente" }) : Results.NotFound();
        })
        .WithName("DeleteClient")
        .WithDescription("Delete an OAuth client (soft delete)")
        .Produces<object>()
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/clients/{id}/regenerate-secret
        group.MapPost("/{id:guid}/regenerate-secret", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RegenerateClientSecretCommand(id), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("RegenerateClientSecret")
        .WithDescription("Regenerate the client secret (only for confidential clients)")
        .Produces<RegenerateClientSecretResponse>()
        .Produces(StatusCodes.Status400BadRequest);

        // POST /api/clients/{id}/redirect-uris
        group.MapPost("/{id:guid}/redirect-uris", async (Guid id, AddRedirectUriRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new AddRedirectUriCommand(id, request), cancellationToken);
            return Results.Created($"/api/clients/{id}/redirect-uris/{result.Id}", result);
        })
        .WithName("AddRedirectUri")
        .WithDescription("Add a redirect URI to a client")
        .Produces<ClientRedirectUriDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // DELETE /api/clients/{id}/redirect-uris/{uriId}
        group.MapDelete("/{id:guid}/redirect-uris/{uriId:guid}", async (Guid id, Guid uriId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RemoveRedirectUriCommand(id, uriId), cancellationToken);
            return result ? Results.Ok(new { Message = "Redirect URI eliminada exitosamente" }) : Results.NotFound();
        })
        .WithName("RemoveRedirectUri")
        .WithDescription("Remove a redirect URI from a client")
        .Produces<object>()
        .Produces(StatusCodes.Status404NotFound);
    }
}
