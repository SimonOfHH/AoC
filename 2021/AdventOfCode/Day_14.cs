using System.Text;
using SheepTools;
using SheepTools.Model;

namespace AoC_2021;

public class Day_14 : BaseDay
{
    private readonly (string Template, List<(string pattern, string Value)> Instructions) _input;

    public Day_14()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        var result = _input.Template;
        for (int step = 0; step < 10; ++step)
        {
            var sb = new StringBuilder();

            var prevChar = '\n';
            foreach (var ch in result)
            {
                var patt = $"{prevChar}{ch}";

                var matching = _input.Instructions.FirstOrDefault(i => i.pattern == patt);
                if (matching != default)
                {
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(matching.Value);
                }
                else
                {
                    sb.Append(ch);
                }
                prevChar = ch;
            }

            result = sb.ToString();
        }

        var groups = result.GroupBy(i => i).OrderByDescending(g => g.Count());
        var diff = groups.First().Count() - groups.Last().Count();

        return new($"{diff}");
    }

    public override ValueTask<string> Solve_2()
    {
        return new("");
        var result = _input.Template;
        for (int step = 0; step < 40; ++step)
        {
            Console.WriteLine($"Step {step + 1}");
            var sb = new StringBuilder();

            var prevChar = '\n';
            foreach (var ch in result)
            {
                var patt = $"{prevChar}{ch}";

                var matching = _input.Instructions.FirstOrDefault(i => i.pattern == patt);
                if (matching != default)
                {
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(matching.Value);
                }
                else
                {
                    sb.Append(ch);
                }
                prevChar = ch;
            }

            result = sb.ToString();
        }

        var groups = result.GroupBy(i => i).OrderByDescending(g => g.Count());
        var diff = groups.First().Count() - groups.Last().Count();

        return new($"{diff}");
    }

    private (string Template, List<(string pattern, string Value)> Instructions) ParseInput()
    {
        var groupsOfLines = ParsedFile.ReadAllGroupsOfLines(InputFilePath);
        if (groupsOfLines.Count != 2) throw new SolvingException("2 groups of lines expected in input file");

        return (
            groupsOfLines[0][0],
            groupsOfLines[1].Select(g => { var parts = g.Split(" -> "); return (parts[0], $"{parts[0][0]}{parts[1]}{parts[0][1]}"); }).ToList()
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
