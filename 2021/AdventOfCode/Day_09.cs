using SheepTools.Model;

namespace AoC_2021;

public class Day_09 : BaseDay
{
    private readonly List<List<int>> _input;

    public Day_09()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        var lowPointList = GetLowPoints(_input);

        return new($"{lowPointList.Sum(point => _input[point.Y][point.X]) + lowPointList.Count}");
    }

    public override ValueTask<string> Solve_2()
    {
        var lowPointList = GetLowPoints(_input);
        var basinList = new List<List<int>>();

        foreach (var point in lowPointList)
        {
            var basinPointSet = new HashSet<IntPoint>(_input.Count) { point };

            var minValue = _input[point.Y][point.X];
            basinList.Add(new() { minValue });

            GetBasin(_input, point, basinPointSet, basinList);
        }

        var result = basinList
            .OrderByDescending(item => item.Count())
            .Take(3)
            .Aggregate(1, (total, basin) => total * basin.Count);

        return new($"{result}");
    }

    private List<List<int>> ParseInput()
    {
        return File.ReadAllLines(InputFilePath).Select(str => str.Select(ch => int.Parse(ch.ToString())).ToList()).ToList();
    }

    private static List<IntPoint> GetLowPoints(List<List<int>> grid)
    {
        List<IntPoint> lowPoints = new();

        for (int y = 0; y < grid.Count; ++y)
        {
            for (int x = 0; x < grid[y].Count; ++x)
            {
                var point = new IntPoint(x, y);
                var value = grid[y][x];
                var neighbours = GetNeighbours(grid, point);
                if (neighbours.All(n => grid[n.Y][n.X] > value))
                {
                    lowPoints.Add(point);
                }
            }
        }

        return lowPoints;
    }

    private static List<IntPoint> GetNeighbours(List<List<int>> grid, IntPoint point)
    {
        return new List<IntPoint>{
            point.Move(Direction.Down),
            point.Move(Direction.Up),
            point.Move(Direction.Left),
            point.Move(Direction.Right),
        }
        .Where(p => p.Y >= 0 && p.Y < grid.Count && p.X >= 0 && p.X < grid[0].Count)
        .ToList();
    }

    private static void GetBasin(List<List<int>> grid, IntPoint basinElement, HashSet<IntPoint> existingBasinPoints, List<List<int>> basin)
    {
        var neighbourList = GetNeighbours(grid, basinElement);
        foreach (var neighbour in neighbourList)
        {
            var value = grid[neighbour.Y][neighbour.X];
            if (value != 9 && existingBasinPoints.Add(neighbour))
            {
                basin.Last().Add(value);
                GetBasin(grid, neighbour, existingBasinPoints, basin);
            }
        }
    }
}