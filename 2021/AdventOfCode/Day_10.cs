using System.Collections.Generic;

namespace AoC_2021;

public class Day_10 : BaseDay
{
    private static readonly IReadOnlyDictionary<char, char> OpeningChar = new Dictionary<char, char>
    {
        [')'] = '(',
        [']'] = '[',
        ['}'] = '{',
        ['>'] = '<'
    };

    private static readonly IReadOnlyDictionary<char, char> ClosingChar = new Dictionary<char, char>
    {
        ['('] = ')',
        ['['] = ']',
        ['{'] = '}',
        ['<'] = '>'
    };

    private static readonly IReadOnlyDictionary<char, int> ErrorTable = new Dictionary<char, int>
    {
        [')'] = 3,
        [']'] = 57,
        ['}'] = 1197,
        ['>'] = 25137
    };

    private static readonly IReadOnlyDictionary<char, int> AutoCompleteTable = new Dictionary<char, int>
    {
        ['('] = 1,
        ['['] = 2,
        ['{'] = 3,
        ['<'] = 4
    };

    private readonly List<List<char>> _input;

    public Day_10()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        var result = 0L;

        foreach (var input in _input)
        {
            bool corrupted = false;
            var symbolStack = new Stack<char>();

            foreach (var ch in input)
            {

                switch (ch)
                {
                    case '(':
                    case '[':
                    case '<':
                    case '{':
                        symbolStack.Push(ch);
                        break;
                    case ')':
                    case ']':
                    case '>':
                    case '}':
                        var openingChar = symbolStack.Pop();
                        if (openingChar != OpeningChar[ch])
                        {
                            result += ErrorTable[ch];
                            corrupted = true;
                        }
                        break;
                }

                if (corrupted)
                {
                    break;
                }
            }
        }

        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var scoreList = new List<long>();

        foreach (var input in _input)
        {
            bool corrupted = false;
            var symbolStack = new Stack<char>();

            foreach (var ch in input)
            {

                switch (ch)
                {
                    case '(':
                    case '[':
                    case '<':
                    case '{':
                        symbolStack.Push(ch);
                        break;
                    case ')':
                    case ']':
                    case '>':
                    case '}':
                        var openingChar = symbolStack.Pop();
                        if (openingChar != OpeningChar[ch])
                        {
                            corrupted = true;
                        }
                        break;
                }

                if (corrupted)
                {
                    break;
                }
            }

            if (!corrupted)
            {
                var score = 0L;
                while (symbolStack.TryPop(out char openingChar))
                {
                    score *= 5;
                    score += AutoCompleteTable[openingChar];
                }

                scoreList.Add(score);
            }
        }

        scoreList.Sort();
        return new($"{scoreList[scoreList.Count / 2]}");
    }

    private List<List<char>> ParseInput()
    {
        return File.ReadAllLines(InputFilePath).Select(line => line.ToList()).ToList();
    }
}