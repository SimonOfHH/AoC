namespace AdventOfCode;

public class Day_03 : BaseDay
{
    public bool Sample = false;
    public List<Rucksack> Rucksacks;
    public Day_03()
    {
        Rucksacks = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new(Rucksacks.Sum(x => x.PriorityValue).ToString());

    public override ValueTask<string> Solve_2() => new(GroupRucksacks().Sum(x => x.PriorityValue).ToString());

    private List<Rucksack> ParseInput()
    {
        var rucksacks = new List<Rucksack>();
        foreach (var line in this.GetInput(Sample).Where(x => !String.IsNullOrEmpty(x)))
        {
            rucksacks.Add(new Rucksack(line));
        }
        return rucksacks;
    }
    private List<RucksackGroup> GroupRucksacks()
    {
        var group = new List<RucksackGroup>();
        foreach (var chunk in Rucksacks.Chunk(3))
        {
            group.Add(new RucksackGroup(chunk.ToList()));
        }
        return group;
    }
}

public class Rucksack
{
    public List<char> AllItems;
    public List<char> Compartment1;
    public List<char> Compartment2;

    public int PriorityValue
    {
        get { return GetSameItem().PriorityValue(); }
    }
    public Rucksack(string s)
    {
        AllItems = s.ToList();
        Compartment1 = AllItems.Chunk(AllItems.Count() / 2).First().ToList();
        Compartment2 = AllItems.Chunk(AllItems.Count() / 2).Last().ToList();
    }
    private char GetSameItem()
    {
        return Compartment1.Where(c => Compartment2.Contains(c)).First();
    }
}
public class RucksackGroup
{
    public List<Rucksack> Rucksacks;
    public char Badge
    {
        get { return GetCommonItem(); }
    }
    public int PriorityValue
    {
        get { return GetCommonItem().PriorityValue(); }
    }
    public RucksackGroup(List<Rucksack> rucksacks)
    {
        Rucksacks = rucksacks;
    }
    private char GetCommonItem()
    {
        return Rucksacks[0].AllItems.Where(a => (Rucksacks[1].AllItems.Contains(a)) && (Rucksacks[2].AllItems.Contains(a))).Distinct().First();
    }
    public override string ToString()
    {
        return String.Format("Badge: {0}, Priority {1}", Badge, PriorityValue);
    }
}