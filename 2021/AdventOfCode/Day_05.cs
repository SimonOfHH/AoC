using SheepTools.Model;
using System.Collections.Concurrent;

namespace AoC_2021
{
    public class Day_05 : BaseDay
    {
        private readonly List<(IntPoint Start, IntPoint End)> _input;

        public Day_05()
        {
            _input = ParseInput().ToList();
        }

        public override ValueTask<string> Solve_1()
        {
            var linePoints = new Dictionary<IntPoint, int>();

            foreach (var (Start, End) in _input)
            {
                var start = Start;
                var end = End;
                if (start.X == end.X)
                {
                    if (start.Y > end.Y)
                    {
                        (start, end) = (end, start);
                    }

                    for (int y = 0; y <= (end.Y - start.Y); ++y)
                    {
                        var point = start with { Y = start.Y + y };
                        if (!linePoints.TryAdd(point, 1))
                        {
                            ++linePoints[point];
                        }
                    }
                }
                else if (Start.Y == End.Y)
                {
                    var m = (end.Y - start.Y) / ((double)end.X - start.X);
                    if (start.X > end.X)
                    {
                        (start, end) = (end, start);
                    }

                    for (int x = 0; x <= (end.X - start.X); ++x)
                    {
                        var point = new IntPoint(start.X + x, (int)(start.Y + (m * x)));
                        if (!linePoints.TryAdd(point, 1))
                        {
                            ++linePoints[point];
                        }
                    }
                }
            }

            return new($"{linePoints.Count(pair => pair.Value >= 2)}");
        }

        public override ValueTask<string> Solve_2()
        {
            var linePoints = new Dictionary<IntPoint, int>();

            foreach (var (Start, End) in _input)
            {
                var start = Start;
                var end = End;
                if (start.X == end.X)
                {
                    if (start.Y > end.Y)
                    {
                        (start, end) = (end, start);
                    }

                    for (int y = 0; y <= (end.Y - start.Y); ++y)
                    {
                        var point = start with { Y = start.Y + y };
                        if (!linePoints.TryAdd(point, 1))
                        {
                            ++linePoints[point];
                        }
                    }
                }
                else
                {
                    var m = (end.Y - start.Y) / ((double)end.X - start.X);
                    if (start.Y == end.Y || Math.Abs(m) == 1)   // Vertical or 45º, which guarantees int points
                    {
                        if (start.X > end.X)
                        {
                            (start, end) = (end, start);
                        }

                        for (int x = 0; x <= (end.X - start.X); ++x)
                        {
                            var point = new IntPoint(start.X + x, (int)(start.Y + (m * x)));
                            if (!linePoints.TryAdd(point, 1))
                            {
                                ++linePoints[point];
                            }
                        }
                    }
                }
            }

            return new($"{linePoints.Count(pair => pair.Value >= 2)}");
        }

        private IEnumerable<(IntPoint Start, IntPoint End)> ParseInput()
        {
            var file = new ParsedFile(InputFilePath, new[] { ' ', ',' });

            while (!file.Empty)
            {
                var line = file.NextLine();

                var start = new IntPoint(line.NextElement<int>(), line.NextElement<int>());
                line.NextElement<string>(); // ->
                var end = new IntPoint(line.NextElement<int>(), line.NextElement<int>());

                yield return (start, end);
            }
        }

        public ValueTask<string> Solve_1_Parallel_ForEach()
        {
            var linePoints = new ConcurrentDictionary<IntPoint, int>();

            Parallel.ForEach(_input, pair =>
            {
                var start = pair.Start;
                var end = pair.End;
                if (start.X == end.X)
                {
                    if (start.Y > end.Y)
                    {
                        (start, end) = (end, start);
                    }

                    for (int y = 0; y <= (end.Y - start.Y); ++y)
                    {
                        var point = start with { Y = start.Y + y };
                        linePoints.AddOrUpdate(point, 1, (_, existingValue) => existingValue + 1);
                    }
                }
                else if (pair.Start.Y == pair.End.Y)
                {
                    var m = (end.Y - start.Y) / ((double)end.X - start.X);
                    if (start.X > end.X)
                    {
                        (start, end) = (end, start);
                    }

                    for (int x = 0; x <= (end.X - start.X); ++x)
                    {
                        var point = new IntPoint(start.X + x, (int)(start.Y + (m * x)));
                        linePoints.AddOrUpdate(point, 1, (_, existingValue) => existingValue + 1);
                    }
                }
            });

            return new($"{linePoints.Count(pair => pair.Value >= 2)}");
        }

