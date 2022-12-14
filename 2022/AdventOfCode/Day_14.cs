using System.Diagnostics;

namespace AdventOfCode;

public class Day_14 : BaseDay
{
    private readonly bool Sample = true;
    public Day_14()
    {
        var input = ParseInput();
        var map = new RockMap();
        map.AddInputData(input);
        map.DrawMap();
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    private List<RockMapPath> ParseInput()
    {
        var paths = new List<RockMapPath>();
        foreach (var line in this.GetInput(Sample))
        {
            RockMapPoint prev = null;
            var sPoints = line.Split(" -> ").ToList().Select(s => new RockMapPoint(s));
            var lines = new List<RockMapLine>();
            foreach (var sPoint in sPoints)
            {
                if (prev != null)
                    lines.Add(new RockMapLine(prev, sPoint));
                prev = sPoint;
            }
            paths.Add(new RockMapPath(lines));
        }
        return paths;
    }
}

class RockMap
{
    public int FromWidth { get; set; }
    public int ToWidth { get; set; }
    public int Height { get; set; }
    public List<RockMapPoint> Sand { get; set; }
    public List<RockMapPoint> Points { get; set; }
    public RockMap()
    {
        Initialize(494, 503, 10);
    }
    private void Initialize(int fromWidth, int toWidth, int height)
    {
        Points = new List<RockMapPoint>();
        FromWidth = fromWidth;
        ToWidth = toWidth;
        Height = height;
        InitializePoints();
    }
    private void InitializePoints()
    {
        for (int x = FromWidth; x <= ToWidth; x++)
            for (int y = 0; y < Height; y++)
                Points.Add(new RockMapPoint(x, y));
    }
    public void AddInputData(List<RockMapPath> paths)
    {
        paths.ForEach(path => path.Points.ForEach(point => UpdatePoint(point, PointType.Rock)));
        /*
        foreach (var path in paths)
        {
            foreach (var point in path.Points)
            {
                UpdatePoint(point, PointType.Rock);
            }
        }
        */
    }
    private void UpdatePoint(RockMapPoint p, PointType type)
    {
        var sourcePoint = Points.First(ep => ep.X == p.X && ep.Y == p.Y);
        sourcePoint.Type = type;
    }
    public void DrawMap()
    {
        var values = new string[Points.Max(p => p.Y) + 1];
        foreach (var p in Points)
            values[p.Y] += p.ToString();
        string map = string.Join(Environment.NewLine, values);
        Console.WriteLine(map);
    }
}
class RockMapPath
{
    public List<RockMapLine> Lines { get; set; }
    public List<RockMapPoint> Points => Lines.SelectMany(l => l.AllPoints).Distinct().ToList();
    public RockMapPath(List<RockMapLine> lines) { Lines = lines; }
    public override string ToString()
    {
        string s = String.Empty;
        foreach (var line in Lines)
        {
            if (String.IsNullOrEmpty(s))
                s += $"[{line.FromPoint.X},{line.FromPoint.Y}]";
            if (!String.IsNullOrEmpty(s))
                s += " -> ";
            s += $"[{line.ToPoint.X},{line.ToPoint.Y}]";
        }
        if (!String.IsNullOrEmpty(s))
            return s;
        return base.ToString();
    }
}
class RockMapLine
{
    public RockMapPoint FromPoint { get; set; }
    public RockMapPoint ToPoint { get; set; }
    public List<RockMapPoint> Points => new() { FromPoint, ToPoint };
    public List<RockMapPoint> AllPoints => GetPointsInBetween();
    public RockMapLine(RockMapPoint from, RockMapPoint to) { FromPoint = from; ToPoint = to; }
    private List<RockMapPoint> GetPointsInBetween()
    {
        var points = new List<RockMapPoint>();
        if (FromPoint.Y != ToPoint.Y)
        {
            points.Add(FromPoint);
            var tempPoint = FromPoint.Copy();
            while (!tempPoint.Equals(ToPoint))
            {
                if (tempPoint.Y < ToPoint.Y)
                    points.Add(new RockMapPoint(tempPoint.X, tempPoint.Y + 1, tempPoint.Type));
                else if (tempPoint.Y > ToPoint.Y)
                    points.Add(new RockMapPoint(tempPoint.X, tempPoint.Y - 1, tempPoint.Type));
                tempPoint = points.Last();
            }
        }
        else if (FromPoint.X != ToPoint.X)
        {
            points.Add(FromPoint);
            var tempPoint = FromPoint.Copy();
            while (!tempPoint.Equals(ToPoint))
            {
                if (tempPoint.X < ToPoint.X)
                    points.Add(new RockMapPoint(tempPoint.X + 1, tempPoint.Y, tempPoint.Type));
                else if (tempPoint.X > ToPoint.X)
                    points.Add(new RockMapPoint(tempPoint.X - 1, tempPoint.Y, tempPoint.Type));
                tempPoint = points.Last();
            }
        }
        return points;
    }
    public override string ToString()
    {
        return $"[{FromPoint.X},{FromPoint.Y}] -> [{ToPoint.X},{ToPoint.Y}]";
    }
}
[DebuggerDisplay("[{X}, {Y}] ({Type})")]
class RockMapPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    public PointType Type { get; set; }
    public RockMapPoint(string s)
    {
        int x = int.Parse(s.Split(',')[0]);
        int y = int.Parse(s.Split(',')[1]);
        Initialize(x, y, PointType.Rock);
    }
    public RockMapPoint(int x, int y) { Initialize(x, y, PointType.Air); }
    public RockMapPoint(int x, int y, PointType type) { Initialize(x, y, type); }
    private void Initialize(int x, int y, PointType type)
    {
        X = x;
        Y = y;
        Type = type;
    }
    public override string ToString()
    {
        return Type switch
        {
            PointType.Rock => "#",
            PointType.Sand => "o",
            _ => ".",
        };
    }
    public RockMapPoint Copy()
    {
        return new RockMapPoint(this.X, this.Y, this.Type);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as RockMapPoint);
    }
    public bool Equals(RockMapPoint other)
    {
        return other.X == this.X && other.Y == this.Y && other.Type == this.Type;
    }
}
enum PointType
{
    Air,
    Rock,
    Sand
}