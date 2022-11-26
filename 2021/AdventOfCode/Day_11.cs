using SheepTools.Model;

namespace AoC_2021;

sealed record class IntPointWithValue : IntPoint
{
    public int Value { get; set; }

    public IntPointWithValue(int x, int y, int value) : base(x, y)
    {
        Value = value;
    }

    private IntPointWithValue(IntPoint point, int value) : base(point.X, point.Y)
    {
        Value = value;
    }

    public new IntPointWithValue Move(Direction direction, int distnce = 1)
    {
        return new IntPointWithValue(base.Move(direction), Value);
    }

    #region Equals override, to avoid taking into account Value for equality

    public override int GetHashCode() => base.GetHashCode();

    public bool Equals(IntPointWithValue? other) => base.Equals(other);

    #endregion
}

/// Reading the input in each, since we modify it
public class Day_11 : BaseDay
{
    public override ValueTask<string> Solve_1()
    {
        var _input = ParseInput().ToList();

        const int steps = 100;
        Dictionary<IntPoint, List<IntPointWithValue>> neighbours = new(_input
            .Select(point => new KeyValuePair<IntPoint, List<IntPointWithValue>>(
                point,
                GetNeighbours(_input, point))));

        int flashesCount = 0;
        for (int step = 0; step < steps; ++step)
        {
            // Print(_input, step);
            foreach (var p in _input) ++p.Value;

            while (_input.Any(p => p.Value > 9))
            {
                foreach (var point in _input)
                {
                    if (point.Value > 9)
                    {
                        ++flashesCount;
                        point.Value = 0;

                        foreach (var p in neighbours[point]) if (p.Value != 0) ++p.Value;
                    }
                }
            }
        }

        return new($"{flashesCount}");
    }

    public override ValueTask<string> Solve_2()
    {
        var _input = ParseInput().ToList();

        Dictionary<IntPoint, List<IntPointWithValue>> neighbours = new(_input
            .Select(point => new KeyValuePair<IntPoint, List<IntPointWithValue>>(
                point,
                GetNeighbours(_input, point))));

        int steps = 0;
        while (!_input.All(p => p.Value == 0))
        {
            // Print(_input, steps);
            foreach (var p in _input) ++p.Value;

            while (_input.Any(p => p.Value > 9))
            {
                foreach (var point in _input)
                {
                    if (point.Value > 9)
                    {
                        point.Value = 0;

                        foreach (var p in neighbours[point]) if (p.Value != 0) ++p.Value;
                    }
                }
            }
            ++steps;
        }

        return new($"{steps}");
    }

    private IEnumerable<IntPointWithValue> ParseInput()
    {
        var file = new ParsedFile(InputFilePath);

        int y = 0;
        while (!file.Empty)
        {
            int x = 0;
            var line = file.NextLine().ToSingleString();
            foreach (var ch in line)
            {
                yield return new IntPointWithValue(x, y, int.Parse($"{ch}"));
                ++x;
            }
            ++y;
        }
    }

    private static List<IntPointWithValue> GetNeighbours(List<IntPointWithValue> squareGrid, IntPointWithValue point)
    {
        return new List<IntPointWithValue?>{
            squareGrid.FirstOrDefault(p => p.Equals(point.Move(Direction.Down))),
            squareGrid.FirstOrDefault(p => p.Equals(point.Move(Direction.Up))),
            squareGrid.FirstOrDefault(p => p.Equals(point.Move(Direction.Left))),
            squareGrid.FirstOrDefault(p => p.Equals(point.Move(Direction.Right))),
            squareGrid.FirstOrDefault(p => p.Equals(point.Move(Direction.Down).Move(Direction.Left))),
            squareGrid.FirstOrDefault(p => p.Equals(point.Move(Direction.Down).Move(Direction.Right))),
            squareGrid.FirstOrDefault(p => p.Equals(point.Move(Direction.Up).Move(Direction.Left))),
            squareGrid.FirstOrDefault(p => p.Equals(point.Move(Direction.Up).Move(Direction.Right)))
        }
        .Where(p => p is not default(IntPointWithValue))
        .Select(p => p!)
        .ToList();
    }

    private static void Print(List<IntPointWithValue> points, int step)
    {
        if (step < 10 || step % 10 == 0)
        {
            Console.WriteLine($"Step {step}");

            IntPointWithValue? prevP = default;
            foreach (var p in points.OrderBy(p => p.Y).ThenBy(p => p.X))
            {
                if (p.Y != prevP?.Y)
                {
                    Console.WriteLine();
                }
                Console.Write(p.Value);
                prevP = p;
            }
            Console.WriteLine();
        }
    }
}