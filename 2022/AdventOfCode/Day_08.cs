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
        return new Grid(this.GetInput(Sample).Where(x => !String.IsNullOrEmpty(x)).ToArray());
    }
}

public class Grid
{
    public List<Tree> Trees { get; set; }
    public List<Tree> VisibleTrees => Trees.Where(tree => tree.Visible).ToList();
    public List<Tree> HiddenTrees => Trees.Where(tree => !tree.Visible).ToList();
    public List<Tree> ScenicTrees => Trees.OrderByDescending(tree => tree.ScenicScore).ToList();
    public Grid(string[] input)
    {
        Trees = new List<Tree>();
        for (int i = 0; i < input.Length; i++)
        {
            for (int i2 = 0; i2 < input[i].Length; i2++)
            {
                Trees.Add(new Tree(i, i2, int.Parse(input[i][i2].ToString())));
            }
        }
        UpdateTrees();
    }
    private void UpdateTrees()
    {
        foreach (var tree in Trees)
        {
            tree.Neighbours.Add(Direction.Left, GetTrees(tree.X, tree.Y, Direction.Left));
            tree.Neighbours.Add(Direction.Right, GetTrees(tree.X, tree.Y, Direction.Right));
            tree.Neighbours.Add(Direction.Top, GetTrees(tree.X, tree.Y, Direction.Top));
            tree.Neighbours.Add(Direction.Below, GetTrees(tree.X, tree.Y, Direction.Below));
            tree.Visible = tree.IsVisible();
            tree.ScenicScore = tree.GetScenicScore();
        }
    }
    private List<Tree> GetTrees(int row, int column, Direction direction)
    {
        switch (direction)
        {
            case Direction.Left: return Trees.Where(tree => tree.X == row && tree.Y < column).ToList();
            case Direction.Right: return Trees.Where(tree => tree.X == row && tree.Y > column).ToList();
            case Direction.Top: return Trees.Where(tree => tree.X < row && tree.Y == column).ToList();
            case Direction.Below: return Trees.Where(tree => tree.X > row && tree.Y == column).ToList();
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
    public Tree(int x, int y, int value)
    {
        X = x;
        Y = y;
        Value = value;
        Neighbours = new Dictionary<Direction, List<Tree>>();
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
        if (Neighbours[Direction.Left].Count == 0)
            return true;
        if (Neighbours[Direction.Right].Count == 0)
            return true;
        if (Neighbours[Direction.Top].Count == 0)
            return true;
        if (Neighbours[Direction.Below].Count == 0)
            return true;
        return false;
    }
    public int GetScenicScore()
    {
        int left = Neighbours[Direction.Left].OrderByDescending(tree => tree.Y).TakeUntil(x => x.Value >= Value).Count();
        int right = Neighbours[Direction.Right].OrderBy(tree => tree.Y).TakeUntil(x => x.Value >= Value).Count();
        int top = Neighbours[Direction.Top].OrderByDescending(tree => tree.X).TakeUntil(x => x.Value >= Value).Count();
        int below = Neighbours[Direction.Below].OrderBy(tree => tree.X).TakeUntil(x => x.Value >= Value).Count();
        return left * right * top * below;

    }
    public override string ToString()
    {
        return String.Format("X={0},Y={1} ({2}) (Scenic: {3})", X, Y, Value, ScenicScore);
    }
}
public enum Direction
{
    Left,
    Right,
    Top,
    Below
}