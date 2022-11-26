namespace AoC_2021;

public class Day_01 : BaseDay
{
    private readonly int[] _input;

    public Day_01()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        var result = 0;

        for (int i = 1; i < _input.Length; ++i)
        {
            if (_input[i] > _input[i - 1])
            {
                ++result;
            }
        }

        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        const int n = 3;
        var result = 0;

        for (int i = n; i < _input.Length; ++i)
        {
            if (_input[i] > _input[i - n])
            {
                ++result;
            }
        }

        return new(result.ToString());
    }

    private int[] ParseInput()
    {
        return File.ReadAllLines(InputFilePath).Select(int.Parse).ToArray();
    }
}