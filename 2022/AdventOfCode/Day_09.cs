namespace AdventOfCode;

public class Day_09 : BaseDay
{
    private readonly bool Sample = false;
    private Map map;
    public Day_09()
    {
        map = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new(map.PathTailDistinct.Count.ToString());

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");

    private Map ParseInput()
    {
        return new Map(this.GetInput(Sample));
    }
}
class Map
{
    public List<MapInstruction> Instructions { get; set; }
    public List<Point> PathTailDistinct => PathTailComplete.Distinct(new PointEqualityComparer()).ToList();
    public List<Point> PathHeadComplete { get; set; }
    public List<Point> PathTailComplete { get; set; }
    public Point LastHead { get; set; }
    public Point LastTail { get; set; }
    public Map(string[] input)
    {
        LastHead = new Point(0, 0);
        LastTail = new Point(0, 0);

        PathHeadComplete = new List<Point>();
        PathTailComplete = new List<Point>();
        PathHeadComplete.Add(LastHead);
        PathTailComplete.Add(LastTail);

        Instructions = new List<MapInstruction>();

        input.ToList().ForEach(x => Instructions.Add(new MapInstruction(x)));
        foreach (var instruction in Instructions)
            ProcessInstruction(instruction);
    }
    private void ProcessInstruction(MapInstruction instruction)
    {
        var currPointHead = LastHead;
        var currPointTail = LastTail;
        for (int i = 0; i < instruction.Steps; i++)
        {
            currPointHead.Move(instruction.Direction, PathHeadComplete);
            if (!currPointHead.NextToPoint(currPointTail, true))
                currPointTail.Move(currPointHead, instruction.Direction, PathTailComplete);
        }
    }
}
class MapInstruction
{
    public MoveDirection Direction { get; set; }
    public int Steps { get; set; }
    public MapInstruction(string s)
    {
        Direction = GetDirectionFromShortString(s.Split(" ")[0].Trim());
        Steps = int.Parse(s.Split(" ")[1].Trim());
    }
    private static MoveDirection GetDirectionFromShortString(string s)
    {
        switch (s)
        {
            case "L": return MoveDirection.Left;
            case "R": return MoveDirection.Right;
            case "U": return MoveDirection.Top;
            case "D": return MoveDirection.Below;
            default: throw new Exception("Wrong type");
        }
    }
    public override string ToString()
    {
        return String.Format("{0} {1}", Direction, Steps);
    }
}
class Point
{
    public int X { get; set; }
    public int Y { get; set; }
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool NextToPoint(Point point, bool checkSamePosition)
    {
        if (checkSamePosition)
            if ((point.X == this.X) && (point.Y == this.Y)) // Same Position
                return true;
        if (point.X == this.X) // same row
        {
            if ((point.Y - 1 == this.Y) || (point.Y + 1 == this.Y))
                return true;
        }
        if (point.Y == this.Y) // same column
        {
            if ((point.X - 1 == this.X) || (point.X + 1 == this.X))
                return true;
        }
        // Check for diagonal position
        if ((point.X - 1 == this.X) && ((point.Y - 1 == this.Y) || (point.Y - 1 == this.Y)))
            return true;
        if ((point.X + 1 == this.X) && ((point.Y - 1 == this.Y) || (point.Y + 1 == this.Y)))
            return true;
        if ((point.Y + 1 == this.Y) && ((point.X - 1 == this.X) || (point.X - 1 == this.X)))
            return true;
        if ((point.Y - 1 == this.Y) && ((point.X - 1 == this.X) || (point.X + 1 == this.X)))
            return true;
        return false;
    }
    public void Move(MoveDirection direction, List<Point> pathHead)
    {
        if (direction == MoveDirection.Left)
            X--;
        if (direction == MoveDirection.Right)
            X++;
        if (direction == MoveDirection.Top)
            Y++;
        if (direction == MoveDirection.Below)
            Y--;
        pathHead.Add(new Point(X, Y));
    }
    public void Move(Point head, MoveDirection parentDirection, List<Point> path)
    {
        if (parentDirection == MoveDirection.Right)
        {
            if (head.Y != Y)
            {
                if (head.NextToPoint(new Point(X + 1, Y - 1), false))
                {
                    X++;
                    Y--;
                    path.Add(new Point(X, Y));
                    return;
                }
                if (head.NextToPoint(new Point(X + 1, Y + 1), false))
                {
                    X++;
                    Y++;
                    path.Add(new Point(X, Y));
                    return;
                }
            }
            if (head.NextToPoint(new Point(X + 1, Y), false))
            {
                X++;
                path.Add(new Point(X, Y));
                return;
            }
        }
        if (parentDirection == MoveDirection.Left)
        {
            if (head.Y != Y)
            {
                if (head.NextToPoint(new Point(X - 1, Y - 1), false))
                {
                    X--;
                    Y--;
                    path.Add(new Point(X, Y));
                    return;
                }
                if (head.NextToPoint(new Point(X - 1, Y + 1), false))
                {
                    X--;
                    Y++;
                    path.Add(new Point(X, Y));
                    return;
                }
            }
            if (head.NextToPoint(new Point(X - 1, Y), false))
            {
                X--;
                path.Add(new Point(X, Y));
                return;
            }
        }
        if (parentDirection == MoveDirection.Top)
        {
            if (head.X != X)
            {
                if (head.NextToPoint(new Point(X + 1, Y + 1), false))
                {
                    X++;
                    Y++;
                    path.Add(new Point(X, Y));
                    return;
                }
                if (head.NextToPoint(new Point(X - 1, Y + 1), false))
                {
                    X--;
                    Y++;
                    path.Add(new Point(X, Y));
                    return;
                }
            }
            if (head.NextToPoint(new Point(this.X, this.Y + 1), false))
            {
                Y++;
                path.Add(new Point(X, Y));
                return;
            }
        }
        if (parentDirection == MoveDirection.Below)
        {
            if (head.X != this.X)
            {
                if (head.NextToPoint(new Point(X + 1, Y - 1), false))
                {
                    X++;
                    Y--;
                    path.Add(new Point(X, Y));
                    return;
                }
                if (head.NextToPoint(new Point(X - 1, Y - 1), false))
                {
                    X--;
                    Y--;
                    path.Add(new Point(X, Y));
                    return;
                }
            }
            if (head.NextToPoint(new Point(X, Y - 1), false))
            {
                Y--;
                path.Add(new Point(X, Y));
                return;
            }
        }
    }
    public override string ToString()
    {
        return String.Format("X={0} Y={1}", X, Y);
    }
}
class PointEqualityComparer : IEqualityComparer<Point>
{
    public bool Equals(Point p1, Point p2)
    {
        if (p2 == null && p1 == null)
            return true;
        else if (p1 == null || p2 == null)
            return false;
        else if (p1.X == p2.X && p1.Y == p2.Y)
            return true;
        else
            return false;
    }

    public int GetHashCode(Point p)
    {
        int hCode = p.X ^ p.Y;
        return hCode.GetHashCode();
    }
}
enum MoveDirection
{
    Left,
    Right,
    Top,
    Below
}