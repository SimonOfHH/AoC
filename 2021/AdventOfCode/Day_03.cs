using System.Text;

namespace AoC_2021
{
    public class Day_03 : BaseDay
    {
        private readonly string[] _input;

        public Day_03()
        {
            _input = ParseInput();
        }

        /// <summary>
        /// ~11011 becomes 11...1100100, so unfortunately epsilon isn't just ~gamma
        /// </summary>
        /// <returns></returns>
        public override ValueTask<string> Solve_1()
        {
            var gammaSb = new StringBuilder(_input[0].Length);
            var epsilonSb = new StringBuilder(_input[0].Length);

            for (int i = 0; i < _input[0].Length; ++i)
            {
                var slice = _input.Select(number => number[i] - '0');
                if (slice.Sum() > _input.Length / 2)
                {
                    gammaSb.Append('0');
                    epsilonSb.Append('1');
                }
                else
                {
                    gammaSb.Append('1');
                    epsilonSb.Append('0');
                }
            }

            var gamma = Convert.ToInt32(gammaSb.ToString(), 2);
            var epsilon = Convert.ToInt32(epsilonSb.ToString(), 2);

            return new($"{gamma * epsilon}");
        }

        public override ValueTask<string> Solve_2()
        {
            var oxygenGeneratorRating = FindRating(_input, isOxygenGeneratorRating: true);
            var co2ScrubberRating = FindRating(_input, isOxygenGeneratorRating: false);

            return new($"{oxygenGeneratorRating * co2ScrubberRating}");
        }

        private string[] ParseInput()
        {
            return File.ReadAllLines(InputFilePath);
        }

        public static int FindRating(string[] input, bool isOxygenGeneratorRating, int bitPosition = 0)
        {
            if (input.Length == 1)
            {
                return Convert.ToInt32(input[0], 2);
            }

            var slice = input.Select(number => number[bitPosition] - '0');

            var mostCommonChar = 2 * slice.Sum() >= input.Length
                ? isOxygenGeneratorRating ? '1' : '0'
                : isOxygenGeneratorRating ? '0' : '1';

            return FindRating(input.Where(number => number[bitPosition] == mostCommonChar).ToArray(), isOxygenGeneratorRating, bitPosition + 1);
        }
    }
}