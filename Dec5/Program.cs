var lines = await ReadInputAsync();
var singleSeeds = lines[0][7..]
    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(long.Parse)
    .ToList();

var seedRanges = Enumerable.Range(0, singleSeeds.Count)
    .Where(int.IsEvenInteger)
    .Select(i => new SeedRange(singleSeeds[i], singleSeeds[i + 1]))
    .ToList();

Dictionary<string, List<MappingRange>> mappingRanges = [];
var currentMappingRangeName = string.Empty;

foreach (var line in lines[1..])
{
    if (char.IsLetter(line[0]))
    {
        currentMappingRangeName = line[..^5];
        mappingRanges[currentMappingRangeName] = [];
        continue;
    }
    var range = line.Split(' ').Select(long.Parse).ToArray();

    mappingRanges[currentMappingRangeName].Add(new(range[0], range[1], range[2]));
}

var mappingRangeNames = mappingRanges.Keys;

var lowestLocationSingleSeed = singleSeeds.Select(MapSeed).Min();

var lowestLocation = seedRanges.SelectMany(SeedListFromRange).AsParallel().Select(MapSeed).Min();

Console.WriteLine(lowestLocationSingleSeed);
Console.WriteLine(lowestLocation);

static async Task<string[]> ReadInputAsync()
{
    using StreamReader reader = new("input.txt");

    return (await reader.ReadToEndAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

long MapSeed(long seed)
{
    foreach (var mappingRangeName in mappingRangeNames)
    {
        foreach (var mappingRange in mappingRanges[mappingRangeName])
        {
            if (mappingRange.TryMap(seed, out var output))
            {
                seed = output;
                break;
            }
        }
    }

    return seed;
}

static List<long> SeedListFromRange(SeedRange seedRange)
{
    checked 
    {
        List<long> seedList = [];
        for (var seedOffset = 0; seedOffset < seedRange.Length; seedOffset++)
            seedList.Add(seedRange.RangeStart + seedOffset);
        return seedList;
    }
}

public record struct MappingRange(long DestinationRangeStart, long SourceRangeStart, long Length)
{
    public readonly bool TryMap(long input, out long output)
    {
        checked
        {
            if (SourceRangeStart <= input && input < SourceRangeStart + Length)
            {
                output = DestinationRangeStart + (input - SourceRangeStart);
                return true;
            }
            output = input;
            return false;
        }
    }
}

public record struct SeedRange(long RangeStart, long Length);