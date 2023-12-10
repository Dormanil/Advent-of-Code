using AoC.Common;





var lines = await Parsing.ReadInputAsync();
var hands = lines.Select(l => Parse(l)).ToList();
var jokerHands = lines.Select(l => Parse(l, true)).ToList();

hands.Sort();
jokerHands.Sort();

var winnings = hands.Zip(Enumerable.Range(1, hands.Count))
    .Select(t => t.First.Bid * t.Second);

Console.WriteLine(winnings.Sum());

var jokerWinnings = jokerHands.Zip(Enumerable.Range(1, jokerHands.Count))
    .Select(t => t.First.Bid * t.Second);

Console.WriteLine(jokerWinnings.Sum());

static Hand Parse(string line, bool isJokerGame = false)
{
    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    return new(parts[0], int.Parse(parts[1]), isJokerGame);
}

public record struct Hand(string Cards, int Bid, bool IsJokerGame = false) : IComparable<Hand>, IComparer<Hand>, IComparer<string>, IComparer<char>
{
    public readonly int CompareTo(Hand other)
    {
        if (this.Type != other.Type)
            return this.Type - other.Type;
        
        return Compare(this.Cards, other.Cards);
    }

    public readonly HandType Type 
    {
        get
        {
            var distinctCards = Cards.Distinct().ToList();
            return distinctCards switch 
            {
                { Count: 1 } => HandType.FiveOfAKind,
                { Count: 2 } => FourOfAKindOrFullHouse(distinctCards),
                { Count: 3 } => ThreeOfAKindOrTwoPair(distinctCards),
                { Count: 4 } when IsJokerGame && distinctCards.Any(dc => dc == 'J') => HandType.ThreeOfAKind,
                { Count: 4 } => HandType.OnePair,
                { Count: 5 } when IsJokerGame && distinctCards.Any(dc => dc == 'J') => HandType.OnePair,
                _ => HandType.HighCard
            };
        }
    }

    public readonly int Compare(string? x, string? y) 
    {
        if (Compare(x![0], y![0]) > 0)
            return 1;
        if (Compare(x[0], y[0]) < 0)
            return -1;
        if (x.Length == 1 && Compare(x[0], y[0]) == 0)
            return 0;
        return Compare(x[1..], y[1..]);
    }

