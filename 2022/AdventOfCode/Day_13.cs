using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;

namespace AdventOfCode;

public class Day_13 : BaseDay
{
    private Signal Signal;
    private readonly bool Sample = true;
    public Day_13()
    {
        Signal = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");

    private Signal ParseInput()
    {
        return new Signal(this.GetInput(Sample));
    }
}
class Signal
{
    private List<string[]> RawPairs { get; set; }
    public List<Packet> Packets { get; set; }
    public Signal() { }
    public Signal(string[] input) { ParseSignal(input); }
    public void ParseSignal(string[] input)
    {
        RawPairs = new List<string[]>();
        Packets = new List<Packet>();
        while (input.Any())
        {
            var pair = input.TakeUntil(s => String.IsNullOrEmpty(s)).ToArray();
            RawPairs.Add(pair.Take(2).ToArray());
            input = input.Skip(pair.Length).ToArray();
        }
        RawPairs.ForEach(p => Packets.Add(new Packet(p)));
        var temp = Packets.Skip(1).Take(1).First().Compare();
    }
}
[DebuggerDisplay("{Left}, {Right} (Order: {CorrectOrder})")]
class Packet
{
    private string[] Pair { get; set; }
    public Dictionary<int, PacketNumber> LeftNumbers => Left.PacketNumbers.Select((s, index) => new { s, index }).ToDictionary(x => x.index, x => x.s);
    public PacketPart Left { get; set; }
    public PacketPart Right { get; set; }
    private bool CorrectOrder => Compare();
    public Packet(string[] pair)
    {
        Pair = pair;
        Left = PacketPart.ParseRaw(pair[0]);
        Right = PacketPart.ParseRaw(pair[1]);
    }
    public bool Compare()
    {
        return Compare(Left, Right);
    }
    public static bool Compare(PacketPart left, PacketPart right)
    {
        var leftNumbers = left.Numbers;
        var rightNumbers = right.Numbers;
        for (int l = 0; l < left.Numbers.Count; l++)
        {
            if (rightNumbers.Count - 1 < l)
                return true;
            if (leftNumbers[l] > rightNumbers[l])
                return false;
            if ((left.NumbersDictionary[l].Child != null) && (right.NumbersDictionary[l].Child != null))
                return Compare(left.NumbersDictionary[l].Child, right.NumbersDictionary[l].Child);
        }
        if (rightNumbers.Count < leftNumbers.Count)
            return false;
        return true;
    }
}
[DebuggerDisplay("{Format()}")]
class PacketPart : IPacket
{
    private string RawPart { get; set; }
    public Dictionary<int, PacketNumber> NumbersDictionary => PacketNumbers.Select((s, index) => new { s, index }).ToDictionary(x => x.index, x => x.s);
    public PacketNumber FirstNumber => PacketNumbers.FirstOrDefault();
    public List<PacketNumber> PacketNumbers { get; set; }
    public List<int> Numbers => GetNumbers(false);
    public PacketPart(string s)
    {
        RawPart = s;
        PacketNumbers = new List<PacketNumber>();
    }

    public static PacketPart ParseRaw(string s)
    {
        //Dictionary
        string source = s;
        var matchList = new List<string>();
        while (s.Any())
        {
            var matches = Regex.Matches(s, @"\[((\d*\,*)*\])");
            foreach (var match in matches.Cast<Match>())
            {
                matchList.Add(ValidateEmpty(match.Value));
                if (match.Value == "[]")
                    matchList = matchList.Move(matchList.Count - 1, 1, 0).ToList();
                s = Cleanup(s, match.Value);
            }
        }
        //if (matchList.Contains("[0]"))
        //    matchList.Reverse();
        return ProcessMatchList(matchList);
    }
    private static PacketPart ProcessMatchList(List<string> matchList)
    {

        PacketPart part = null;
        int i = 0;
        if (matchList.Contains("[0]"))
            i = 1;
        foreach (var match in matchList)
        {
            string matchValue = match;
            if (match == "[0]")
                part = new PacketPart(matchValue);
            var partTemp = new PacketPart(matchValue);
            partTemp.PacketNumbers.AddRange(partTemp.Numbers.Select(n => new PacketNumber(n)));
            if (partTemp.FirstNumber != null)
                partTemp.FirstNumber.Child = part ?? null;
            part = partTemp;
        }
        return part;
    }
    private static string ValidateEmpty(string s)
    {
        if (s == "[]")
            s = "[0]";
        return s;
    }
    private static string Cleanup(string s, string matchValue)
    {
        s = s.Replace(matchValue, "");
        s = s.Replace(",]", "]");
        return s;
    }
    public string Format()
    {
        var builder = new StringBuilder();
        builder.Append('[');
        foreach (var number in PacketNumbers)
        {
            if (builder[^1] != '[')
                builder.Append(',');
            builder.Append(number.Number == 0 ? "" : number.Number);
            if (number.Child != null)
                builder.Append(',');
            builder.Append(number.Child?.Format());
            if (number.Child != null)
                builder.Append(']');
        }
        if (builder[^1] != ']')
            builder.Append(']');
        builder.Replace("[,", "[");
        return builder.ToString();
    }
    private List<int> GetNumbers(bool reversed)
    {
        var numbers = new List<int>();
        var matches = Regex.Matches(RawPart, @"(\d)*").Where(m => !String.IsNullOrEmpty(m.Value));
        foreach (var match in matches)
        {
            numbers.Add(int.Parse(match.Value));
        }
        if (reversed)
            numbers.Reverse();
        return numbers;
    }
    public override string ToString()
    {
        return string.Format($"[{Format()}]");
    }
}
[DebuggerDisplay("[{Number}]")]
class PacketNumber : IPacket
{
    public int Number { get; set; }
    public PacketPart Child { get; set; }
    public PacketNumber(int number) => Number = number;
    public override string ToString()
    {
        return string.Format($"[{Number}]");
    }
}
public interface IPacket { }