        public ValueTask<string> Solve_1_Parallel_ForEach_Hardcore()
        {
            var linePoints = new ConcurrentDictionary<IntPoint, int>();

            Parallel.ForEach(_input, pair =>
            {
                var start = pair.Start;
                var end = pair.End;
                if (start.X == end.X)
                {
                    if (start.Y > end.Y)
                    {
                        (start, end) = (end, start);
                    }

                    Parallel.For(0, end.Y - start.Y, y =>
                    {
                        var point = start with { Y = start.Y + y };
                        linePoints.AddOrUpdate(point, 1, (_, existingValue) => existingValue + 1);
                    });
                }
                else if (pair.Start.Y == pair.End.Y)
                {
                    var m = (end.Y - start.Y) / ((double)end.X - start.X);
                    if (start.X > end.X)
                    {
                        (start, end) = (end, start);
                    }

                    Parallel.For(0, end.X - start.X, x =>
                    {
                        var point = new IntPoint(start.X + x, (int)(start.Y + (m * x)));
                        linePoints.AddOrUpdate(point, 1, (_, existingValue) => existingValue + 1);
                    });
                }
            });

            return new($"{linePoints.Count(pair => pair.Value >= 2)}");
        }

        public ValueTask<string> Solve_2_Parallel_ForEach()
        {
            var linePoints = new ConcurrentDictionary<IntPoint, int>();

            Parallel.ForEach(_input, pair =>
            {
                var start = pair.Start;
                var end = pair.End;
                if (start.X == end.X)
                {
                    if (start.Y > end.Y)
                    {
                        (start, end) = (end, start);
                    }

                    for (int y = 0; y <= (end.Y - start.Y); ++y)
                    {
                        var point = start with { Y = start.Y + y };
                        if (!linePoints.TryAdd(point, 1))
                        {
                            ++linePoints[point];
                        }
                    }
                }
                else
                {
                    var m = (end.Y - start.Y) / ((double)end.X - start.X);
                    if (start.Y == end.Y || Math.Abs(m) == 1)   // Vertical or 45º, which guarantees int points
                    {
                        if (start.X > end.X)
                        {
                            (start, end) = (end, start);
                        }

                        for (int x = 0; x <= (end.X - start.X); ++x)
                        {
                            var point = new IntPoint(start.X + x, start.Y + (int)(m * x));
                            if (!linePoints.TryAdd(point, 1))
                            {
                                ++linePoints[point];
                            }
                        }
                    }
                }
            });

            return new($"{linePoints.Count(pair => pair.Value >= 2)}");
        }

        public ValueTask<string> Solve_2_Parallel_ForEach_Hardcore()
        {
            var linePoints = new ConcurrentDictionary<IntPoint, int>();

            Parallel.ForEach(_input, pair =>
            {
                var start = pair.Start;
                var end = pair.End;
                if (start.X == end.X)
                {
                    if (start.Y > end.Y)
                    {
                        (start, end) = (end, start);
                    }

                    Parallel.For(0, end.Y - start.Y, y =>
                    {
                        var point = start with { Y = start.Y + y };
                        if (!linePoints.TryAdd(point, 1))
                        {
                            ++linePoints[point];
                        }
                    });
                }
                else
                {
                    var m = (end.Y - start.Y) / ((double)end.X - start.X);
                    if (start.Y == end.Y || Math.Abs(m) == 1)   // Vertical or 45º, which guarantees int points
                    {
                        if (start.X > end.X)
                        {
                            (start, end) = (end, start);
                        }

                        Parallel.For(0, end.X - start.X, x =>
                        {
                            var point = new IntPoint(start.X + x, start.Y + (int)(m * x));
                            if (!linePoints.TryAdd(point, 1))
                            {
                                ++linePoints[point];
                            }
                        });
                    }
                }
            });

            return new($"{linePoints.Count(pair => pair.Value >= 2)}");
        }
    }
}