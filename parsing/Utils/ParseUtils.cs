namespace Parsing.Utils {
    public static class ParseUtils {
        public static IEnumerable<(string first, string second)> IteratePairs(string[] inputLines, char splitChar) {
            return inputLines.Select(line => SplitToPair(line, splitChar));
        }

        public static (string, string) SplitToPair(string input, char splitChar) {
            var arr = input.Split(splitChar);
            // TODO: think about not stuffy safety keeping. again
            return (arr[0], arr[1]);
        }

        public static IEnumerable<(T first, T second)> IterateParsedPairs<T>(string[] inputLines, IParser<T> parser, char baseSplitChar = ' ') {
            return IteratePairs(inputLines, baseSplitChar)
                .Select(parser.ParsePair);
        }

        public static (T first, T second) ParsePair<T>(this IParser<T> parser, (string, string) pair) {
            return (parser.Parse(pair.Item1), parser.Parse(pair.Item2));
        }
    }
}
