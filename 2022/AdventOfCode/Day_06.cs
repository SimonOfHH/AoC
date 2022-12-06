namespace AdventOfCode;

public class Day_06 : BaseDay
{
    private readonly bool Sample = true;
    private readonly DataStream dataStream;
    public Day_06()
    {
        dataStream = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new(dataStream.StartOfPacket.ToString());

    public override ValueTask<string> Solve_2() => new(dataStream.StartOfMessage.ToString());

    private DataStream ParseInput()
    {
        return new DataStream(this.GetInput(Sample).First());
    }
}

public class DataStream
{
    public string StreamData { get; }
    public int StartOfPacket => GetStartOfIndicator(4);
    public int StartOfMessage => GetStartOfIndicator(14);
    public DataStream(string s)
    {
        StreamData = s;
    }
    private int GetStartOfIndicator(int length)
    {
        for (int i = 0; i < StreamData.Length; i++)
        {
            if (StreamData.Skip(i).Take(length).Distinct().Count() == length)
                return i + length;
        }
        return 0;
    }
}