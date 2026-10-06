using API.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Methods.Player;

public sealed record GetAllQuery() : IRequest<List<Entities.Player>>;

public class GetAllHandler(AppDbContext dbContext) : IRequestHandler<GetAllQuery, List<Entities.Player>>
{
    public async Task<List<Entities.Player>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Players.ToListAsync(cancellationToken);
    }
}
