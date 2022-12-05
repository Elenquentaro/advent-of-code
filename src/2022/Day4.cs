using Parsing;
using Parsing.Utils;

namespace advent_of_code.y2022 {
    class Day4 : AdventDay {
        const char baseSeparator = ',';

        public override void SolvePart1(string[] inputLines) {
            var totalSum = ParseUtils.IterateParsedPairs(inputLines, Range.DefaultParser, baseSeparator)
                .Where(Contains)
                .Count();
            Console.WriteLine("total sum: {0}", totalSum);
        }

        public override void SolvePart2(string[] inputLines) {
            var totalSum = ParseUtils.IterateParsedPairs(inputLines, Range.DefaultParser, baseSeparator)
                .Where(Overlaps)
                .Count();
            Console.WriteLine("total sum: {0}", totalSum);
        }

        static bool Contains((Range first, Range second) ranges) {
            return ranges.first.Contains(ranges.second) || ranges.second.Contains(ranges.first);
        }

        static bool Overlaps((Range first, Range second) ranges) {
            return ranges.first.Overlaps(ranges.second);
        }

        struct Range {
            public static IParser<Range> DefaultParser = new Parser();

            public int Start;
            public int End;

            public bool Contains(int value) {
                return Start <= value && End >= value;
            }

            public bool Contains(Range other) {
                return Contains(other.Start) && Contains(other.End);
            }

            public bool Overlaps(Range other) {
                return Contains(other.Start) || Contains(other.End);
            }

            class Parser : Parsing.Parsers.GenericParser<Range> {
                public override Range Parse(string input) {
                    var splitted = SplitInput(input);
                    // safety? what safety??
                    return new() { Start = int.Parse(splitted[0]), End = int.Parse(splitted[1]) };
                }

                public override char GetDefaultSeparator() {
                    return '-';
                }
            }
        }
    }
}
