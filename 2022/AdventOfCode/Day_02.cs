namespace AdventOfCode;

public class Day_02 : BaseDay
{
    public bool Sample = true;
    private List<RpsGame> rpsGames;
    public Day_02()
    {
    }

    public override ValueTask<string> Solve_1()
    {
        rpsGames = ParseInput(1);
        return new(rpsGames.Sum(x => x.Result).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        rpsGames = ParseInput(2);
        return new(rpsGames.Sum(x => x.Result).ToString());
    }

    private List<RpsGame> ParseInput(int part)
    {
        var table = new List<RpsGame>();
        foreach (var line in this.GetInput(Sample))
        {
            if (!String.IsNullOrEmpty(line))
            {
                var values = line.Split(" ");
                table.Add(new RpsGame(values, part));
            }
        }
        return table;
    }
}

public class RpsGame
{
    public RpsValue PlayerSelection { get; set; }
    public RpsValue OpponentSelection { get; set; }
    public Outcome DesiredOutcome { get; set; }
    public int Result
    {
        get
        {
            return GetScore();
        }
    }
    public RpsGame(string[] s, int part)
    {
        if (part == 1)
        {
            OpponentSelection = GetRpsValue(s[0]);
            PlayerSelection = GetRpsValue(s[1]);
        }
        else
        {
            OpponentSelection = GetRpsValue(s[0]);
            DesiredOutcome = GetDesiredOutcome(s[1]);
            PlayerSelection = FindPlayerSelectionBaseOnDesiredOutcome();
        }
    }
    private RpsValue GetRpsValue(string s)
    {
        switch (s)
        {
            case "A":
            case "X": return RpsValue.Rock;
            case "B":
            case "Y": return RpsValue.Paper;
            case "C":
            case "Z": return RpsValue.Scissors;
            default: throw new Exception("String should not be empty");
        }
    }
    private Outcome GetDesiredOutcome(string s)
    {
        switch (s)
        {
            case "X": return Outcome.Loss;
            case "Y": return Outcome.Draw;
            case "Z": return Outcome.Win;
            default: throw new Exception("String should not be empty");
        }
    }
    private RpsValue FindPlayerSelectionBaseOnDesiredOutcome()
    {
        if (OpponentSelection == RpsValue.Null)
            throw new Exception("OpponentSelection must be set");
        switch (DesiredOutcome)
        {
            case Outcome.Loss:
                switch (OpponentSelection)
                {
                    case RpsValue.Rock: return RpsValue.Scissors;
                    case RpsValue.Paper: return RpsValue.Rock;
                    case RpsValue.Scissors: return RpsValue.Paper;
                }
                break;
            case Outcome.Draw:
                return OpponentSelection;
            case Outcome.Win:
                switch (OpponentSelection)
                {
                    case RpsValue.Rock: return RpsValue.Paper;
                    case RpsValue.Paper: return RpsValue.Scissors;
                    case RpsValue.Scissors: return RpsValue.Rock;
                }
                break;
        }
        return RpsValue.Null;
    }
    private int GetScore()
    {
        int score = (int)PlayerSelection;
        score += (int)GetOutcome();
        return score;
    }
    private Outcome GetOutcome()
    {
        if (Loss()) return Outcome.Loss;
        if (Draw()) return Outcome.Draw;
        return Outcome.Win;
    }
    private bool Draw() => (this.OpponentSelection == this.PlayerSelection);
    private bool Win()
    {
        if ((PlayerSelection == RpsValue.Rock) && (OpponentSelection == RpsValue.Scissors))
            return true;
        if ((PlayerSelection == RpsValue.Paper) && (OpponentSelection == RpsValue.Rock))
            return true;
        if ((PlayerSelection == RpsValue.Scissors) && (OpponentSelection == RpsValue.Paper))
            return true;
        return false;
    }
    private bool Loss()
    {
        if (Draw()) return false;
        if (Win()) return false;
        return true;
    }
}
public enum Outcome
{
    Loss = 0,
    Draw = 3,
    Win = 6
}
public enum RpsValue
{
    Null = 0,
    Rock = 1,
    Paper = 2,
    Scissors = 3
}