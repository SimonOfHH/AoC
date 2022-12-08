using System.Text;
using MoreLinq.Extensions;

namespace AdventOfCode;

public class Day_08 : BaseDay
{
    private Grid TreeGrid { get; set; }
    private readonly bool Sample = false;
    public Day_08()
    {
        TreeGrid = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new(TreeGrid.VisibleTrees.Count.ToString());

    public override ValueTask<string> Solve_2() => new(TreeGrid.ScenicTrees.First().ScenicScore.ToString());

    private Grid ParseInput()
    {
        return new Grid(this.GetInput(Sample));
    }
}

public class Grid
{
    public Tree[,] TreesArray { get; set; }
    public List<Tree> FlattenedArray { get; set; }
    public List<Tree> VisibleTrees => FlattenedArray.Where(tree => tree.Visible).ToList();
    public List<Tree> HiddenTrees => FlattenedArray.Where(tree => !tree.Visible).ToList();
    public List<Tree> ScenicTrees => FlattenedArray.OrderByDescending(tree => tree.ScenicScore).ToList();
    public Grid(string[] input)
    {
        TreesArray = new Tree[input.Length, input[0].Length];
        for (int i = 0; i < input.Length; i++)
        {
            for (int i2 = 0; i2 < input[i].Length; i2++)
                TreesArray[i, i2] = new Tree(i, i2, int.Parse(input[i][i2].ToString()));
        }
        UpdateTreesArray();
        FlattenedArray = TreesArray.Flatten().Cast<Tree>().ToList();
    }
    private void UpdateTreesArray()
    {
        for (int i = 0; i < TreesArray.GetLength(0); i++)
        {
            for (int i2 = 0; i2 < TreesArray.GetLength(1); i2++)
            {
                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                    TreesArray[i, i2].Neighbours[direction] = GetNeighbourTrees(TreesArray[i, i2], direction);
                TreesArray[i, i2].UpdateValues();
            }
        }
    }
    private List<Tree> GetNeighbourTrees(Tree sourceTree, Direction direction)
    {
        var list = new List<Tree>();
        switch (direction)
        {
            case Direction.Left:
                for (int i = sourceTree.X - 1; i >= 0; i--)
                    list.Add(TreesArray[sourceTree.Y, i]);
                return list;
            case Direction.Right:
                for (int i = sourceTree.X + 1; i < TreesArray.GetLength(1); i++)
                    list.Add(TreesArray[sourceTree.Y, i]);
                return list;
            case Direction.Top:
                for (int i = sourceTree.Y - 1; i >= 0; i--)
                    list.Add(TreesArray[i, sourceTree.X]);
                return list;
            case Direction.Below:
                for (int i = sourceTree.Y + 1; i < TreesArray.GetLength(0); i++)
                    list.Add(TreesArray[i, sourceTree.X]);
                return list;
            default:
                return null;
        }
    }
}
public class Tree
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Value { get; set; }
    public bool Visible { get; set; }
    public int ScenicScore { get; set; }
    public Dictionary<Direction, List<Tree>> Neighbours { get; set; }
    public Tree(int y, int x, int value)
    {
        X = x;
        Y = y;
        Value = value;
        Neighbours = new Dictionary<Direction, List<Tree>>();
        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            Neighbours.Add(direction, new List<Tree>());
    }
    public void UpdateValues()
    {
        Visible = IsVisible();
        ScenicScore = GetScenicScore();
    }
    public bool IsVisible()
    {
        if (OnEdge())
            return true;
        if (Neighbours[Direction.Left].All(x => x.Value < this.Value))
            return true;
        if (Neighbours[Direction.Right].All(x => x.Value < this.Value))
            return true;
        if (Neighbours[Direction.Top].All(x => x.Value < this.Value))
            return true;
        if (Neighbours[Direction.Below].All(x => x.Value < this.Value))
            return true;
        return false;
    }
    private bool OnEdge()
    {
        if (Neighbours[Direction.Left]?.Count == 0)
            return true;
        if (Neighbours[Direction.Right]?.Count == 0)
            return true;
        if (Neighbours[Direction.Top]?.Count == 0)
            return true;
        if (Neighbours[Direction.Below]?.Count == 0)
            return true;
        return false;
    }
    public int GetScenicScore()
    {
        int left = Neighbours[Direction.Left].OrderByDescending(tree => tree.X).TakeUntil(x => x.Value >= Value).Count();
        int right = Neighbours[Direction.Right].OrderBy(tree => tree.X).TakeUntil(x => x.Value >= Value).Count();
        int top = Neighbours[Direction.Top].OrderByDescending(tree => tree.Y).TakeUntil(x => x.Value >= Value).Count();
        int below = Neighbours[Direction.Below].OrderBy(tree => tree.Y).TakeUntil(x => x.Value >= Value).Count();
        return left * right * top * below;

    }
    public override string ToString()
    {
        return String.Format("X={0},Y={1} ({2}) (Scenic: {3})", Y, X, Value, ScenicScore);
    }
}
public enum Direction
{
    Left,
    Right,
    Top,
    Below
}