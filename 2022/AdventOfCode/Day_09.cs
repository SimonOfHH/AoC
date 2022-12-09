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
        for (int i = 0; i < instruction.Steps; i++)
        {
            currPointHead.Move(instruction.Direction);
            UpdatePathHead(currPointHead.X, currPointHead.Y);
            if (!currPointHead.NextToPoint(currPointTail, true))
            {
                currPointTail.Move(currPointHead, instruction.Direction);
                UpdatePathTail(currPointTail.X, currPointTail.Y);
            }
        }
        LastHead = new Point(currPointHead.X, currPointHead.Y);
        LastTail = new Point(currPointTail.X, currPointTail.Y);
    }
    /*
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
        // Check for diagonal position
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
    */
    /*
    private Point MoveTail(Point head, Point tail, MoveDirection parentDirection)
    {
        if (parentDirection == MoveDirection.Right)
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
        if (parentDirection == MoveDirection.Left)
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
        if (parentDirection == MoveDirection.Top)
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
        if (parentDirection == MoveDirection.Below)
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
        return null;
    }
    */
    private void UpdatePathHead(int x, int y)
    {
        var point = new Point(x, y);
        PathHeadComplete.Add(point);
        LastHead = new Point(x, y);
    }
    private void UpdatePathTail(int x, int y)
    {
        var point = new Point(x, y);
        PathTailComplete.Add(point);
        LastTail = new Point(x, y);
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
    public void Move(MoveDirection direction)
    {
        if (direction == MoveDirection.Left)
            X--;
        if (direction == MoveDirection.Right)
            X++;
        if (direction == MoveDirection.Top)
            Y++;
        if (direction == MoveDirection.Below)
            Y--;
    }
    public void Move(Point head, MoveDirection parentDirection)
    {
        if (parentDirection == MoveDirection.Right)
        {
            if (head.Y != Y)
            {
                if (head.NextToPoint(new Point(X + 1, Y - 1), false))
                {
                    X++;
                    Y--;
                    return;
                }
                if (head.NextToPoint(new Point(X + 1, Y + 1), false))
                {
                    X++;
                    Y++;
                    return;
                }
            }
            if (head.NextToPoint(new Point(X + 1, Y), false))
            {
                X++;
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
                    return;
                }
                if (head.NextToPoint(new Point(X - 1, Y + 1), false))
                {
                    X--;
                    Y++;
                    return;
                }
            }
            if (head.NextToPoint(new Point(X - 1, Y), false))
            {
                X--;
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
                    return;
                }
                if (head.NextToPoint(new Point(X - 1, Y + 1), false))
                {
                    X--;
                    Y++;
                    return;
                }
            }
            if (head.NextToPoint(new Point(this.X, this.Y + 1), false))
            {
                Y++;
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
                    return;
                }
                if (head.NextToPoint(new Point(X - 1, Y - 1), false))
                {
                    X--;
                    Y--;
                    return;
                }
            }
            if (head.NextToPoint(new Point(X, Y - 1), false))
            {
                Y--;
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