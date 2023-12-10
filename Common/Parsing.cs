namespace AoC.Common;

public static class Parsing
{
    public static async Task<string[]> ReadInputAsync()
    {
        using StreamReader reader = new("input.txt");

        return (await reader.ReadToEndAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
