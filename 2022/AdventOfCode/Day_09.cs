namespace AdventOfCode;

public class Day_09 : BaseDay
{
    private readonly bool Sample = false;
    private Map map;
    public Day_09()
    {
        map = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new(map.PathTail.Count.ToString());

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");

    private Map ParseInput()
    {
        return new Map(this.GetInput(Sample));
    }
}
public class Map
{
    public List<MapInstruction> Instructions { get; set; }
    public Dictionary<Point, int> PathHead { get; set; }
    public Dictionary<Point, int> PathTail { get; set; }
    public List<Point> PathHeadComplete { get; set; }
    public List<Point> PathTailComplete { get; set; }
    public Point LastHead { get; set; }
    public Point LastTail { get; set; }
    public Map(string[] input)
    {
        PathHead = new Dictionary<Point, int>();
        PathTail = new Dictionary<Point, int>();
        PathHeadComplete = new List<Point>();
        PathTailComplete = new List<Point>();

        Instructions = new List<MapInstruction>();
        UpdatePathHead(0, 0);
        UpdatePathTail(0, 0);

        input.ToList().ForEach(x => Instructions.Add(new MapInstruction(x)));
        foreach (var instruction in Instructions)
            ProcessInstruction(instruction);
    }
    private void ProcessInstruction(MapInstruction instruction)
    {
        var currPointHead = LastHead;
        var currPointTail = LastTail;
        switch (instruction.Direction)
        {
            case Direction.Left:
                for (int i = 0; i < instruction.Steps; i++)
                {
                    currPointHead.X--;
                    UpdatePathHead(currPointHead.X, currPointHead.Y);
                    if (!IsTailNextToHead(currPointHead, currPointTail, true))
                        currPointTail = MoveTail(currPointHead, currPointTail, instruction.Direction);
                }
                break;
            case Direction.Right:
                for (int i = 0; i < instruction.Steps; i++)
                {
                    currPointHead.X++;
                    UpdatePathHead(currPointHead.X, currPointHead.Y);
                    if (!IsTailNextToHead(currPointHead, currPointTail, true))
                        currPointTail = MoveTail(currPointHead, currPointTail, instruction.Direction);
                }
                break;
            case Direction.Top:
                for (int i = 0; i < instruction.Steps; i++)
                {
                    currPointHead.Y++;
                    UpdatePathHead(currPointHead.X, currPointHead.Y);
                    if (!IsTailNextToHead(currPointHead, currPointTail, true))
                        currPointTail = MoveTail(currPointHead, currPointTail, instruction.Direction);
                }
                break;
            case Direction.Below:
                for (int i = 0; i < instruction.Steps; i++)
                {
                    currPointHead.Y--;
                    UpdatePathHead(currPointHead.X, currPointHead.Y);
                    if (!IsTailNextToHead(currPointHead, currPointTail, true))
                        currPointTail = MoveTail(currPointHead, currPointTail, instruction.Direction);
                }
                break;
        }
        LastHead = new Point(currPointHead.X, currPointHead.Y);
        LastTail = new Point(currPointTail.X, currPointTail.Y);
    }
    private bool IsTailNextToHead(Point head, Point tail, bool checkSamePosition)
    {
        if (checkSamePosition)
            if ((head.X == tail.X) && (head.Y == tail.Y)) // Same Position
                return true;
        if (head.X == tail.X) // same row
        {
            if ((head.Y - 1 == tail.Y) || (head.Y + 1 == tail.Y))
                return true;
        }
        if (head.Y == tail.Y) // same column
        {
            if ((head.X - 1 == tail.X) || (head.X + 1 == tail.X))
                return true;
        }
        if ((head.X - 1 == tail.X) && ((head.Y - 1 == tail.Y) || (head.Y - 1 == tail.Y)))
            return true;
        if ((head.X + 1 == tail.X) && ((head.Y - 1 == tail.Y) || (head.Y + 1 == tail.Y)))
            return true;
        if ((head.Y + 1 == tail.Y) && ((head.X - 1 == tail.X) || (head.X - 1 == tail.X)))
            return true;
        if ((head.Y - 1 == tail.Y) && ((head.X - 1 == tail.X) || (head.X + 1 == tail.X)))
            return true;
        return false;
    }
    private Point MoveTail(Point head, Point tail, Direction parentDirection)
    {
        if (parentDirection == Direction.Right)
        {
            if (head.Y != tail.Y)
            {
                if (IsTailNextToHead(head, new Point(tail.X + 1, tail.Y - 1), false))
                {
                    tail.X++;
                    tail.Y--;
                    UpdatePathTail(tail.X, tail.Y);
                    return tail;
                }
                if (IsTailNextToHead(head, new Point(tail.X + 1, tail.Y + 1), false))
                {
                    tail.X++;
                    tail.Y++;
                    UpdatePathTail(tail.X, tail.Y);
                    return tail;
                }
            }
            if (IsTailNextToHead(head, new Point(tail.X + 1, tail.Y), false))
            {
                tail.X++;
                UpdatePathTail(tail.X, tail.Y);
                return tail;
            }
        }
        if (parentDirection == Direction.Left)
        {
            if (head.Y != tail.Y)
            {
                if (IsTailNextToHead(head, new Point(tail.X - 1, tail.Y - 1), false))
                {
                    tail.X--;
                    tail.Y--;
                    UpdatePathTail(tail.X, tail.Y);
                    return tail;
                }
                if (IsTailNextToHead(head, new Point(tail.X - 1, tail.Y + 1), false))
                {
                    tail.X--;
                    tail.Y++;
                    UpdatePathTail(tail.X, tail.Y);
                    return tail;
                }
            }
            if (IsTailNextToHead(head, new Point(tail.X - 1, tail.Y), false))
            {
                tail.X--;
                UpdatePathTail(tail.X, tail.Y);
                return tail;
            }
        }
        if (parentDirection == Direction.Top)
        {
            if (head.X != tail.X)
            {
                if (IsTailNextToHead(head, new Point(tail.X + 1, tail.Y + 1), false))
                {
                    tail.X++;
                    tail.Y++;
                    UpdatePathTail(tail.X, tail.Y);
                    return tail;
                }
                if (IsTailNextToHead(head, new Point(tail.X - 1, tail.Y + 1), false))
                {
                    tail.X--;
                    tail.Y++;
                    UpdatePathTail(tail.X, tail.Y);
                    return tail;
                }
            }
            if (IsTailNextToHead(head, new Point(tail.X, tail.Y + 1), false))
            {
                tail.Y++;
                UpdatePathTail(tail.X, tail.Y);
                return tail;
            }
        }
        if (parentDirection == Direction.Below)
        {
            if (head.X != tail.X)
            {
                if (IsTailNextToHead(head, new Point(tail.X + 1, tail.Y - 1), false))
                {
                    tail.X++;
                    tail.Y--;
                    UpdatePathTail(tail.X, tail.Y);
                    return tail;
                }
                if (IsTailNextToHead(head, new Point(tail.X - 1, tail.Y - 1), false))
                {
                    tail.X--;
                    tail.Y--;
                    UpdatePathTail(tail.X, tail.Y);
                    return tail;
                }
            }
            if (IsTailNextToHead(head, new Point(tail.X, tail.Y - 1), false))
            {
                tail.Y--;
                UpdatePathTail(tail.X, tail.Y);
                return tail;
            }
        }
        /*
        if (IsTailNextToHead(head, new Point(tail.X + 1, tail.Y + 1), false))
        {
            tail.X++;
            tail.Y++;
            UpdatePathTail(tail.X, tail.Y);
            return tail;
        }
        if (IsTailNextToHead(head, new Point(tail.X - 1, tail.Y + 1), false))
        {
            tail.X--;
            tail.Y++;
            UpdatePathTail(tail.X, tail.Y);
            return tail;
        }
        if (IsTailNextToHead(head, new Point(tail.X + 1, tail.Y - 1), false))
        {
            tail.X++;
            tail.Y--;
            UpdatePathTail(tail.X, tail.Y);
            return tail;
        }
        if (IsTailNextToHead(head, new Point(tail.X - 1, tail.Y - 1), false))
        {
            tail.X--;
            tail.Y--;
            UpdatePathTail(tail.X, tail.Y);
            return tail;
        }
        if (IsTailNextToHead(head, new Point(tail.X, tail.Y + 1), false))
        {
            tail.Y++;
            UpdatePathTail(tail.X, tail.Y);
            return tail;
        }
        if (IsTailNextToHead(head, new Point(tail.X, tail.Y - 1), false))
        {
            tail.Y--;
            UpdatePathTail(tail.X, tail.Y);
            return tail;
        }
        */
        return null;
    }
    private void UpdatePathHead(int x, int y)
    {
        var point = new Point(x, y);
        PathHeadComplete.Add(point);
        var existingPoint = PathHead.FirstOrDefault(p => p.Key.X == x && p.Key.Y == y).Key;
        if (PathHead.ContainsKey(point))
            PathHead[point]++;
        else
            PathHead.Add(point, 1);
        LastHead = new Point(x, y);
    }
    private void UpdatePathTail(int x, int y)
    {
        var point = new Point(x, y);
        PathTailComplete.Add(point);
        var existingPoint = PathTail.FirstOrDefault(p => p.Key.X == x && p.Key.Y == y).Key;
        if (existingPoint != null)
            PathTail[existingPoint]++;
        else
            PathTail.Add(point, 1);
        LastTail = new Point(x, y);
    }
}
public class MapInstruction
{
    public Direction Direction { get; set; }
    public int Steps { get; set; }
    public MapInstruction(string s)
    {
        Direction = GetDirectionFromShortString(s.Split(" ")[0].Trim());
        Steps = int.Parse(s.Split(" ")[1].Trim());
    }
    private Direction GetDirectionFromShortString(string s)
    {
        switch (s)
        {
            case "L": return Direction.Left;
            case "R": return Direction.Right;
            case "U": return Direction.Top;
            case "D": return Direction.Below;
            default: throw new Exception("Wrong type");
        }
    }
    public override string ToString()
    {
        return String.Format("{0} {1}", Direction, Steps);
    }
}
public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
    public override string ToString()
    {
        return String.Format("X={0} Y={1}", X, Y);
    }
}