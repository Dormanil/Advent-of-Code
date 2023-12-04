using StreamReader reader = new("input.txt");

var text = await reader.ReadToEndAsync();
var cards = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Select(l => 
        new{
            id = l[4..l.IndexOf(':')].Trim(), 
            winningNumbers = l[(l.IndexOf(':') + 1)..l.IndexOf('|')].Trim(), 
            numbersDrawn = l[(l.IndexOf('|') + 1)..].Trim()
        })
    .Select(c => 
        new Card(
            int.Parse(c.id), 
            c.winningNumbers.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(wn => int.Parse(wn)).ToArray(),
            c.numbersDrawn.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(nd => int.Parse(nd)).ToArray()
        ))
    .ToList();

Dictionary<int,int> cardsAndInstances = cards.Select(c => (c.ID, instances: 1)).ToDictionary();
var highestID = cardsAndInstances.Keys.Max();

cards.ForEach(card => 
{
    AddNextCards(cardsAndInstances[card.ID], card.ID, card.MatchingNumberCount);
});

Console.WriteLine(cards.Select(c => c.Points).Sum());
Console.WriteLine(cardsAndInstances.Values.Sum());

void AddNextCards(int multiplier, int currentID, int copiesToAdd)
{
    if (currentID >= highestID)
        return;
    if (copiesToAdd == 0)
        return;
    cardsAndInstances[++currentID] += multiplier;
    AddNextCards(multiplier, currentID, copiesToAdd - 1);
}

public record struct Card(int ID, int[] WinningNumbers, int[] NumbersDrawn)
{
    public readonly int Points
    {
        get
        {
            var winningNumbers = WinningNumbers;
            return NumbersDrawn.Where(num => winningNumbers.Contains(num)).Aggregate(0, (prev, _) => prev == 0 ? 1 : prev << 1);
        }
    }

    public readonly int MatchingNumberCount 
    {
        get 
        {
            var winningNumbers = WinningNumbers;
            return NumbersDrawn.Where(num => winningNumbers.Contains(num)).Count();
        }
    }
}