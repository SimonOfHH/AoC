namespace AoC_2021;

public class Day_15 : BaseDay
{
    sealed record Node(SheepTools.Model.IntPoint Id, int Risk)
        : SheepTools.Model.GenericNode<SheepTools.Model.IntPoint>(Id)
    {
        public List<Node> Neighbours { get; private set; } = new List<Node>();

        public int ScoreFromStart { get; set; }
        public int EstimatedScoreFromStartToEndThroughThis { get; set; }

        private int _manhattanDistance = -1;
        public int ManhattanDistance => _manhattanDistance;

        public void CalculateNeighbours(List<Node> grid)
        {
            var squareSize = (int)(Math.Sqrt(grid.Count));

            if (Id.Y != squareSize)
            {
                var points = new List<SheepTools.Model.IntPoint>
                {
                    Id.Move(SheepTools.Model.Direction.Down),
                    Id.Move(SheepTools.Model.Direction.Up),
                    Id.Move(SheepTools.Model.Direction.Left),
                    Id.Move(SheepTools.Model.Direction.Right)
                }
                .Where(p => p.Y >= 0 && p.Y < squareSize && p.X >= 0 && p.X < squareSize)
                .ToList();

                Neighbours = points.Select(node => grid.First(n => n.Id.X == node.X && n.Id.Y == node.Y)).ToList();
            }
        }

        public void CalculateManhattanDistance(Node node)
        {
            _manhattanDistance = (int)Id.ManhattanDistance(node.Id);
        }

        #region Equals override, to avoid taking into account Connections for equality

        public override int GetHashCode() => base.GetHashCode();

        public bool Equals(Node? other) => base.Equals(other);

        #endregion
    }

    private readonly List<Node> _input;

    public Day_15()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        return new($"{AStarBacktracking(_input)}");
    }

    public override ValueTask<string> Solve_2()
    {
        var squareSize = (int)(Math.Sqrt(_input.Count));
        var modifiedInput = new List<Node>(_input);

        foreach (var p in _input)
        {
            for (int y = 0; y < 5; ++y)
            {
                var risk = Math.DivRem(p.Risk + y, 10, out int remainder) + remainder;
                for (int x = 0; x < 5; ++x)
                {
                    if (x == 0 && y == 0)
                    {
                        risk = Math.DivRem(risk + 1, 10, out remainder) + remainder;
                        continue;
                    }
                    modifiedInput.Add(new Node(p.Id with { X = p.Id.X + (x * squareSize), Y = p.Id.Y + (y * squareSize) }, risk));
                    risk = Math.DivRem(risk + 1, 10, out remainder) + remainder;
                }
            }
        }

        // Print(modifiedInput);

        var result = AStarBacktracking(modifiedInput);
        return new($"{result}");
    }

    private List<Node> ParseInput()
    {
        return File.ReadAllLines(InputFilePath).SelectMany((str, y) => str.Select((ch, x) => new Node(new SheepTools.Model.IntPoint(x, y), int.Parse(ch.ToString()))).ToList()).ToList();
    }

    /// <summary>
    /// A node needs all the previous used nodes to check success, so it cannot be deducted afterwards
    /// </summary>
    /// <param name="allNodes"></param>
    /// <param name="isSuccess"></param>
    private static int AStarBacktracking(List<Node> allNodes)
    {
        foreach (var node in allNodes)
        {
            node.ScoreFromStart = int.MaxValue;
            node.EstimatedScoreFromStartToEndThroughThis = int.MaxValue;
            node.CalculateNeighbours(allNodes);
            node.CalculateManhattanDistance(allNodes.Last());
        }

        var start = allNodes[0];
        var end = allNodes.Last();

        var priorityQueue = new PriorityQueue<Node, int>();

        start.ScoreFromStart = 0;
        start.EstimatedScoreFromStartToEndThroughThis = 0;
        priorityQueue.Enqueue(start, start.EstimatedScoreFromStartToEndThroughThis);

        while (priorityQueue.TryDequeue(out var current, out _))
        {
            if (current == end)
            {
                return current.ScoreFromStart;
            }

            foreach (var neighbour in current.Neighbours)
            {
                var tentativeScore = current.ScoreFromStart + neighbour.Risk;
                if (tentativeScore < neighbour.ScoreFromStart)
                {
                    neighbour.ScoreFromStart = tentativeScore;
                    neighbour.EstimatedScoreFromStartToEndThroughThis =tentativeScore + neighbour.ManhattanDistance;

                    // if (!priorityQueue.UnorderedItems.Any(pair => pair.Element == neighbour))
                    // {
                    priorityQueue.Enqueue(neighbour, neighbour.EstimatedScoreFromStartToEndThroughThis);
                    // }
                }
            }
        }

        throw new SolvingException();
    }

    private static void Print(List<Node> nodes)
    {
        Node? prevPNode = default;
        foreach (var p in nodes.OrderBy(p => p.Id.Y).ThenBy(p => p.Id.X))
        {
            if (p.Id.Y != prevPNode?.Id.Y)
            {
                Console.WriteLine();
            }
            Console.Write(p.Risk);
            prevPNode = p;
        }
        Console.WriteLine();
    }
}