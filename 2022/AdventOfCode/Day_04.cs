namespace AdventOfCode;

public class Day_04 : BaseDay
{
    public bool Sample = false;
    public List<SectionPair> sectionPairs;
    public Day_04()
    {
        sectionPairs = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new(sectionPairs.Count(x => x.SectionContainendFullyInOtherSection()).ToString());

    public override ValueTask<string> Solve_2() => new(sectionPairs.Count(x => x.SectionContainedPartlyInOtherSection()).ToString());

    private List<SectionPair> ParseInput()
    {
        var pairs = new List<SectionPair>();
        foreach (var line in this.GetInput(Sample).Where(x => !String.IsNullOrEmpty(x)))
        {
            pairs.Add(new SectionPair(line));
        }
        return pairs;
    }
}
public class Section
{
    public List<int> SectionElements;
    public Section(string s)
    {
        SectionElements = new List<int>();
        for (int i = int.Parse(s.Split("-")[0]); i <= int.Parse(s.Split("-")[1]); i++)
        {
            SectionElements.Add(i);
        }
    }
    public override string ToString()
    {
        return String.Join(" ", SectionElements.ToArray());
    }
}
public class SectionPair
{
    public Section SectionA;
    public Section SectionB;
    public SectionPair(string s)
    {
        SectionA = new Section(s.Split(",")[0]);
        SectionB = new Section(s.Split(",")[1]);
    }
    public bool SectionContainendFullyInOtherSection()
    {
        if ((SectionA.SectionElements.First() >= SectionB.SectionElements.First()) && (SectionA.SectionElements.Last() <= SectionB.SectionElements.Last()))
            return true;
        if ((SectionB.SectionElements.First() >= SectionA.SectionElements.First()) && (SectionB.SectionElements.Last() <= SectionA.SectionElements.Last()))
            return true;
        return false;
    }
    public bool SectionContainedPartlyInOtherSection()
    {
        return SectionA.SectionElements.Intersect(SectionB.SectionElements).Any();
    }
}