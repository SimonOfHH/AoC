using System.Diagnostics;

namespace AdventOfCode;

public class Day_14 : BaseDay
{
    private readonly bool Sample = false;
    public Day_14()
    {
    }

    public override ValueTask<string> Solve_1()
    {
        (int left, int top) = Console.GetCursorPosition();
        var map = GetRockMapFromInput(1);
        map.Solve(left, top);
        return new ValueTask<string>((map.Sand.Count).ToString());
    }
    public override ValueTask<string> Solve_2()
    {
        (int left, int top) = Console.GetCursorPosition();
        var map = GetRockMapFromInput(2);
        map.Solve(left, top);
        return new ValueTask<string>((map.Sand.Count).ToString());
    }
    private RockMap GetRockMapFromInput(int part)
    {
        var input = ParseInput();
        (int minX, int maxX, int maxY) = GetMinMaxValues(input);
        if (part == 2)
        {
            maxY++;
            var line = new RockMapLine(new RockMapPoint(minX, maxY, PointType.Rock), new RockMapPoint(maxX, maxY, PointType.Rock)) { Infinite = true };
            input.Add(new RockMapPath(new List<RockMapLine>() { line }));
        }
        var map = new RockMap(minX, maxX, maxY) { Part = part };
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
    public RockMapPoint ActiveSand { get; set; }
    public PointType[,] PointsArray { get; set; }
    public List<RockMapPoint> Sand
    {
        get
        {
            var sandPoints = new List<RockMapPoint>();
            for (int x = WidthFrom; x < WidthTo; x++)
                for (int y = 0; y < Height; y++)
                    if (PointsArray[y, x] == PointType.Sand)
                        sandPoints.Add(new RockMapPoint(x, y));
            return sandPoints;
        }
    }
    public void Solve() => Solve(Console.GetCursorPosition().Left, Console.GetCursorPosition().Top);
    public void Solve(int left, int top)
    {
        while (!FlowIntoAbyss)
            CycleUntilRest(false, left, top);
        DrawMap(left, top, 0);
    }
    public RockMap(int fromWidth, int toWidth, int height)
    {
        Initialize(fromWidth, toWidth, height);
    }
    private void Initialize(int fromWidth, int toWidth, int height)
    {
        WidthFrom = fromWidth;
        WidthTo = toWidth;
        Height = height;
        InitializePoints();
    }
    private void InitializePoints()
    {
        PointsArray = new PointType[Height + 1, WidthTo + 1];
    }
    private RockMapPoint ExtendMapArrayAndGetNextPossibleMove()
    {
        WidthFrom -= 2;
        WidthTo += 2;
        // Temporary assignment to local var
        PointType[,] array = PointsArray;
        ResizeArray(ref array, Height + 1, WidthTo + 1);
        PointsArray = array;
        for (int x = WidthFrom - 1; x <= WidthTo; x++)
            if (ArrayPointExists(x, Height))
                PointsArray[Height, x] = PointType.Rock;
        return GetNextPossibleMove();
    }
    public void AddInputData(List<RockMapPath> paths)
    {
        paths.ForEach(path => path.Points.ForEach(point => PointsArray[point.Y, point.X] = PointType.Rock));
    }
    public void DrawMap() => DrawMap(Console.GetCursorPosition().Left, Console.GetCursorPosition().Top, 0);
    public void DrawMap(int left, int top, int sleep)
    {
        Console.SetCursorPosition(left, top);
        var values = new string[Height + 1];
        for (int y = 0; y < PointsArray.GetLength(0); y++)
            for (int x = WidthFrom; x < PointsArray.GetLength(1); x++)
                values[y] += TypeToString(PointsArray[y, x]);
        string map = string.Join(Environment.NewLine, values);
        Console.WriteLine(map);
        Console.WriteLine("");
        if (sleep > 0) // If we want a kind-of animated drawing output, use a short sleep
            Thread.Sleep(sleep);
    }
    public void CycleUntilRest() => CycleUntilRest(false, Console.GetCursorPosition().Left, Console.GetCursorPosition().Top);
    public void CycleUntilRest(bool draw, int left, int top)
    {
        if (IntoAbyss(true))
            return;
        ActiveSand = AddNewSandPoint();
        while (ActiveSand != null)
        {
            Cycle();
            if (draw)
                DrawMap(left, top, 5);
        }
    }
    private bool IntoAbyss(bool setActiveSandNull)
    {
        if (Part == 1)
        {
            if (ActiveSand != null)
                if ((ActiveSand.X == WidthFrom) || (ActiveSand.X == WidthTo) || (ActiveSand.Y >= Height - 1))
                {
                    FlowIntoAbyss = true;
                    if (setActiveSandNull) ActiveSand = null;
                    return true;
                }
        }
        if (PointsArray[0, 500] == PointType.Sand)
        {
            FlowIntoAbyss = true;
            if (setActiveSandNull) ActiveSand = null;
            return true;
        }
        return false;
    }
    public void Cycle()
    {
        if (ActiveSand == null)
        {
            if (IntoAbyss(false))
                return;
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
            switch (Part)
            {
                case 1:
                    if (IntoAbyss(true))
                        return;
                    break;
                case 2:
                    if ((ActiveSand.X <= WidthFrom) || (ActiveSand.X >= WidthTo))
                        nextMove = ExtendMapArrayAndGetNextPossibleMove();
                    break;
            }
            if (nextMove == null)
            {
                ActiveSand = null;
                return;
            }
        }
        MoveSand(ActiveSand, nextMove);
    }
    private void MoveSand(RockMapPoint from, RockMapPoint to)
    {
        PointsArray[from.Y, from.X] = PointType.Air;
        PointsArray[to.Y, to.X] = PointType.Sand;
        ActiveSand = to;
    }
    private RockMapPoint AddNewSandPoint()
    {
        var p = GetNewSandPoint();
        AddSandToMap(p);
        return p;
    }
    private void AddSandToMap(RockMapPoint sand)
    {
        PointsArray[sand.Y, sand.X] = sand.Type;
    }
    private static RockMapPoint GetNewSandPoint()
    {
        return new RockMapPoint(500, 0, PointType.Sand);
    }
    private bool ActiveSandCanMove()
    {
        (bool moveDown, bool moveDL, bool moveDR) = (true, true, true);

        if (ArrayPointExists(ActiveSand.X, ActiveSand.Y + 1))
            if (ArrayPoint(ActiveSand.X, ActiveSand.Y + 1) != PointType.Air) moveDown = false;
        if (ArrayPointExists(ActiveSand.X - 1, ActiveSand.Y + 1))
            if (ArrayPoint(ActiveSand.X - 1, ActiveSand.Y + 1) != PointType.Air) moveDL = false;
        if (ArrayPointExists(ActiveSand.X + 1, ActiveSand.Y + 1))
            if (ArrayPoint(ActiveSand.X + 1, ActiveSand.Y + 1) != PointType.Air) moveDR = false;

        return moveDown || moveDL || moveDR;
    }
    private bool ArrayPointExists(int x, int y)
    {
        if (PointsArray.GetLength(0) <= y)
            return false;
        if (PointsArray.GetLength(1) <= x)
            return false;
        var p = ArrayPoint(x, y);
        return true;
    }
    private PointType ArrayPoint(int x, int y)
    {
        return PointsArray[y, x];
    }
    private RockMapPoint GetNextPossibleMove()
    {
        if (ActiveSandCanMove() == false)
            return null;
        if (ArrayPointExists(ActiveSand.X, ActiveSand.Y + 1))
            if (ArrayPoint(ActiveSand.X, ActiveSand.Y + 1) == PointType.Air) return new RockMapPoint(ActiveSand.X, ActiveSand.Y + 1); // down
        if (ArrayPointExists(ActiveSand.X - 1, ActiveSand.Y + 1))
            if (ArrayPoint(ActiveSand.X - 1, ActiveSand.Y + 1) == PointType.Air) return new RockMapPoint(ActiveSand.X - 1, ActiveSand.Y + 1); // diagonal-left
        if (ArrayPointExists(ActiveSand.X + 1, ActiveSand.Y + 1))
            if (ArrayPoint(ActiveSand.X + 1, ActiveSand.Y + 1) == PointType.Air) return new RockMapPoint(ActiveSand.X + 1, ActiveSand.Y + 1); // diagonal-right
        return null;
    }
    private static string TypeToString(PointType type)
    {
        return type switch
        {
            PointType.Rock => "#",
            PointType.Sand => "o",
            _ => ".",
        };
    }
    private static void ResizeArray<T>(ref T[,] original, int newCoNum, int newRoNum)
    {
        var newArray = new T[newCoNum, newRoNum];
        int columnCount = original.GetLength(1);
        int columnCount2 = newRoNum;
        int columns = original.GetUpperBound(0);
        for (int co = 0; co <= columns; co++)
            Array.Copy(original, co * columnCount, newArray, co * columnCount2, columnCount);
        original = newArray;
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
    public bool Infinite { get; set; }
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
internal class RockMapPoint
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
    public RockMapPoint Copy() => new(this.X, this.Y, this.Type);
    public override string ToString()
    {
        return Type switch
        {
            PointType.Rock => "#",
            PointType.Sand => "o",
            _ => ".",
        };
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as RockMapPoint);
    }
    public bool Equals(RockMapPoint other)
    {
        return other.X == this.X && other.Y == this.Y && other.Type == this.Type;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Type);
    }
}
enum PointType
{
    Air,
    Rock,
    Sand
}