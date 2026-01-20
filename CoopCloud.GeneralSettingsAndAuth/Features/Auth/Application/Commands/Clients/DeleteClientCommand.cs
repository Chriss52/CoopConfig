using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.Clients;

public record DeleteClientCommand(Guid ClientId) : IRequest<bool>;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteClientCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteClientCommand command, CancellationToken cancellationToken)
    {
        var client = await _context.Set<Client>()
            .FirstOrDefaultAsync(c => c.Id == command.ClientId && !c.IsDeleted, cancellationToken);

        if (client == null)
            return false;

        // Soft delete
        client.IsDeleted = true;
        client.DeletedAt = DateTime.UtcNow;
        client.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