    public readonly int Compare(char x, char y)
    {
        if (IsJokerGame)
        {
            // A, K, Q, T, 9, 8, 7, 6, 5, 4, 3, 2, or J
            return x switch 
            {
                'A' => y switch 
                {
                    'A' => 0,
                    _   => 1
                },
                'K' => y switch 
                {
                    'A' => -1,
                    'K' => 0,
                    _   => 1
                },
                'Q' => y switch 
                {
                    'A' => -1,
                    'K' => -1,
                    'Q' => 0,
                    _   => 1
                },
                'T' => y switch 
                {
                    'A' => -1,
                    'K' => -1,
                    'Q' => -1,
                    'T' => 0,
                    _   => 1
                },
                '9' => y switch 
                {
                    'A' => -1,
                    'K' => -1,
                    'Q' => -1,
                    'T' => -1,
                    '9' => 0,
                    _   => 1
                },
                '8' => y switch 
                {
                    'A' => -1,
                    'K' => -1,
                    'Q' => -1,
                    'T' => -1,
                    '9' => -1,
                    '8' => 0,
                    _   => 1
                },
                '7' => y switch 
                {
                    'A' => -1,
                    'K' => -1,
                    'Q' => -1,
                    'T' => -1,
                    '9' => -1,
                    '8' => -1,
                    '7' => 0,
                    _   => 1
                },
                '6' => y switch 
                {
                    'A' => -1,
                    'K' => -1,
                    'Q' => -1,
                    'T' => -1,
                    '9' => -1,
                    '8' => -1,
                    '7' => -1,
                    '6' => 0,
                    _   => 1
                },
                '5' => y switch 
                {
                    'A' => -1,
                    'K' => -1,
                    'Q' => -1,
                    'T' => -1,
                    '9' => -1,
                    '8' => -1,
                    '7' => -1,
                    '6' => -1,
                    '5' => 0,
                    _   => 1
                },
                '4' => y switch 
                {
                    'A' => -1,
                    'K' => -1,
                    'Q' => -1,
                    'T' => -1,
                    '9' => -1,
                    '8' => -1,
                    '7' => -1,
                    '6' => -1,
                    '5' => -1,
                    '4' => 0,
                    _   => 1
                },
                '3' => y switch 
                {
                    'A' => -1,
                    'K' => -1,
                    'Q' => -1,
                    'T' => -1,
                    '9' => -1,
                    '8' => -1,
                    '7' => -1,
                    '6' => -1,
                    '5' => -1,
                    '4' => -1,
                    '3' => 0,
                    _   => 1
                },
                '2' => y switch 
                {
                    '2' => 0,
                    'J' => 1,
                    _   => -1
                },
                _ => y switch 
                {
                    'J' => 0,
                    _   => -1
                },
            };
        }
        // A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, or 2
        return x switch 
        {
            'A' => y switch 
            {
                'A' => 0,
                _   => 1
            },
            'K' => y switch 
            {
                'A' => -1,
                'K' => 0,
                _   => 1
            },
            'Q' => y switch 
            {
                'A' => -1,
                'K' => -1,
                'Q' => 0,
                _   => 1
            },
            'J' => y switch 
            {
                'A' => -1,
                'K' => -1,
                'Q' => -1,
                'J' => 0,
                _   => 1
            },
            'T' => y switch 
            {
                'A' => -1,
                'K' => -1,
                'Q' => -1,
                'J' => -1,
                'T' => 0,
                _   => 1
            },
            '9' => y switch 
            {
                'A' => -1,
                'K' => -1,
                'Q' => -1,
                'J' => -1,
                'T' => -1,
                '9' => 0,
                _   => 1
            },
            '8' => y switch 
            {
                'A' => -1,
                'K' => -1,
                'Q' => -1,
                'J' => -1,
                'T' => -1,
                '9' => -1,
                '8' => 0,
                _   => 1
            },
            '7' => y switch 
            {
                'A' => -1,
                'K' => -1,
                'Q' => -1,
                'J' => -1,
                'T' => -1,
                '9' => -1,
                '8' => -1,
                '7' => 0,
                _   => 1
            },
            '6' => y switch 
            {
                'A' => -1,
                'K' => -1,
                'Q' => -1,
                'J' => -1,
                'T' => -1,
                '9' => -1,
                '8' => -1,
                '7' => -1,
                '6' => 0,
                _   => 1
            },
            '5' => y switch 
            {
                'A' => -1,
                'K' => -1,
                'Q' => -1,
                'J' => -1,
                'T' => -1,
                '9' => -1,
                '8' => -1,
                '7' => -1,
                '6' => -1,
                '5' => 0,
                _   => 1
            },
            '4' => y switch 
            {
                'A' => -1,
                'K' => -1,
                'Q' => -1,
                'J' => -1,
                'T' => -1,
                '9' => -1,
                '8' => -1,
                '7' => -1,
                '6' => -1,
                '5' => -1,
                '4' => 0,
                _   => 1
            },
            '3' => y switch 
            {
                '3' => 0,
                '2' => 1,
                _   => -1
            },
            _ => y switch 
            {
                '2' => 0,
                _   => -1
            },
        };
    }

    private readonly HandType FourOfAKindOrFullHouse(List<char> distinctCards)
    {
        if (IsJokerGame)
        {
            if (distinctCards.Any(c => c == 'J'))
                return HandType.FiveOfAKind;
            if (Cards.Where(c => c == distinctCards[0]).Count() > 1 && Cards.Where(c => c == distinctCards[1]).Count() > 1)
                return HandType.FullHouse;
            return HandType.FourOfAKind;
        }
        
        if (Cards.Where(c => c == distinctCards[0]).Count() > 1 && Cards.Where(c => c == distinctCards[1]).Count() > 1)
            return HandType.FullHouse;
        return HandType.FourOfAKind;
    }

    private readonly HandType ThreeOfAKindOrTwoPair(List<char> distinctCards)
    {
        var cards = this.Cards;
        if (IsJokerGame)
        {
            var hasJokers = distinctCards.Any(c => c == 'J');
            if (distinctCards.Where(dc => cards.Where(c => c == dc).Count() == 3).Count() == 1)
                return hasJokers ? HandType.FourOfAKind : HandType.ThreeOfAKind;
            if (cards.Where(c => c == 'J').Count() == 2)
                return HandType.FourOfAKind;
            return hasJokers ? HandType.FullHouse : HandType.TwoPair;            
        }

        if (distinctCards.Where(dc => cards.Where(c => c == dc).Count() == 3).Count() == 1)
            return HandType.ThreeOfAKind;
        return HandType.TwoPair;
    }

    public readonly int Compare(Hand x, Hand y)
    {
        return x.CompareTo(y);
    }

    public static bool operator <(Hand left, Hand right) =>
        left.CompareTo(right) < 0;

    public static bool operator >(Hand left, Hand right) =>
        left.CompareTo(right) > 0;

    public static bool operator <=(Hand left, Hand right) => 
        left.CompareTo(right) <= 0;

    public static bool operator >=(Hand left, Hand right) => 
        left.CompareTo(right) >= 0;
}

public enum HandType
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind,
}