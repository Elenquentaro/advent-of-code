namespace advent_of_code.y2022 {
    class Day6 : AdventDay {
        public override void SolvePart1(string[] inputLines) {
            Console.WriteLine(IndexOfUnique(inputLines.First(), 4));
        }

        public override void SolvePart2(string[] inputLines) {
            Console.WriteLine(IndexOfUnique(inputLines.First(), 14));
        }

        public static int IndexOfUnique(string input, int count) {
            var reader = new StringReader(input);
            int symb;
            int pos = 0;
            var queue = new Queue<int>();
            do {
                symb = reader.Read();
                pos++;
                while (queue.Contains(symb)) {
                    queue.Dequeue();
                }
                queue.Enqueue(symb);
            } while (symb != -1 && queue.Count < count);
            return pos;
        }
    }
}
