using API.Domain;
using Microsoft.EntityFrameworkCore;

namespace API.Jobs;

public class AcademyReportJob(ILogger<AcademyReportJob> logger, AppDbContext dbContext)
{
    public async Task GenerateReport()
    {
        var players = await dbContext.Players.CountAsync();
        var teams = await dbContext.Teams.CountAsync();
        
        logger.LogInformation("Generating academy report...");
        
        await Task.Delay(1000);
        logger.LogInformation("[Report is ready!. Actually we have {teams} teams and {players} players.", teams, players);
    }
}