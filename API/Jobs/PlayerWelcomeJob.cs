using API.Domain;
using API.Methods.Player;

namespace API.Jobs;

public class PlayerWelcomeJob(ILogger<PlayerWelcomeJob> logger, AppDbContext dbContext)
{
    public async Task SendWelcomeEmail(int playerId)
    {
        var player = await dbContext.Players.FindAsync(playerId);
        if (player == null) return;
        
        logger.LogInformation("Start generating mail...");

        await Task.Delay(1000);
        
        logger.LogInformation("Sending welcome email to {playerId}...", playerId);
    }
}