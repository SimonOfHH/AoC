using System.Text;

namespace AoC_2021;

public class Day_16 : BaseDay
{
    private readonly string _input;

    public Day_16()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        var packet = ParsePacketFromHexadecimal(_input);

        return new($"{packet.VersionSum()}");
    }

    public override ValueTask<string> Solve_2()
    {
        var packet = ParsePacketFromHexadecimal(_input);

        return new($"{packet.Operate()}");
    }

    private string ParseInput() => File.ReadAllText(InputFilePath).Trim();

    public static string HexadecimalToBinary(string str)
    {
        var sb = new StringBuilder();
        foreach (var ch in str)
        {
            var n = Convert.ToInt64($"{ch}", 16);
            var resultStr = Convert.ToString(n, 2);

            var zerosToAdd = 4 - resultStr.Length;
            for (int i = 0; i < zerosToAdd; ++i)
            {
                resultStr = $"0{resultStr}";
            }
            sb.Append(resultStr);
        }

        return sb.ToString();
    }

    public static Packet ParsePacketFromHexadecimal(string hexadecimalInput)
    {
        var binaryInput = HexadecimalToBinary(hexadecimalInput);
        return ParsePacketFromBinary(binaryInput).Packet;
    }

    public static (Packet Packet, int LastPackageIndex) ParsePacketFromBinary(string binaryString)
    {
        var packet = new Packet();

        var span = binaryString.AsSpan();
        packet.Version = Convert.ToInt32(span.Slice(0, 3).ToString(), 2);
        packet.TypeId = Convert.ToInt32(span.Slice(3, 3).ToString(), 2);

        if (packet.TypeId == 4)
        {
            var sb = new StringBuilder();

            var literalNumber = span.Slice(6);
            int index = 0;
            var control = literalNumber[index];
            do
            {
                control = literalNumber[index];
                var digit = literalNumber.Slice(index + 1, 4);
                sb.Append(digit);
                index += 5;
            } while (control == '1');

            packet.Value = Convert.ToInt64(sb.ToString(), 2);

            return (packet, index + 6);
        }
        else // Operator
        {
            int index = -1;
            int accumulatedIndex;

            var lengthTypeId = span[6];
            if (lengthTypeId == '0')
            {
                var totalSubpacketsBitLength = Convert.ToInt32(span.Slice(7, 15).ToString(), 2);
                var subpacketString = span.Slice(22, totalSubpacketsBitLength).ToString();

                accumulatedIndex = 22;
                while (true)
                {
                    (var subPacket, index) = ParsePacketFromBinary(subpacketString);
                    accumulatedIndex += index;
                    totalSubpacketsBitLength -= index;
                    packet.SubPackets.Add(subPacket);

                    if (totalSubpacketsBitLength == 0)
                    {
                        break;
                    }

                    subpacketString = subpacketString.AsSpan().Slice(index, totalSubpacketsBitLength).ToString().ToString();
                }
            }
            else
            {
                var numberOfSubpackets = Convert.ToInt64(span.Slice(7, 11).ToString(), 2);
                var subpacketString = span.Slice(18).ToString();

                accumulatedIndex = 18;
                for (int i = 0; i < numberOfSubpackets; ++i)
                {
                    (var subPacket, index) = ParsePacketFromBinary(subpacketString);
                    accumulatedIndex += index;
                    packet.SubPackets.Add(subPacket);

                    if (i != numberOfSubpackets - 1)
                    {
                        subpacketString = subpacketString.AsSpan().Slice(index).ToString();
                    }
                }
            }

            return (packet, accumulatedIndex);
        }
    }

    public class Packet
    {
        public int Version { get; set; }

        public int TypeId { get; set; }

        public long? Value { get; set; }

        public List<Packet> SubPackets { get; set; } = new List<Packet>();

        public int VersionSum()
        {
            return Version + SubPackets.Sum(p => p.VersionSum());
        }

        public long Operate()
        {
            if (Value is not null)
            {
                return Value.Value;
            }

            return TypeId switch
            {
                0 => SubPackets.Sum(p => p.Operate()),
                1 => SubPackets.Aggregate(1L, (acc, p) => p.Operate() * acc),
                2 => SubPackets.Min(p => p.Operate()),
                3 => SubPackets.Max(p => p.Operate()),
                5 => SubPackets[0].Operate() > SubPackets[1].Operate() ? 1 : 0,
                6 => SubPackets[0].Operate() < SubPackets[1].Operate() ? 1 : 0,
                7 => SubPackets[0].Operate() == SubPackets[1].Operate() ? 1 : 0,
            };
        }
    }
}