namespace advent_of_code.y2022 {
    class Day3 : AdventDay {
        public override void SolvePart1(string[] inputLines) {
            int total = 0;
            for (int i = 0; i < inputLines.Length; i++) {
                var line = inputLines[i];
                var (item1, item2) = SplitInventory(line);
                var duplicateChars = item1.Where(item2.Contains).Distinct();
                var prioritySum = duplicateChars.Sum(GetPriority);
                total += prioritySum;
                Console.WriteLine("duplicates in ({0}) + ({1}) -> [{2}]  (sum {3})  (input: {4})", item1, item2, string.Join(",", duplicateChars), prioritySum, line);
            }
            Console.WriteLine("total sum: {0}", total);
        }

        public override void SolvePart2(string[] inputLines) {
            int total = 0;
            for (int i = 0; i < inputLines.Length; i += 3) {
                var (first, second, third) = (inputLines[i], inputLines[i + 1], inputLines[i + 2]);
                var common = first.Where(second.Contains).Where(third.Contains).Distinct();

                total += common.Sum(GetPriority);
                Console.WriteLine("{0} - {1} - {2} : {3}", first, second, third, string.Join(",", common));
            }
            Console.WriteLine("total sum: {0}", total);
        }

        static (string, string) SplitInventory(string inputLine) {
            return (inputLine.Substring(0, inputLine.Length / 2),
                inputLine.Substring(inputLine.Length / 2));
        }

        static int GetPriority(char product) {
            int offset = char.IsUpper(product) ? (27 - 'A') : (1 - 'a');
            return product + offset;
        }
    }
}
