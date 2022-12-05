using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day_05 : BaseDay
{
    public bool Sample = false;
    public int NumberOfColumns
    {
        get
        {
            if (Sample) return 3;
            return 9;
        }
    }
    public List<Stack> Stacks;
    public List<Instruction> Instructions;

    public Day_05()
    {
    }

    public override ValueTask<string> Solve_1()
    {
        (Stacks, Instructions) = ParseInput();
        ProcessInstructions(1);
        return new(GetStackTopValues());
    }

    public override ValueTask<string> Solve_2()
    {
        (Stacks, Instructions) = ParseInput();
        ProcessInstructions(2);
        return new(GetStackTopValues());
    }

    public (List<Stack>, List<Instruction>) ParseInput()
    {
        var stacks = PrepareStackList();
        var rawStacks = new List<string[]>();
        var instructions = new List<Instruction>();

        var input = this.GetInput(Sample);
        rawStacks = input.TakeWhile(s => !String.IsNullOrEmpty(s)).Select(l => ParseStackLine(l)).ToList();
        rawStacks.Reverse();
        instructions = input.SkipWhile(s => !String.IsNullOrEmpty(s)).Where(s => !String.IsNullOrEmpty(s)).Select(l => new Instruction(l)).ToList();

        foreach (var rawStack in rawStacks.Skip(1))
        {
            for (int i = 0; i < NumberOfColumns; i++)
            {
                if (!String.IsNullOrEmpty(rawStack[i].Trim()))
                    stacks[i].Push(rawStack[i]);
            }
        }
        return (stacks, instructions);
    }
    private List<Stack> PrepareStackList()
    {
        var stacks = new List<Stack>();
        Enumerable.Range(0, NumberOfColumns).ToList().ForEach(n => stacks.Add(new Stack()));
        return stacks;
    }
    public string[] ParseStackLine(string s)
    {
        int fieldLength = 3;
        var entries = new List<string>();
        while ((!String.IsNullOrEmpty(s)) && (entries.Count() < NumberOfColumns))
        {
            string entry = s.Length >= fieldLength ? s.Substring(0, fieldLength) : s;
            s = s.Length >= fieldLength + 1 ? s.Substring(fieldLength + 1) : String.Empty;
            entries.Add(entry.Replace("[", "").Replace("]", ""));
        }
        return entries.ToArray();
    }
    public void ProcessInstructions(int part)
    {
        int count = 0;
        foreach (var instruction in Instructions)
        {
            count++;
            if (part == 1)
                for (int i = 0; i < instruction.Count; i++)
                {
                    Stacks[instruction.ToStack].Push(Stacks[instruction.FromStack].Pop());
                }
            else
            {
                var range = new List<string>();
                for (int i = 0; i < instruction.Count; i++)
                {
                    range.Add((string)Stacks[instruction.FromStack].Pop());
                }
                range.Reverse();
                range.ForEach(x => Stacks[instruction.ToStack].Push(x));
            }
        }
    }
    public string GetStackTopValues()
    {
        var values = Stacks.Select(s => s.Peek()).ToList();
        return String.Join("", values.ToArray());
    }
}
public class Instruction
{
    public int Count { get; set; }
    public int FromStack { get; set; }
    public int ToStack { get; set; }
    public Instruction(string s)
    {
        var matches = Regex.Matches(s, @" (\d{1,2})");
        Count = int.Parse(matches[0].Value);
        FromStack = int.Parse(matches[1].Value) - 1;
        ToStack = int.Parse(matches[2].Value) - 1;
    }
}