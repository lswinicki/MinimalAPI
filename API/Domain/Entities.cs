namespace API.Domain;

public class Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Player> Players { get; set; } = new();
    }

    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;
    }
    
}