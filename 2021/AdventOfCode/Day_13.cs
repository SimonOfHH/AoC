using System.Text;
using SheepTools;
using SheepTools.Model;

namespace AoC_2021;

public class Day_13 : BaseDay
{
    private readonly (HashSet<IntPoint> Points, HashSet<(bool IsX, int Value)> FoldInstructions) _input;

    public Day_13()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        var resultsGrid = Fold(_input.Points, _input.FoldInstructions.First());

        return new($"{resultsGrid.Count}");
    }

    public override ValueTask<string> Solve_2()
    {
        var resultsGrid = _input.Points;

        foreach (var instruction in _input.FoldInstructions)
        {
            resultsGrid = Fold(resultsGrid, instruction);
        }

        return new(Print(resultsGrid));
    }

    private (HashSet<IntPoint> Points, HashSet<(bool isX, int Value)> FoldInstructions) ParseInput()
    {
        var groupsOfLines = ParsedFile.ReadAllGroupsOfLines(InputFilePath);
        if (groupsOfLines.Count != 2) throw new SolvingException("2 groups of lines expected in input file");

        return (
            groupsOfLines[0].Select(g => { var parts = g.Split(','); return new IntPoint(int.Parse(parts[0]), int.Parse(parts[1])); }).ToHashSet(),
            groupsOfLines[1].Select(g => { var parts = g.Split('='); return (parts[0].Last() == 'x', int.Parse(parts[1])); }).ToHashSet()
        );
    }

    private static HashSet<IntPoint> Fold(HashSet<IntPoint> points, (bool IsX, int Value) foldInstruction)
    {
        IEnumerable<IntPoint> pointsToRemove;
        IEnumerable<IntPoint> pointsToAdd;

        if (foldInstruction.IsX)
        {
            pointsToRemove = points.Where(p => p.X > foldInstruction.Value).ToList();
            pointsToAdd = pointsToRemove.Select(p => p with { X = foldInstruction.Value + foldInstruction.Value - p.X }).ToList();
        }
        else
        {
            pointsToRemove = points.Where(p => p.Y > foldInstruction.Value).ToList();
            pointsToAdd = pointsToRemove.Select(p => p with { Y = foldInstruction.Value + foldInstruction.Value - p.Y }).ToList();
        }

        foreach (var p in pointsToRemove) points.Remove(p);
        foreach (var p in pointsToAdd) points.Add(p);

        return points;
    }

    private static string Print(HashSet<IntPoint> points)
    {
        var sb = new StringBuilder(Environment.NewLine);

        foreach (var y in RangeHelpers.GenerateRange(points.Min(p => p.Y), points.Max(p => p.Y)))
        {
            foreach (var x in RangeHelpers.GenerateRange(points.Min(p => p.X), points.Max(p => p.X)))
                sb.Append(points.Any(p => p.X == x && p.Y == y) ? 'X' : '.');
            
            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }
}
