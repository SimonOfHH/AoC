namespace AdventOfCode;

public class Day_12 : BaseDay
{
    private readonly bool Sample = false;
    private Map Map { get; set; }
    public Day_12()
    {
    }

    public override ValueTask<string> Solve_1()
    {
        Map = ParseInput();
        var result = Map.FindPath();
        return new ValueTask<string>((result.Count - 1).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        Map = ParseInput();
        var result = Map.FindPaths();
        return new ValueTask<string>(result.Where(l => l.Value.Count > 0).Select(l => l.Value.Count - 1).Min().ToString());
    }
    private Map ParseInput()
    {
        return new Map(this.GetInput(Sample));
    }
}
class Map
{
    private List<Point> Points => Graph.Points.ToList();
    private Graph Graph { get; set; }
    private int[,] Matrix { get; set; }
    private Point StartPoint { get; set; }
    private Point EndPoint { get; set; }
    public Map(string[] input)
    {
        Matrix = GetMatrix(input);
        (StartPoint, EndPoint) = GetStartAndEndpoint(input);
        Graph = new Graph(Matrix);
    }
    public List<Point> FindPath()
    {
        return Graph.AStar(GetActualPointFromList(StartPoint),
                           GetActualPointFromList(EndPoint));
    }

    public Dictionary<Point, List<Point>> FindPaths()
    {
        var paths = new Dictionary<Point, List<Point>>();
        Points.Where(p => p.Cost == 1).ToList().ForEach(p => paths.Add(p, Graph.AStar(p, GetActualPointFromList(EndPoint))));
        return paths;
    }
    static (Point, Point) GetStartAndEndpoint(string[] input)
    {
        int startY = input.ToList().FindIndex(i => i.Contains('S'));
        int startX = input.ToList()[startY].IndexOf('S');

        int endY = input.ToList().FindIndex(i => i.Contains('E'));
        int endX = input.ToList()[endY].IndexOf('E');
        var startPoint = new Point() { X = startX, Y = startY };
        var endPoint = new Point() { X = endX, Y = endY };
        return (startPoint, endPoint);
    }
    private Point GetActualPointFromList(Point index)
    {
        return Points.First(p => p.X == index.X && p.Y == index.Y);
    }
    static int[,] GetMatrix(string[] input)
    {
        int[,] matrix = new int[input.Length, input[0].Length];
        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                char val = input[y][x];
                if (val == 'S')
                    val = 'a';
                if (val == 'E')
                    val = 'z';
                matrix[y, x] = GetValueForChar(val);
            }
        }
        return matrix;
    }
    static int GetValueForChar(char c)
    {
        return c - 96;
    }
}
class Graph
{
    public List<Point> Points { get; } = new();

    public Graph(int[,] Matrix)
    {
        for (int row = 0; row < Matrix.GetLength(0); row++)
        {
            for (int col = 0; col < Matrix.GetLength(1); col++)
            {
                Points.Add(new Point() { X = col, Y = row, Cost = Matrix[row, col] });
            }
        }
    }

    public IEnumerable<Point> GetNeighbours(Point point)
    {
        return Points.Where(p => p.Y == point.Y && Math.Abs(point.X - p.X) == 1 ||
                                 p.X == point.X && Math.Abs(point.Y - p.Y) == 1);
    }

    public List<Point> AStar(Point start, Point end)
    {
        var prioQueue = new PriorityQueue<Point, int>();
        prioQueue.Enqueue(start, 0);

        var cameFrom = new Dictionary<Point, Point>();
        var scores = new Dictionary<Point, int>()
                {
                    { start, 0}
                };
        var distanceScores = Points.Select(point => new { Key = point, Value = point.GetDistance(end) })
                                   .ToDictionary(x => x.Key, x => x.Value);

        while (prioQueue.Count > 0)
        {
            var current = prioQueue.Dequeue();
            if (current == end)
            {
                return DictToPathList(cameFrom, current);
            }

            foreach (Point neighbour in GetNeighbours(current))
            {
                var weight = GetWeight(current, neighbour);
                if (weight == null)
                    continue;

                var tempScore = scores[current] + weight.Value;
                if (!scores.ContainsKey(neighbour) || tempScore < scores[neighbour])
                {
                    cameFrom[neighbour] = current;
                    scores[neighbour] = tempScore;
                    distanceScores[neighbour] = tempScore + neighbour.GetDistance(end);
                    prioQueue.Enqueue(neighbour, distanceScores[neighbour]);
                }
            }
        }

        return new List<Point>();
    }
    private static int? GetWeight(Point p1, Point p2)
    {
        var score = p2.Cost > p1.Cost + 1
            ? null
            : (int?)p1.Cost - p2.Cost + 1;

        return score;
    }

    private static List<Point> DictToPathList(Dictionary<Point, Point> cameFrom, Point current)
    {
        List<Point> path = new() { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}

class Point : IEquatable<Point>
{
    public int X { get; set; }
    public int Y { get; set; }
    public int? Cost { get; set; }
    public int GetDistance(Point comparePoint)
    {
        return Math.Abs(this.X - comparePoint.X) + Math.Abs(this.Y - comparePoint.Y);
    }
    public override string ToString() => $"{X}, {Y} ({Cost})";
    public override bool Equals(object obj)
    {
        return Equals(obj as Point);
    }
    public bool Equals(Point other)
    {
        return other.X == this.X && other.Y == this.Y;
    }
    public override int GetHashCode()
    {
        int hCode = this.X ^ this.Y;
        return hCode.GetHashCode();
    }
}