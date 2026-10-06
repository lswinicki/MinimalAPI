using API.Domain;
using API.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Methods.Player;

public record CreatePlayerRequest(string Name);
public sealed record CreatePlayerCommand(string Name, int TeamId) : IRequest<int>;

public class CreatePlayerHandler(AppDbContext dbContext) : IRequestHandler<CreatePlayerCommand, int>
{
    public async Task<int> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
    {
        var newPlayer = new Entities.Player()
        {
            Name = request.Name,
            TeamId = request.TeamId
        };
        
        var team = await dbContext.Teams.AnyAsync(x => x.Id == request.TeamId, cancellationToken);
        if (!team)
            throw new DomainException("Team not found");
        
        dbContext.Players.Add(newPlayer);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return newPlayer.Id;
    }
}