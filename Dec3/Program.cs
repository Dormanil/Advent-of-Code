using System.Buffers;

using var reader = new StreamReader("input.txt");

var text = await reader.ReadToEndAsync();
var diagram = text.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(t => t.ToCharArray()).ToArray();

var coordinates = FindCoordinatesOfSymbols(diagram);
var partNumbers = coordinates.Select(c => FindPartNumbersAdjacentToCoordinate(diagram, c)).ToList();

Console.WriteLine(partNumbers.Flatten().Sum());
Console.WriteLine(partNumbers.Where(s => s.Count == 2).Select(s => s.Aggregate(1, (prev, curr) => prev * curr)).Sum());

static ICollection<Coordinate> FindCoordinatesOfSymbols(char[][] diagram)
{
    List<Coordinate> coordinates = [];
    for (int y = 0; y < diagram.Length; y++)
        for (int x = 0; x < diagram[y].Length; x++)
            if (symbolSearchValues.Contains(diagram[y][x]))
                coordinates.Add(new(x,y));
    return coordinates;
}

static ICollection<int> FindPartNumbersAdjacentToCoordinate(char[][] diagram, Coordinate coordinate)
{
    HashSet<int> partNumbers = [];

    foreach(var adjCoord in GetAdjacentCoordinates())
    {
        var partNumber = FindPartNumberAtCoordinate(adjCoord);
        if (partNumber.HasValue)
            partNumbers.Add(partNumber.Value);
    }

    return partNumbers;

    Coordinate[] GetAdjacentCoordinates()
    {
        var MaxWidth  = diagram[0].Length - 1;
        var MaxHeight = diagram.Length - 1;

        // Corners
        if (coordinate.X == 0 && coordinate.Y == 0)
            return [new(1, 0), new(0, 1), new (1, 1)];
        if (coordinate.X == MaxWidth && coordinate.Y == 0)
            return [new(MaxWidth - 1, 0), new (MaxWidth - 1, 1), new(MaxWidth, 1)];
        if (coordinate.X == 0 && coordinate.Y == MaxHeight)
            return [new(0, MaxHeight - 1), new (1, MaxHeight - 1), new(1, MaxHeight)];
        if (coordinate.X == MaxWidth && coordinate.Y == MaxHeight)
            return [new(MaxWidth - 1, MaxHeight - 1), new(MaxWidth, MaxHeight - 1), new(MaxWidth - 1, MaxHeight)];
        
        // Left side
        if (coordinate.X == 0)
            return [new(0, coordinate.Y - 1), new(1, coordinate.Y - 1), new(1, coordinate.Y), new(0, coordinate.Y + 1), new(1, coordinate.Y + 1)];

        // Right side
        if (coordinate.X == MaxWidth)
            return [new(MaxWidth - 1, coordinate.Y - 1), new(MaxWidth, coordinate.Y - 1), new(MaxWidth - 1, coordinate.Y), new(MaxWidth - 1, coordinate.Y + 1), new(MaxWidth, coordinate.Y + 1)];
        
        // Top side
        if (coordinate.Y == 0)
            return [new(coordinate.X - 1, 0), new(coordinate.X + 1, 0), new(coordinate.X - 1, 1), new(coordinate.X, 1), new(coordinate.X + 1, 1)];

        // Bottom side
        if (coordinate.Y == MaxHeight)
            return [new(coordinate.X - 1, MaxHeight - 1), new(coordinate.X, MaxHeight - 1), new(coordinate.X + 1, MaxHeight - 1), new(coordinate.X - 1, MaxHeight), new(coordinate.X + 1, MaxHeight)];
        
        // Anywhere else
        return 
        [
            new(coordinate.X - 1, coordinate.Y - 1), new(coordinate.X, coordinate.Y - 1), new(coordinate.X + 1, coordinate.Y - 1),
            new(coordinate.X - 1, coordinate.Y),                                          new(coordinate.X + 1, coordinate.Y),
            new(coordinate.X - 1, coordinate.Y + 1), new(coordinate.X, coordinate.Y + 1), new(coordinate.X + 1, coordinate.Y + 1)
        ];
    }

    int? FindPartNumberAtCoordinate(Coordinate coordinate)
    {
        var MaxWidth  = diagram[0].Length - 1;
        var MaxHeight = diagram.Length - 1;

        if (!numberSearchValues.Contains(diagram[coordinate.Y][coordinate.X]))
            return null;
        
        var offset = 0;

        while (coordinate.X + offset > 0 && numberSearchValues.Contains(diagram[coordinate.Y][coordinate.X + offset - 1]))
            offset--;

        var number = diagram[coordinate.Y][coordinate.X + offset] - 48;
        offset++;

        while (coordinate.X + offset <= MaxWidth && numberSearchValues.Contains(diagram[coordinate.Y][coordinate.X + offset]))
        {
            number = number * 10 + diagram[coordinate.Y][coordinate.X + offset] - 48;
            offset++;
        }

        return number;
    }
}

/*
Console.WriteLine(GetUniqueCharacters(text).Aggregate("", (s, c) => s + c));

static ICollection<char> GetUniqueCharacters(string text)
{
    var chars = new HashSet<char>();
    foreach (var c in text)
        chars.Add(c);
    return chars;
}
*/
partial class Program
{
    // Found using the commented code above
    private static readonly SearchValues<char> symbolSearchValues = SearchValues.Create("-@*=%/$#+&");
    private static readonly SearchValues<char> numberSearchValues = SearchValues.Create("0123456789");
}

public record struct Coordinate(int X, int Y);

public static class Util
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        return enumerable.SelectMany(i => i);
    }
}