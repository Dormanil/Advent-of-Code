using var reader = new StreamReader("input.txt");

var text = await reader.ReadToEndAsync();
var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

var sumOfCalibrationValues = lines.Aggregate(0, (int previous, string current) => previous + GetCalibrationValues(current));
var sumOfCalibrationValuesConverted = lines.Aggregate(0, (int previous, string current) => previous + GetCalibrationValues(current, true));

static int GetCalibrationValues(ReadOnlySpan<char> calibrationLine, bool convertWords = false)
{
    var first = GetFirst(calibrationLine.IndexOfAny(numberSearchValues), calibrationLine, convertWords);
    var last = GetLast(calibrationLine.LastIndexOfAny(numberSearchValues), calibrationLine, convertWords);
    ReadOnlySpan<char> cv = [first, last];
    return int.Parse(cv);
}

static char GetFirst(int numberIndex, ReadOnlySpan<char> calibrationLine, bool convertWords = false)
{
    if (!convertWords)
        return calibrationLine[numberIndex];
        
    var lowestKnownIndex = numberIndex;
    var lowestIndexByNumber = new Dictionary<int, string>();
    foreach (var number in numbers)
    {
        var i = calibrationLine.IndexOf(number);
        lowestIndexByNumber[i] = number;
        if (lowestKnownIndex > i && i > -1)
            lowestKnownIndex = i;
    }
    if (lowestKnownIndex == numberIndex)
        return calibrationLine[numberIndex];
    
    return numbersToDigits[lowestIndexByNumber[lowestKnownIndex]];
}

static char GetLast(int numberIndex, ReadOnlySpan<char> calibrationLine, bool convertWords = false)
{
    if (!convertWords)
        return calibrationLine[numberIndex];
        
    var highestKnownIndex = numberIndex;
    var highestIndexByNumber = new Dictionary<int, string>();
    foreach (var number in numbers)
    {
        var i = calibrationLine.LastIndexOf(number);
        highestIndexByNumber[i] = number;
        if (highestKnownIndex < i && i > -1)
            highestKnownIndex = i;
    }
    if (highestKnownIndex == numberIndex)
        return calibrationLine[numberIndex];
    
    return numbersToDigits[highestIndexByNumber[highestKnownIndex]];
}

Console.WriteLine(sumOfCalibrationValues);
Console.WriteLine(sumOfCalibrationValuesConverted);

partial class Program
{
    private static readonly System.Buffers.SearchValues<char> numberSearchValues = System.Buffers.SearchValues.Create("123456789");
    private static readonly string[] numbers = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
    private static readonly Dictionary<string, char> numbersToDigits = new()
    {
        ["one"] = '1',
        ["two"] ='2',
        ["three"] = '3',
        ["four"] = '4',
        ["five"] = '5',
        ["six"] = '6',
        ["seven"] = '7',
        ["eight"] = '8',
        ["nine"] = '9',
    };
}