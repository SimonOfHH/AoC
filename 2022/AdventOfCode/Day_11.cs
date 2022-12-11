using MoreLinq;
namespace AdventOfCode;

public class Day_11 : BaseDay
{
    private Game Game;
    private readonly bool Sample = false;
    public Day_11()
    {
    }
    public override ValueTask<string> Solve_1()
    {
        Game = ParseInput();
        Game.Play(20, 3);
        return new(Game.MonkeyBusiness.ToString());
    }
    public override ValueTask<string> Solve_2()
    {
        Game = ParseInput();
        Game.Play(10000, GetLeastCommonMultiple());
        return new(Game.MonkeyBusiness.ToString());
    }
    private long GetLeastCommonMultiple()
    {
        return Game.Monkeys.Values.Select(x => x.TestValidation.Value).Aggregate((val1, val2) => val1 * val2);
    }
    private Game ParseInput()
    {
        return new Game(this.GetInput(Sample));
    }
}
class Game
{
    public Dictionary<int, Monkey> Monkeys { get; set; }
    public List<long> InspectedItems => Monkeys.Select(m => m.Value.InspectedItems).ToList();
    public long MonkeyBusiness => InspectedItems.OrderByDescending(x => x).Take(2).Aggregate((a,b) => a*b);
    public Game(string[] input)
    {
        Monkeys = new Dictionary<int, Monkey>();
        while (input.Any())
        {
            var monkeyInput = input.TakeUntil(s => String.IsNullOrEmpty(s)).ToArray();
            Monkeys.Add(Monkeys.Count, new Monkey(monkeyInput));
            input = input.Skip(monkeyInput.Count()).ToArray();
        }
    }
    public void Play(int rounds, long divisor)
    {
        for (int i = 0; i < rounds; i++)
        {
            PlayRound(divisor);
        }
    }
    public void PlayRound(long divisor)
    {
        foreach (var monkey in Monkeys.Values)
        {
            while (monkey.Items.Count > 0)
            {
                var item = monkey.InspectAndReturnItem(divisor);
                if (monkey.TestValidation.Test(item))
                    Monkeys[monkey.TestValidation.NextMonkeyTrue].Items.Add(item);
                else
                    Monkeys[monkey.TestValidation.NextMonkeyFalse].Items.Add(item);
            }
        }
    }
}
class Item
{
    public long OriginalValue { get; set; }
    public long WorryLevel { get; set; }
    private decimal WorryLevelTemp { get; set; }
    public Item(string s)
    {
        WorryLevel = (long)int.Parse(s.Trim());
        OriginalValue = WorryLevel;
    }
    public void ApplyOperation(Operation operation, long divisor)
    {
        if (operation.MultiplyWithOld)
            WorryLevel = WorryLevel * WorryLevel;
        else
        {
            switch (operation.ArithmeticOperation)
            {
                case ArithmeticOperation.Plus: WorryLevel = WorryLevel + operation.Value; break;
                case ArithmeticOperation.Minus: WorryLevel = WorryLevel - operation.Value; break;
                case ArithmeticOperation.Multiply: WorryLevel = WorryLevel * operation.Value; break;
                case ArithmeticOperation.Divide: WorryLevel = WorryLevel / operation.Value; break;
            }
        }
        if (divisor == 3)
            WorryLevel = WorryLevel / divisor;
        else
            WorryLevel = WorryLevel % divisor;
    }
    public override string ToString()
    {
        return String.Format("{0} (Initial: {1})", WorryLevel, OriginalValue);
    }
}
class Monkey
{
    private int ID { get; set; }
    public List<Item> Items { get; set; }
    public Operation Operation { get; set; }
    public TestValidation TestValidation { get; set; }
    public long InspectedItems { get; set; }
    public Monkey(string[] input)
    {
        Items = new List<Item>();
        ID = int.Parse(input[0].Replace("Monkey", "").Trim().Replace(":", "").Trim());
        input[1].Replace("Starting items: ", "").Trim().Split(",").Select(s => s.Trim()).ForEach(i => Items.Add(new Item(i)));
        Operation = new Operation(input[2]);
        TestValidation = new TestValidation(input.Skip(3).Take(3).ToArray());
    }
    public Item InspectAndReturnItem(long divisor)
    {
        InspectedItems++;
        Items.First().ApplyOperation(Operation, divisor);
        var item = Items.First();
        Items.RemoveAt(0);
        return item;

    }
    public override string ToString()
    {
        return String.Format("Monkey {0} (Items: {1})", ID, string.Join(",", Items.Select(i => i.WorryLevel)));
    }
}

class Operation
{
    public int Value { get; set; }
    public ArithmeticOperation ArithmeticOperation { get; set; }
    public bool MultiplyWithOld { get; set; }
    public Operation(string s)
    {
        s = s.Replace("Operation: new = ", "").Trim();
        if (s == "old * old")
        {
            MultiplyWithOld = true;
            ArithmeticOperation = ArithmeticOperation.Multiply;
        }
        else
        {
            s = s.Replace("old", "").Trim();
            switch (s[0])
            {
                case '+': ArithmeticOperation = ArithmeticOperation.Plus; break;
                case '-': ArithmeticOperation = ArithmeticOperation.Minus; break;
                case '*': ArithmeticOperation = ArithmeticOperation.Multiply; break;
                case '/': ArithmeticOperation = ArithmeticOperation.Divide; break;
            }
            s = s.Remove(0, 1).Trim();
            Value = int.Parse(s);            //Multiplier = int.Parse()
        }
    }
    public override string ToString()
    {
        return String.Format("{0} {1} {2}", "old", ArithmeticOperation, MultiplyWithOld ? "old" : Value);
    }
}
class TestValidation
{
    public long Value { get; set; }
    public int NextMonkeyTrue { get; set; }
    public int NextMonkeyFalse { get; set; }
    public TestValidation(string[] s)
    {
        s[0] = s[0].Replace("Test: divisible by ", "").Trim();
        Value = int.Parse(s[0]);
        NextMonkeyTrue = int.Parse(s[1].Replace("If true: throw to monkey ", "").Trim());
        NextMonkeyFalse = int.Parse(s[2].Replace("If false: throw to monkey ", "").Trim());
    }
    public bool Test(Item item)
    {
        if (item.WorryLevel % Value == 0)
            return true;
        return false;
    }
    public override string ToString()
    {
        return String.Format("Divisible by {0}; next true: {1}, next false {2}", Value, NextMonkeyTrue, NextMonkeyFalse);
    }
}
enum ArithmeticOperation
{
    Plus,
    Minus,
    Multiply,
    Divide
}