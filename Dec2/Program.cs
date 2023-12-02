using var reader = new StreamReader("input.txt");

var text = await reader.ReadToEndAsync();
var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

var games = GetGameInfo(lines);

Console.WriteLine(games.Aggregate(0, IDAdder));
Console.WriteLine(games.Aggregate(0, (prior, game) => prior + game.Power));

static Game[] GetGameInfo(string[] gameLines) => 
    gameLines.Select(line => 
    {
        var ID = int.Parse(line[4..line.IndexOf(':')]);
        var gameData = line[(line.IndexOf(':')+2)..];
        var matches = gameData.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(GetMatchFromMatchData).ToArray();
        return new Game(ID, matches);
    }).ToArray();

static Match GetMatchFromMatchData(string matchData)
{
    var colours = matchData.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    int reds, greens, blues;
    reds = greens = blues = 0;
    foreach (var colour in colours)
    {
        var data = colour.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        switch (data[1])
        {
            case "red":
                reds += int.Parse(data[0]);
                break;
            case "green":
                greens += int.Parse(data[0]);
                break;
            case "blue":
                blues += int.Parse(data[0]);
                break;
            default:
                throw new InvalidDataException();
        }
    }
    return new Match(reds, greens, blues);
}

static int IDAdder(int priorResult, Game current) => current.Matches.Any(IsInvalid) ? priorResult : priorResult + current.ID;

static bool IsInvalid(Match match) => GreatestPossibleMatch.Reds < match.Reds || GreatestPossibleMatch.Greens < match.Greens || GreatestPossibleMatch.Blues < match.Blues;

public static partial class Program 
{
    public static Match GreatestPossibleMatch = new(12, 13, 14);
}

public record struct Game(int ID, Match[] Matches)
{
    public readonly int Power => MaxRed * MaxGreen * MaxBlue;

    private readonly int MaxRed => Matches.Max(m => m.Reds);

    private readonly int MaxGreen => Matches.Max(m => m.Greens);

    private readonly int MaxBlue => Matches.Max(m => m.Blues);     
}

public record struct Match(int Reds, int Greens, int Blues);