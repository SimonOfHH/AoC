using SheepTools.Model;

namespace AoC_2021;

public class Day_02 : BaseDay
{
    private readonly List<(Direction Direction, int Value)> _input;

    public Day_02()
    {
        _input = ParseInput().ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        var position = new Point(0, 0);

        foreach (var direction in _input)
        {
            position = position.Move(direction.Direction, direction.Value);
        }

        return new(Math.Abs(position.X * position.Y).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var aim = 0;
        var position = new Point(0, 0);

        foreach (var direction in _input)
        {
            switch (direction.Direction)
            {
                case Direction.Down:
                    aim += direction.Value;
                    break;
                case Direction.Up:
                    aim -= direction.Value;
                    break;
                case Direction.Right:
                    position = position.Move(direction.Direction, direction.Value);
                    position.Y += (direction.Value * aim);
                    break;
                default:
                    throw new();
            }
        }

        return new((position.X * position.Y).ToString());
    }

    private IEnumerable<(Direction Direction, int Value)> ParseInput()
    {
        static Direction ParseDirection(string str)
        {
            return str switch
            {
                "forward" => Direction.Right,
                "up" => Direction.Up,
                "down" => Direction.Down,
                _ => throw new()
            };
        }

        var file = new ParsedFile(InputFilePath);

        while (!file.Empty)
        {
            var line = file.NextLine();
            yield return (ParseDirection(line.NextElement<string>()), line.NextElement<int>());
        }
    }
}