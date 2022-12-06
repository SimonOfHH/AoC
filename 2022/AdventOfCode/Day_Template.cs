namespace AdventOfCode;

public class Day_XX : BaseDay
{
    private readonly bool Sample = false;
    public Day_XX()
    {
        var input = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");

    private string[] ParseInput()
    {
        return this.GetInput(Sample);
    }
}