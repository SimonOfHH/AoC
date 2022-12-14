using System.Diagnostics;

namespace AdventOfCode;

public class Day_14 : BaseDay
{
    private readonly bool Sample = true;
    public Day_14()
    {
    }

    public override ValueTask<string> Solve_1()
    {
        (int left, int top) = Console.GetCursorPosition();
        var map = GetRockMapFromInput();
        map.Part = 1;
        while (!map.FlowIntoAbyss)
        {
            map.CycleUntilRest(false, left, top);
            map.DrawMap(left, top);
        }
        return new ValueTask<string>((map.Sand.Count - 1).ToString());
    }

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    private RockMap GetRockMapFromInput()
    {
        var input = ParseInput();
        (int minX, int maxX, int maxY) = GetMinMaxValues(input);
        var map = new RockMap(minX, maxX, maxY);
        map.AddInputData(input);
        return map;
    }
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
    private static (int, int, int) GetMinMaxValues(List<RockMapPath> input)
    {
        int maxY = input.SelectMany(x => x.Points).Max(p => p.Y) + 1;
        int minX = input.SelectMany(x => x.Points).Min(p => p.X);
        int maxX = input.SelectMany(x => x.Points).Max(p => p.X);
        return (minX, maxX, maxY);
    }
}

class RockMap
{
    public int Part { get; set; }
    private int WidthFrom { get; set; }
    private int WidthTo { get; set; }
    private int Height { get; set; }
    public bool FlowIntoAbyss { get; private set; }
    private RockMapPoint ActiveSandBelow => Points.FirstOrDefault(p => p.X == ActiveSand.X && p.Y == ActiveSand.Y + 1);
    private RockMapPoint ActiveSandDiagonalLeft => Points.FirstOrDefault(p => p.X == ActiveSand.X - 1 && p.Y == ActiveSand.Y + 1);
    private RockMapPoint ActiveSandDiagonalRight => Points.FirstOrDefault(p => p.X == ActiveSand.X + 1 && p.Y == ActiveSand.Y + 1);
    public RockMapPoint ActiveSand { get; set; }
    public List<RockMapPoint> Sand { get; set; }
    public List<RockMapPoint> Points { get; set; }
    public RockMap(int fromWidth, int toWidth, int height)
    {
        Initialize(fromWidth, toWidth, height);
    }
    private void Initialize(int fromWidth, int toWidth, int height)
    {
        Points = new List<RockMapPoint>();
        Sand = new List<RockMapPoint>();
        WidthFrom = fromWidth;
        WidthTo = toWidth;
        Height = height;
        InitializePoints();
    }
    private void InitializePoints()
    {
        for (int x = WidthFrom; x <= WidthTo; x++)
            for (int y = 0; y <= Height; y++)
                Points.Add(new RockMapPoint(x, y));
    }
    public void AddInputData(List<RockMapPath> paths)
    {
        paths.ForEach(path => path.Points.ForEach(point => UpdatePoint(point, PointType.Rock)));
    }
    private void UpdatePoint(RockMapPoint p) => UpdatePoint(p, p.Type);
    private void UpdatePoint(RockMapPoint p, PointType type)
    {
        var sourcePoint = Points.First(ep => ep.X == p.X && ep.Y == p.Y);
        sourcePoint.Type = type;
    }
    public void DrawMap() => DrawMap(Console.GetCursorPosition().Left, Console.GetCursorPosition().Top);
    public void DrawMap(int left, int top)
    {
        Console.SetCursorPosition(left, top);
        var values = new string[Points.Max(p => p.Y) + 1];
        foreach (var p in Points)
            values[p.Y] += p.ToString();
        string map = string.Join(Environment.NewLine, values);
        Console.WriteLine(map);
    }
    public void CycleUntilRest() => CycleUntilRest(false, Console.GetCursorPosition().Left, Console.GetCursorPosition().Top);
    public void CycleUntilRest(bool draw, int left, int top)
    {
        ActiveSand = AddNewSandPoint();
        int rounds = 0;
        while (ActiveSand != null)
        {
            Cycle();
            if (draw)
            {
                DrawMap(left, top);
                Thread.Sleep(250);
            }
            rounds++;
        }
    }
    public void Cycle()
    {
        if (ActiveSand == null)
        {
            ActiveSand = AddNewSandPoint();
            return;
        }
        if (!ActiveSandCanMove())
        {
            ActiveSand = null;
            return;
        }
        var nextMove = GetNextPossibleMove();
        if (nextMove == null)
        {
            if ((ActiveSand.X == WidthFrom) || (ActiveSand.X == WidthTo))
                FlowIntoAbyss = true;
            if (ActiveSand.Y >= Height - 1)
                FlowIntoAbyss = true;
            ActiveSand = null;
            return;
        }
        MoveSand(ActiveSand, nextMove);
    }
    private void MoveSand(RockMapPoint from, RockMapPoint to)
    {
        UpdatePoint(from, PointType.Air);
        UpdatePoint(to, PointType.Sand);
        ActiveSand = to;
    }
    private RockMapPoint AddNewSandPoint()
    {
        var p = GetNewSandPoint();
        AddSandToMap(p);
        Sand.Add(p);
        return p;
    }
    private void AddSandToMap(RockMapPoint sand)
    {
        UpdatePoint(sand);
    }
    private static RockMapPoint GetNewSandPoint()
    {
        return new RockMapPoint(500, 0, PointType.Sand);
    }
    private bool ActiveSandCanMove()
    {
        (bool moveDown, bool moveDL, bool moveDR) = (true, true, true);
        if (ActiveSandBelow != null)
            if (ActiveSandBelow.Type == PointType.Rock) moveDown = false;
        if (ActiveSandDiagonalLeft != null)
            if (ActiveSandDiagonalLeft.Type == PointType.Rock) moveDL = false;
        if (ActiveSandDiagonalRight != null)
            if (ActiveSandDiagonalRight.Type == PointType.Rock) moveDR = false;
        return moveDown || moveDL || moveDR;
    }
    private RockMapPoint GetNextPossibleMove()
    {
        if (ActiveSandCanMove() == false)
            return null;
        if (ActiveSandBelow != null)
            if (ActiveSandBelow.Type == PointType.Air) return ActiveSandBelow;
        if (ActiveSandDiagonalLeft != null)
            if (ActiveSandDiagonalLeft.Type == PointType.Air) return ActiveSandDiagonalLeft;
        if (ActiveSandDiagonalRight != null)
            if (ActiveSandDiagonalRight.Type == PointType.Air) return ActiveSandDiagonalRight;

        return null;
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