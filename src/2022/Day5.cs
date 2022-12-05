using Stack = System.Collections.Generic.Stack<string>;

namespace advent_of_code.y2022 {
    class Day5 : AdventDay {
        public override void SolvePart1(string[] inputLines) {
            var stacks = SetupStacks(inputLines, out var sepIdx);

            foreach (var move in IterateMoves(inputLines, sepIdx + 1)) {
                move.Apply(stacks, MoveMode.ByOne);
            }

            LogStacksInfo(stacks);
        }

        public override void SolvePart2(string[] inputLines) {
            var stacks = SetupStacks(inputLines, out var sepIdx);

            foreach (var move in IterateMoves(inputLines, sepIdx + 1)) {
                move.Apply(stacks, MoveMode.KeepOrder);
            }

            LogStacksInfo(stacks);

        }

        private static void LogStacksInfo(Stack[] stacks) {
            for (int i = 0; i < stacks.Length; i++) {
                var stack = stacks[i];
                string[] asArray = new string[stack.Count];
                stack.CopyTo(asArray, 0);
                Array.Reverse(asArray);
                Console.WriteLine("{0}", string.Join(" ", asArray));
            }

            Console.WriteLine("----\n{0}", string.Join("", stacks.Select(s => s.Peek().TrimStart('[').TrimEnd(']'))));
        }

        static Stack[] SetupStacks(string[] inputLines, out int separatorLineIdx) {
            int emptyIndex = separatorLineIdx = Array.IndexOf(inputLines, "");
            var stacksMap = ReadNumbers(inputLines[emptyIndex - 1]).ToArray();

            var stacks = new Stack[stacksMap.Length];
            for (int stack = 0; stack < stacks.Length; stack++) {
                stacks[stack] = new Stack(10);
            }

            for (int i = emptyIndex - 2; i >= 0; i--) {
                PlaceToStacks(stacks, inputLines[i]);
            }
            return stacks;
        }

        static IEnumerable<Move> IterateMoves(string[] inputLines, int startIndex) {
            for (int i = startIndex; i < inputLines.Length; i++) {
                yield return Move.DefaultParser.Parse(inputLines[i]);
            }
        }

        static void PlaceToStacks(Stack[] allStacks, string itemRow) {
            var rowContent = ReadRow(itemRow);
            for (int i = 0; i < rowContent.Length; i++) {
                var item = rowContent[i];
                if (!string.IsNullOrWhiteSpace(item)) {
                    var stack = allStacks[i];
                    stack.Push(item);
                }
            }
        }

        static string[] ReadRow(string line) {
            var result = new string[(line.Length + 1) / 4];
            for (int i = 0; i < result.Length; i++) {
                result[i] = line.Substring(i * 4, 3);
            }
            return result;
        }

        #region Extended StringReader
        static ReadOnlySpan<char> ReadNextWord(string line, ref int readPosition) {
            int startPosition = readPosition;
            var emptyIdx = line.IndexOf(' ', startPosition);
            int length = emptyIdx;
            int readOffset = 0;
            if (emptyIdx > -1) {
                readOffset++;
            } else {
                length = line.Length;
            }
            length -= startPosition;
            readPosition = startPosition + length + readOffset;
            return line.AsSpan(startPosition, length);
        }

        static int ReadNextNumber(string line, ref int readPosition) {
            int result = 0;
            bool success = false;
            while (!success && readPosition < line.Length) {
                var span = ReadNextWord(line, ref readPosition);
                success = int.TryParse(span, (System.Globalization.NumberStyles)3, null, out result);
            }
            return result;
        }

        static IEnumerable<int> ReadNumbers(string line) {
            int readPos = 0;
            while (readPos < line.Length) {
                yield return ReadNextNumber(line, ref readPos);
            }
        }
        #endregion Extended StringReader

        static IEnumerable<T> PopRange<T>(Stack<T> stack, int count) {
            for (int i = 0; i < count && stack.TryPop(out var elem); i++) {
                yield return elem;
            }
        }

        static void PushRange<T>(Stack<T> stack, IEnumerable<T> values) {
            foreach (var item in values) stack.Push(item);
        }

        struct Move {
            public readonly static Parsing.IParser<Move> DefaultParser = new Parser();

            public int count;
            public int from;
            public int to;

            public void Apply(Stack[] stacks, MoveMode mode) {
                var sourceStack = stacks[from - 1];
                var destStack = stacks[to - 1];

                destStack.EnsureCapacity(destStack.Count + count);

                if (mode == MoveMode.ByOne)
                    PushRange(destStack, PopRange(sourceStack, count));
                else {
                    var moveBuffer = new Stack(count);
                    PushRange(moveBuffer, PopRange(sourceStack, count));
                    PushRange(destStack, PopRange(moveBuffer, count));
                }
            }

            class Parser : Parsing.Parsers.GenericParser<Move> {
                public override Move Parse(string input) {
                    int readPos = 0;
                    return new Move {
                        count = ReadNextNumber(input, ref readPos),
                        from = ReadNextNumber(input, ref readPos),
                        to = ReadNextNumber(input, ref readPos)
                    };
                }
            }
        }

        enum MoveMode { ByOne, KeepOrder }
    }
}
