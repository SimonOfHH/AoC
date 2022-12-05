using System.Diagnostics;
using MoreLinq;
namespace AoC_2021;
public class Day_19 : BaseDay
{
    public bool Sample = true;

    public Day_19()
    {
        var input = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");

    private List<Scanner> ParseInput()
    {
        return this.GetInput(Sample).Segment(x => String.IsNullOrEmpty(x)) // Split up into groups
                                    .Select(y => y.Where(z => !String.IsNullOrEmpty(z)) // Get rid of empty elements
                                    .ToList()).Where(l => l.Count() > 0) // Exclude completely empty lists
                                    .ToList()
                                    .Select(s => new Scanner(s)).ToList();
    }
}
public class Scanner
{
    public string Name { get; set; }
    public List<Point> Coordinates { get; set; }
    public Scanner(List<string> inputs)
    {
        Name = inputs.First();
        Coordinates = inputs.Skip(1).Select(v => new Point(v)).ToList();
    }
}
public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public Point(string rawValue)
    {
        var values = rawValue.Split(",").Select(v => int.Parse(v)).ToArray();
        X = values[0];
        Y = values[1];
        Z = values[2];
    }
    public override string ToString()
    {
        return String.Format("X={0}, Y={1}, Z={2}", this.X, this.Y, this.Z);
    }
}