namespace advent_of_code.y2022 {
    class Day1 : AdventDay {
        public override void SolvePart1(string[] inputLines) {
            // find max total sum
            int maxTotalSum = 0;
            foreach (var bunch in IterateInventories(inputLines)) {
                if (bunch.totalSum > maxTotalSum)
                    maxTotalSum = bunch.totalSum;
                LogBunchInfo(bunch);
            }
            Console.WriteLine("max total sum: {0}\n", maxTotalSum);
        }

        public override void SolvePart2(string[] inputLines) {
            const int count = 3;

            // just iterate all stuff
            var list = IterateInventories(inputLines).ToList();

            list.Sort((b1, b2) => b2.totalSum.CompareTo(b1.totalSum));

            var resultBunches = list.Take(count);

            Console.WriteLine("top {0} bearers:", count);
            foreach (var bunch in resultBunches) {
                LogBunchInfo(bunch);
            }

            var totalSum = resultBunches.Sum(b => b.totalSum);

            Console.WriteLine("top {0} bearers total sum: {1}", count, totalSum);
        }

        public static void LogBunchInfo(BunchInfo bunchInfo) {
            Console.WriteLine();
            Console.WriteLine(bunchInfo.ToString());
            Console.WriteLine();
        }

        public static IEnumerable<BunchInfo> IterateInventories(string[] inputLines) {
            int readPosition = 0;
            while (readPosition < inputLines.Length) {
                int startLine = readPosition;

                var bunch = ReadNextBunch(inputLines, ref readPosition);

                int endLine = readPosition;

                var numbers = LinesToNumbers(bunch);
                yield return new(numbers, startLine, endLine);

                readPosition++;
            }
        }

        public static int[] LinesToNumbers(Span<string> lines) {
            var result = new int[lines.Length];
            for (int i = 0; i < result.Length; i++) {
                var line = lines[i];
                result[i] = int.Parse(line);
            }
            return result;
        }

        public static Span<string> ReadNextBunch(string[] inputLines, ref int readPosition) {
            int startLine = readPosition;
            int endLine = -1;
            for (int lineIdx = startLine; endLine < 0 && lineIdx < inputLines.Length; lineIdx++) {
                if (!IsNumber(inputLines[lineIdx])) {
                    endLine = lineIdx;
                }
            }

            readPosition = endLine >= 0 ? endLine : inputLines.Length;
            int length = readPosition - startLine;

            return new Span<string>(inputLines, startLine, length);
        }

        // in current task conditions (and input content) any way is acceptable
        public static bool IsNumber(string line) {
            return line.Any(char.IsDigit); // why not
            return int.TryParse(line, out _); // most reliable way
            return !string.IsNullOrWhiteSpace(line); // straight way
        }

        public readonly struct BunchInfo {
            public readonly int[] numbers;
            public readonly int startLine;
            public readonly int endLine;

            // cached for pErFoRmAnCe
            public readonly int totalSum;

            public BunchInfo(int[] numbers, int startLine, int endLine) {
                this.numbers = numbers;
                this.startLine = startLine;
                this.endLine = endLine;
                totalSum = numbers.Sum();
            }

            public override string ToString() {
                return string.Format("lines {0} -> {1}, sum {2}\n-----\n{3}",
                                     startLine + 1,
                                     endLine,
                                     totalSum,
                                     string.Join("\n", numbers));
            }
        }
    }
}
