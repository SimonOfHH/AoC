namespace AdventOfCode;

public class Day_01 : BaseDay
{
    private readonly string _input;
    private Dictionary<int, int> elfCalories;

    public Day_01()
    {
        _input = File.ReadAllText(InputFilePath);
        elfCalories = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        return new(elfCalories.MaxBy(kvp => kvp.Value).Value.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new ValueTask<string>(elfCalories.OrderByDescending(kvp => kvp.Value).Take(3).Sum(kvp => kvp.Value).ToString());
    }

    private Dictionary<int, int> ParseInput()
    {
        var values = new Dictionary<int, int>();
        foreach (var line in File.ReadAllLines(InputFilePath))
        {
            if ((String.IsNullOrEmpty(line)) || (values.Count == 0))
                values.Add(values.Count + 1, 0);
            int calories = 0;
            if (int.TryParse(line, out calories))
                values[values.Count] += calories;
        }
        return values;
    }
}
