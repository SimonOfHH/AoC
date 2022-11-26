namespace AoC_2021;

public class Day_12 : BaseDay
{
    sealed record class Node : SheepTools.Model.GenericNode<string>
    {
        private const string StartNodeId = "start";
        private const string EndNodeId = "end";

        public bool IsMajor { get; init; }
        public bool IsStart { get; init; }
        public bool IsEnd { get; init; }

        public HashSet<Node> Connections { get; } = new HashSet<Node>();

        public Node(string id) : base(id)
        {
            IsMajor = char.IsUpper(id[0]);
            IsStart = id == StartNodeId;
            IsEnd = id == EndNodeId;
        }

        #region Equals override, to avoid taking into account Connections for equality

        public override int GetHashCode() => base.GetHashCode();

        public bool Equals(Node? other) => base.Equals(other);

        #endregion
    }
    
    private readonly HashSet<Node> _input;

    public Day_12()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        var start = _input.First(n => n.IsStart);
        List<(string Path, Node LastNode)> pathList = new(1000 * _input.Count) { (start.Id, start) };

        while (true)
        {
            List<(string Path, Node LastNode)> newPathList = new(10 * pathList.Count);
            foreach (var path in pathList)
            {
                if (path.LastNode.IsEnd)
                {
                    newPathList.Add(path);
                }
                else
                {
                    foreach (var connection in path.LastNode.Connections)
                    {
                        if (connection.IsMajor || (!connection.IsStart && !path.Path.Contains($",{connection.Id},")))
                        {
                            var newPath = ($"{path.Path},{connection.Id}", connection);
                            newPathList.Add(newPath);
                        }
                    }
                }
            }

            if (pathList.Count == newPathList.Count)
            {
                break;
            }

            pathList = newPathList;
        }

        return new($"{pathList.Count}");
    }

    public override ValueTask<string> Solve_2()
    {
        var start = _input.First(n => n.IsStart);
        List<List<Node>> pathList = new(1000 * _input.Count) { new() { start } };

        while (true)
        {
            List<List<Node>> newPathList = new(10 * pathList.Count);
            foreach (var path in pathList)
            {
                var last = path.Last();
                if (last.IsEnd)
                {
                    newPathList.Add(path);
                }
                else
                {
                    foreach (var connection in last.Connections)
                    {
                        var minorCaves = path.Where(c => !c.IsMajor);
                        var isMinorCaveRepeated = minorCaves.Distinct().Count() != minorCaves.Count();

                        if (connection.IsMajor || (!connection.IsStart && (!isMinorCaveRepeated || !path.Contains(connection))))
                        {
                            newPathList.Add(path.Append(connection).ToList());
                        }
                    }
                }
            }

            if (pathList.Count == newPathList.Count)
            {
                break;
            }

            pathList = newPathList;
        }

        return new($"{pathList.Count}");
    }

    private HashSet<Node> ParseInput()
    {
        var nodeList = new HashSet<Node>(100);

        foreach (var line in new ParsedFile(InputFilePath, new[] { '-' }).ToList())
        {
            var origin = new Node(line.NextElement<string>());
            var destination = new Node(line.NextElement<string>());

            if (!nodeList.Add(origin))
            {
                origin = nodeList.First(n => n.Id == origin.Id);
            }
            if (!nodeList.Add(destination))
            {
                destination = nodeList.First(n => n.Id == destination.Id);
            }

            origin.Connections.Add(destination);
            destination.Connections.Add(origin);
        }

        return nodeList;
    }
}