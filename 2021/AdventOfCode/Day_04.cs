namespace AoC_2021
{
    public record BingoNumber(int Value)
    {
        public bool IsComplete { get; set; }
    }

    public record BingoBoard(List<BingoNumber> Numbers)
    {
        public const int BoardSize = 5;

        public bool IsComplete { get; private set; }

        public int IncompleteNumbersSum => Numbers.Where(n => !n.IsComplete).Sum(n => n.Value);

        public bool UpdateAndCheckIfComplete(int numberIndex)
        {
            Numbers[numberIndex].IsComplete = true;

            var complete = true;

            var startY = numberIndex % BoardSize;
            for (int y = 0; y < BoardSize; ++y)
            {
                var n = Numbers[startY + (BoardSize * y)];
                if (!n.IsComplete)
                {
                    complete = false;
                    break;
                }
            }

            if (complete)
            {
                IsComplete = true;
                return true;
            }

            complete = true;

            var startX = numberIndex / BoardSize;
            for (int x = 0; x < BoardSize; ++x)
            {
                var n = Numbers[(startX * BoardSize) + x];
                if (!n.IsComplete)
                {
                    complete = false;
                    break;
                }
            }

            if (complete)
            {
                IsComplete = true;
                return true;
            }

            return false;
        }
    }

    public class Day_04 : BaseDay
    {
        private readonly List<BingoBoard> _inputBoards;
        private readonly List<int> _inputNumbers;

        public Day_04()
        {
            (_inputNumbers, _inputBoards) = ParseInput();
        }

        public override ValueTask<string> Solve_1()
        {
            for (int numberIndex = 0; numberIndex < _inputNumbers.Count; ++numberIndex)
            {
                var number = _inputNumbers[numberIndex];
                for (int boardIndex = 0; boardIndex < _inputBoards.Count; ++boardIndex)
                {
                    var board = _inputBoards[boardIndex];

                    var foundNumberIndex = board.Numbers.FindIndex(n => n.Value == number);
                    if (foundNumberIndex != -1 && board.UpdateAndCheckIfComplete(foundNumberIndex))
                    {
                        return new($"{board.IncompleteNumbersSum * number}");
                    }
                }
            }

            throw new SolvingException($"Error solving {nameof(Day_04)} part 1");
        }

        public override ValueTask<string> Solve_2()
        {
            for (int numberIndex = 0; numberIndex < _inputNumbers.Count; ++numberIndex)
            {
                var number = _inputNumbers[numberIndex];
                for (int boardIndex = 0; boardIndex < _inputBoards.Count; ++boardIndex)
                {
                    var board = _inputBoards[boardIndex];
                    if (board.IsComplete)
                    {
                        continue;
                    }

                    var foundNumberIndex = board.Numbers.FindIndex(n => n.Value == number);
                    if (foundNumberIndex != -1
                        && board.UpdateAndCheckIfComplete(foundNumberIndex)
                        && _inputBoards.All(b => b.IsComplete))
                    {
                        return new($"{board.IncompleteNumbersSum * number}");
                    }
                }
            }

            throw new SolvingException($"Error solving {nameof(Day_04)} part 2");
        }

        private (List<int>, List<BingoBoard>) ParseInput()
        {
            var file = new ParsedFile(InputFilePath, existingSeparator: new[] { ' ', ',' });

            var numbers = file.NextLine().ToList<int>();
            var boards = new List<BingoBoard> { new(new()) };

            var y = 0;
            while (!file.Empty)
            {
                if (y == BingoBoard.BoardSize)
                {
                    y = 0;
                    boards.Add(new(new()));
                }

                boards.Last().Numbers.AddRange(file.NextLine().ToList<int>().Select(val => new BingoNumber(val)));
                ++y;
            }

            return (numbers, boards);
        }
    }
}