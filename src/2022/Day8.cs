using advent_of_code.Utils;

namespace advent_of_code.y2022 {
    class Day8 : AdventDay {
        public override void SolvePart1(string[] inputLines) {
            var data = PrepareData(inputLines);

            var count = data.IterateIndicies()
                .Count(i => data.IsVisible(i));

            Console.WriteLine("visible trees: {0}", count);
        }

        public override void SolvePart2(string[] inputLines) {
            var data = PrepareData(inputLines);

            var maxScenicScore = data.IterateIndicies()
                .Max(i => data.GetScenicScore(i));
            Console.WriteLine(maxScenicScore);
        }

        public static Field PrepareData(string[] inputLines) {
            var result = new int[inputLines.Length, inputLines.First().Length];
            for (int lineIdx = 0; lineIdx < inputLines.Length; lineIdx++) {
                var line = inputLines[lineIdx];
                for (int charIdx = 0; charIdx < line.Length; charIdx++) {
                    result[lineIdx, charIdx] = CharToInt(line[charIdx]);
                }
            }
            return new Field(result);
        }

        static int CharToInt(char c) => c - '0';
    }

    public readonly struct SpanBlock {
        readonly static Index2D _d0bofst = (-1, 0);
        readonly static Index2D _d0aofst = (1, 0);
        readonly static Index2D _d1bofst = (0, -1);
        readonly static Index2D _d1aofst = (0, 1);

        public readonly ReadOnlySpan2D<int> d0_bef;
        public readonly ReadOnlySpan2D<int> d0_aft;
        public readonly ReadOnlySpan2D<int> d1_bef;
        public readonly ReadOnlySpan2D<int> d1_aft;

        public SpanBlock(int[,] matrix, Index2D pos) {
            var len0 = matrix.GetLength(0);
            var len1 = matrix.GetLength(1);
            d0_bef = new(matrix, pos + _d0bofst, _d0bofst, pos.d0);
            d0_aft = new(matrix, pos + _d0aofst, _d0aofst, len0 - pos.d0 - 1);
            d1_bef = new(matrix, pos + _d1bofst, _d1bofst, pos.d1);
            d1_aft = new(matrix, pos + _d1aofst, _d1aofst, len1 - pos.d1 - 1);
        }

        public IEnumerable<IReadOnlyList<int>> IterateLines() {
            yield return d0_bef;
            yield return d0_aft;
            yield return d1_bef;
            yield return d1_aft;
        }
    }

    public readonly struct Field {
        readonly int[,] _data;

        public Field(int[,] data) {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public int this[Index2D point] { get => point.GetAt(_data); }
        public int this[int d0, int d1] { get => _data[d0, d1]; }

        public int[,] Data => _data;

        public bool IsEdge(Index2D point) {
            return point.d0 == 0
                || point.d1 == 0
                || point.d0 + 1 == _data.GetLength(0)
                || point.d1 + 1 == _data.GetLength(1);
        }

        public bool IsVisible(Index2D point) {
            if (IsEdge(point)) return true;
            int value = this[point];
            return GetSpanBlock(point)
                .IterateLines()
                .Any(l => l.All(v => v < value));
        }

        public int GetScenicScore(Index2D point) {
            var block = GetSpanBlock(point);
            var value = this[point];

            return GetViewLength(block.d0_bef, value)
                * GetViewLength(block.d0_aft, value)
                * GetViewLength(block.d1_bef, value)
                * GetViewLength(block.d1_aft, value);

            int GetViewLength(ReadOnlySpan2D<int> line, int forValue) {
                if (line.Length == 0) return 0;
                var result = line.Iterate().TakeWhile(v => v < forValue).Count();
                if (result < line.Length) result++;
                return result;
            }
        }

        public SpanBlock GetSpanBlock(Index2D point) {
            return new SpanBlock(_data, point);
        }

        public IEnumerable<Index2D> IterateIndicies() {
            var data = _data;
            for (int i = 0; i < data.GetLength(0); i++) {
                for (int j = 0; j < data.GetLength(1); j++) {
                    yield return (i, j);
                }
            }
        }

        public static IEnumerable<int> InReverse(int[] arr) {
            for (int i = arr.Length - 1; i >= 0; i--) yield return arr[i];
        }
    }
}
