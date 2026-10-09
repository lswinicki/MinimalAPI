using API;
using API.Domain;
using API.Exceptions;
using API.Exceptions.Handlers;
using API.Helpers.Behaviors;
using API.Methods.Player;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>)); 
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=academy.db")
        .LogTo(Console.WriteLine, LogLevel.Information));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();

app.MapGet("/test-error", () => { throw new DomainException("Test Error from API"); });

app.MapGet("/nplus1-bad", async (AppDbContext db) =>
{
    var teams = await db.Teams.ToListAsync();

    var result = new List<object>();
    foreach (var team in teams)
    {
        var players = await db.Players.Where(p => p.TeamId == team.Id).ToListAsync();
        result.Add(new { team.Name, PlayersCount = players.Count });
    }

    return result;
});

app.MapGet("/nplus1-good", async (AppDbContext db) =>
{
    var result = await db.Teams
        .Select(t => new { t.Name, PlayerCount = t.Players.Count })
        .ToListAsync();
    return result;
});

app.MapPost("/teams/{teamId}/players", async (int teamId, [FromBody] CreatePlayerRequest request, IMediator mediator) =>
{
    var command = new CreatePlayerCommand(request.Name, teamId);

    var playerId = await mediator.Send(command);

    return Results.Ok(playerId);
});


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Teams.Any())
    {
        var team1 = new Entities.Team { Name = "Team 1" };
        var team2 = new Entities.Team { Name = "Team 2" };
        db.Teams.AddRange(team1, team2);

        db.Players.AddRange(
            new Entities.Player { Name = "Tom", Team = team1 },
            new Entities.Player { Name = "John", Team = team1 },
            new Entities.Player { Name = "Peter", Team = team2 },
            new Entities.Player { Name = "Jacob", Team = team2 }
        );
        db.SaveChanges();
    }
}

app.Run();