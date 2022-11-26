using SheepTools;

namespace AoC_2021
{
    public class Day_07 : BaseDay
    {
        private readonly List<int> _input;

        public Day_07()
        {
            _input = ParseInput();
        }

        public override ValueTask<string> Solve_1() => Solve_1_Mean();

        public override ValueTask<string> Solve_2() => Solve_2_TriangularNumbers();

        private List<int> ParseInput()
        {
            var file = new ParsedFile(InputFilePath, new[] { ',' });

            return file.ToList<int>();
        }

        /// <summary>
        /// 2-3ms
        /// </summary>
        /// <returns></returns>
        private ValueTask<string> Solve_1_Mean()
        {
            _input.Sort();
            var meanPos = _input.Count / 2.0;
            var candidate1 = _input[(int)Math.Floor(meanPos)];
            var candidate2 = _input[(int)Math.Ceiling(meanPos)];

            int fuel1 = 0, fuel2 = 0;

            foreach (var crabPosition in _input)
            {
                fuel1 += Math.Abs(crabPosition - candidate1);
                fuel2 += Math.Abs(crabPosition - candidate2);
            }

            return new($"{(fuel1 < fuel2 ? fuel1 : fuel2)}");
        }

        /// <summary>
        /// 6 ms
        /// </summary>
        /// <returns></returns>
        private ValueTask<string> Solve_1_Iterating()
        {
            var range = RangeHelpers.GenerateRange(_input.Min(), _input.Max());

            var fuel = new int[range.Count()];

            foreach (var alignPosition in range)
            {
                foreach (var crabPosition in _input)
                {
                    fuel[alignPosition] += Math.Abs(crabPosition - alignPosition);
                }
            }

            return new($"{fuel.Min()}");
        }

        /// <summary>
        /// 6ms, ~part 1 https://en.wikipedia.org/wiki/Triangular_number
        /// </summary>
        /// <returns></returns>
        private ValueTask<string> Solve_2_TriangularNumbers()
        {
            var range = RangeHelpers.GenerateRange(_input.Min(), _input.Max());
            var fuel = new int[range.Count()];

            foreach (var alignPosition in range)
            {
                foreach (var crabPosition in _input)
                {
                    var dist = Math.Abs(crabPosition - alignPosition);
                    fuel[alignPosition] += dist * (dist + 1) / 2;
                }
            }

            return new($"{fuel.Min()}");
        }

        /// <summary>
        /// 2.5s
        /// </summary>
        /// <returns></returns>
        private ValueTask<string> Solve_2_Iterating()
        {
            var range = RangeHelpers.GenerateRange(_input.Min(), _input.Max());
            var fuel = new int[range.Count()];

            foreach (var alignPosition in range)
            {
                foreach (var crabPosition in _input)
                {
                    for (int i = 1; i <= Math.Abs(crabPosition - alignPosition); ++i)
                    {
                        fuel[alignPosition] += i;
                    }
                }
            }

            return new($"{fuel.Min()}");
        }
    }
}