using SheepTools.Model;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace AoC_2021
{
    public class Day_08 : BaseDay
    {
        private List<(List<string> SignalPatterns, List<string> Output)> _input;

        ///   0:      1:      2:      3:      4:
        ///  aaaa    ....    aaaa    aaaa    ....
        /// b    c  .    c  .    c  .    c  b    c
        /// b    c  .    c  .    c  .    c  b    c
        ///  ....    ....    dddd    dddd    dddd
        /// e    f  .    f  e    .  .    f  .    f
        /// e    f  .    f  e    .  .    f  .    f
        ///  gggg    ....    gggg    gggg    ....
        ///
        ///   5:      6:      7:      8:      9:
        ///  aaaa    aaaa    aaaa    aaaa    aaaa
        /// b    .  b    .  .    c  b    c  b    c
        /// b    .  b    .  .    c  b    c  b    c
        ///  dddd    dddd    ....    dddd    dddd
        /// .    f  e    f  .    f  e    f  .    f
        /// .    f  e    f  .    f  e    f  .    f
        ///  gggg    gggg    ....    gggg    gggg
        private static readonly IReadOnlyDictionary<short, HashSet<int>> _positionsByDigitDict = new Dictionary<short, HashSet<int>>()
        {
            [1] = new HashSet<int> { 3, 6 },                // Unique length
            [4] = new HashSet<int> { 2, 3, 4, 6, },         // Unique length
            [7] = new HashSet<int> { 1, 3, 6, },            // Unique length
            [2] = new HashSet<int> { 1, 3, 4, 5, 7 },
            [3] = new HashSet<int> { 1, 3, 4, 6, 7 },
            [5] = new HashSet<int> { 1, 2, 4, 6, 7 },
            [6] = new HashSet<int> { 1, 2, 4, 5, 6, 7 },
            [0] = new HashSet<int> { 1, 2, 3, 5, 6, 7 },
            [9] = new HashSet<int> { 1, 2, 3, 4, 6, 7 },
            [8] = new HashSet<int> { 1, 2, 3, 4, 5, 6, 7 } // Unique length
        };

        public Day_08()
        {
            _input = ParseInput().ToList();
        }

        public override ValueTask<string> Solve_1()
        {
            return new(_input
                .SelectMany(pair => pair.Output.Select(output => output.Length))
                .Count(nChars => nChars == 2 || nChars == 3 || nChars == 4 || nChars == 7)
                .ToString());
        }

        public override ValueTask<string> Solve_2() => DeductionPlusBruteForce();

        /// < 150ms
        private ValueTask<string> DeductionPlusBruteForce()
        {
            ImmutableArray<char> constantChars = ImmutableArray.Create(new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' });
            ImmutableArray<int> constantPositions = ImmutableArray.Create(new[] { 1, 2, 3, 4, 5, 6, 7 });

            var finalResult = 0;
            foreach (var (SignalPatterns, Output) in _input)
            {
                Dictionary<char, HashSet<int>> positionsByCharDict = new(constantChars.Select(c =>
                    new KeyValuePair<char, HashSet<int>>(c, new HashSet<int>(constantPositions))));

                List<Dictionary<char, int>> finalPositionsByCharDictList = new() { new() };
                var finalPositionsByCharDict = finalPositionsByCharDictList.Last();

                Dictionary<short, HashSet<char>> knownCharsByDigitDict = GetCharsByDigit(SignalPatterns);

                var charsWeCareAbout = Output.SelectMany(o => o.ToArray()).ToHashSet();
                while (charsWeCareAbout.Any())
                {
                    var previousFinalPositionsByCharDictLenght = finalPositionsByCharDict.Count;

                    // Positions only considered by one char
                    foreach (var position in constantPositions)
                    {
                        var charMatches = positionsByCharDict.Where(pair => pair.Value.Contains(position));
                        if (charMatches.Count() == 1 && !finalPositionsByCharDict.ContainsValue(position))
                        {
                            positionsByCharDict[charMatches.First().Key] = new HashSet<int> { position };
                        }
                    }

                    // Matches by digit
                    foreach (var knownCharsByDigit in knownCharsByDigitDict)
                    {
                        var charsByDigit = knownCharsByDigit.Value.Where(ch => !finalPositionsByCharDict.ContainsKey(ch));
                        var positionsByDigit = _positionsByDigitDict[knownCharsByDigit.Key].Where(pos => !finalPositionsByCharDict.ContainsValue(pos));

                        // If one of the positions is only possible by one of the chars
                        foreach (var position in positionsByDigit)
                        {
                            var charsThatCanHaveThatPosition = charsByDigit.Where(ch => positionsByCharDict[ch].Contains(position));

                            if (charsThatCanHaveThatPosition.Count() == 1)
                            {
                                positionsByCharDict[charsThatCanHaveThatPosition.First()] = new() { position };
                            }
                        }

                        // If one of these chars, cannot have other positions
                        foreach (var ch in charsByDigit)
                        {
                            var n = positionsByCharDict[ch].Where(pos => !positionsByDigit.Contains(pos)).ToList();
                            positionsByCharDict[ch].RemoveWhere(pos => !positionsByDigit.Contains(pos));
                        }

                        // If not one of these chars, cannot have these positions
                        for (int i = 0; i < positionsByCharDict.Count; ++i)
                        {
                            var item = positionsByCharDict.ElementAt(i);

                            if (!charsByDigit.Contains(item.Key))
                            {
                                item.Value.RemoveWhere(pos => positionsByDigit.Contains(pos));
                            }
                        }
                    }

                    // Verify which chars only have one possible solution
                    for (int i = 0; i < positionsByCharDict.Count; ++i)
                    {
                        var item = positionsByCharDict.ElementAt(i);
                        if (item.Value.Count == 1 && !finalPositionsByCharDict.ContainsKey(item.Key))
                        {
                            for (int j = 0; j < positionsByCharDict.Count; ++j)
                            {
                                if (i != j)
                                {
                                    positionsByCharDict.ElementAt(j).Value.Remove(item.Value.First());
                                }
                            }

                            finalPositionsByCharDict[item.Key] = item.Value.First();
                            positionsByCharDict.Remove(item.Key);
                            charsWeCareAbout.Remove(item.Key);
                        }
                    }

                    // No progress deducting
                    if (finalPositionsByCharDict.Count == previousFinalPositionsByCharDictLenght)
                    {
                        var combinations = new List<Dictionary<int, char>>() { new(finalPositionsByCharDict.Select(pair => new KeyValuePair<int, char>(pair.Value, pair.Key))) };
                        foreach (var candidate in positionsByCharDict)
                        {
                            var newCombinations = new List<Dictionary<int, char>>(2 * combinations.Count);
                            foreach (var position in candidate.Value)
                            {
                                foreach (var combination in combinations)
                                {
                                    var newCombination = new Dictionary<int, char>(combination);
                                    if (!newCombination.TryAdd(position, candidate.Key))
                                    {
                                        continue;
                                    }
                                    newCombinations.Add(newCombination);
                                }
                            }

                            combinations = newCombinations;
                        }

                        // Don't really needed, doesn't appear to discard any possibilities
                        // This should be equivalent to all the magic done above, since in the brute force method is required to get the solution
                        var goodCombinations = combinations.Where(combination =>
                        {
                            foreach (var known in knownCharsByDigitDict)
                            {
                                var positions = known.Value.Select(val => combination.Single(c => c.Value == val).Key);
                                if (positions.Any(pos => !_positionsByDigitDict[known.Key].Contains(pos)))
                                {
                                    return false;
                                }
                            }

                            return true;
                        }).ToList();

                        foreach (var goodCombination in goodCombinations)
                        {
                            finalPositionsByCharDictList.Add(new(finalPositionsByCharDictList.First()));

                            foreach (var pair in goodCombination)
                            {
                                finalPositionsByCharDictList.Last()[pair.Value] = pair.Key;
                            }
                        }

                        finalPositionsByCharDictList = finalPositionsByCharDictList.Skip(1).ToList();
                        break;
                    }
                }

                var solutionDictionary = new Dictionary<string, int>(_positionsByDigitDict.Select(pair => new KeyValuePair<string, int>(string.Join("", pair.Value), pair.Key)));

                foreach (var allegedSol in finalPositionsByCharDictList)
                {
                    try
                    {
                        string outputResult = Output.Aggregate("", (acc, n) => acc + solutionDictionary[string.Join("", n.Select(ch => allegedSol[ch]).OrderBy(pos => pos))]);
                        finalResult += int.Parse(outputResult);
                        break;  // This prevents the situation where two combinations are vaid (and equivalent)
                    }
                    catch (Exception) { }
                }
            }

            return new($"{finalResult}");
        }

        /// > 4s
        private ValueTask<string> BruteForce()
        {
            ImmutableArray<char> constantChars = ImmutableArray.Create(new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' });
            ImmutableArray<int> constantPositions = ImmutableArray.Create(new[] { 1, 2, 3, 4, 5, 6, 7 });

            var finalResult = 0;
            foreach (var (SignalPatterns, Output) in _input)
            {
                Dictionary<char, HashSet<int>> positionsByCharDict = new(constantChars.Select(c =>
                    new KeyValuePair<char, HashSet<int>>(c, new HashSet<int>(constantPositions))));

                List<Dictionary<char, int>> finalPositionsByCharDictList = new() { new() };

                var combinations = new List<Dictionary<int, char>>() { new() };
                foreach (var candidate in positionsByCharDict)
                {
                    var newCombinations = new List<Dictionary<int, char>>(2 * combinations.Count);
                    foreach (var position in candidate.Value)
                    {
                        foreach (var combination in combinations)
                        {
                            var newCombination = new Dictionary<int, char>(combination);
                            if (!newCombination.TryAdd(position, candidate.Key))
                            {
                                continue;
                            }
                            newCombinations.Add(newCombination);
                        }
                    }

                    combinations = newCombinations;
                }

                Dictionary<short, HashSet<char>> knownCharsByDigitDict = GetCharsByDigit(SignalPatterns);

                var goodCombinations = combinations.Where(combination =>
                {
                    foreach (var known in knownCharsByDigitDict)
                    {
                        var positions = known.Value.Select(val => combination.Single(c => c.Value == val).Key);
                        if (positions.Any(pos => !_positionsByDigitDict[known.Key].Contains(pos)))
                        {
                            return false;
                        }
                    }

                    return true;
                }).ToList();

                foreach (var combination in goodCombinations)
                {
                    finalPositionsByCharDictList.Add(new(finalPositionsByCharDictList.First()));

                    foreach (var pair in combination)
                    {
                        finalPositionsByCharDictList.Last()[pair.Value] = pair.Key;
                    }
                }

                var solutionDictionary = new Dictionary<string, int>(_positionsByDigitDict.Select(pair => new KeyValuePair<string, int>(string.Join("", pair.Value), pair.Key)));

                foreach (var allegedSol in finalPositionsByCharDictList)
                {
                    try
                    {
                        string outputResult = Output.Aggregate("", (acc, n) => acc + solutionDictionary[string.Join("", n.Select(ch => allegedSol[ch]).OrderBy(pos => pos))]);
                        finalResult += int.Parse(outputResult);
                        break;  // This prevents the situation where two combinations are vaid (and equivalent)
                    }
                    catch (Exception) { }
                }
            }

            return new($"{finalResult}");
        }

        private Dictionary<short, HashSet<char>> GetCharsByDigit(List<string> signalPatters)
        {
            Dictionary<short, HashSet<char>> charByDigit = new();

            foreach (var pattern in signalPatters)
            {
                var candidates = _positionsByDigitDict.GroupBy(n => n.Value.Count)
                    .Where(n => n.Key == pattern.Length)
                    .SelectMany(group => group.Select(pair => pair.Key));

                if (candidates.Count() == 1)
                {
                    charByDigit[candidates.First()] = pattern.ToHashSet();
                }
            }

            return charByDigit;
        }

        private IEnumerable<(List<string> Input, List<string> Output)> ParseInput()
        {
            var file = new ParsedFile(InputFilePath);

            while (!file.Empty)
            {
                var line = file.NextLine();
                var input = new List<string>();
                var output = new List<string>();

                bool isOutput = false;
                while (!line.Empty)
                {
                    var item = new string(line.NextElement<string>().OrderBy(s => s).ToArray());
                    if (item == "|")
                    {
                        isOutput = true;
                    }
                    else if (isOutput)
                    {
                        output.Add(item);
                    }
                    else
                    {
                        input.Add(item);
                    }
                }

                yield return (input, output);
            }
        }
    }
}
