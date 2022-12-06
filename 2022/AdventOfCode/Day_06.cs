namespace AdventOfCode;

public class Day_06 : BaseDay
{
    public bool Sample = false;
    public Day_06()
    {
    }

    public override ValueTask<string> Solve_1() => new(ParseInput(4).ToString());

    public override ValueTask<string> Solve_2() => new(ParseInput(14).ToString());

    public int ParseInput(int length)
    {
        string input = this.GetInput(Sample).First();
        for (int i = 0; i < input.Length; i++)
        {
            if (input.Skip(i).Take(length).Distinct().Count() == length)
                return i + length;
        }
        return 0;
    }
}