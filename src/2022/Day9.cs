using System.Diagnostics;
using Parsing.StringReading;

namespace advent_of_code.y2022 {
    class Day9 : AdventDay {
        public override void SolvePart1(string[] inputLines) {
            var moveInfo = new RopeMemory(2);
            _ = inputLines.Select(ReadMove)
                .Aggregate(moveInfo, (info, move) => info.ApplyMove(move));

            // can be also visualized to console but file is more convinient
            Visualize(moveInfo, "output_pt1.txt");
            Console.WriteLine("visited {0} uniques\n", moveInfo.visited.Count);
        }

        public override void SolvePart2(string[] inputLines) {
            var moveInfo = new RopeMemory(10);
            _ = inputLines.Select(ReadMove)
                .Aggregate(moveInfo, (info, move) => info.ApplyMove(move));

            Visualize(moveInfo, "output_pt2.txt");
            Console.WriteLine("visited {0} uniques", moveInfo.visited.Count);
        }

        static Move ReadMove(string line) {
            var readPos = 0;
            var dir = Enum.Parse<Direction>(WordReader.ReadNextWord(line, ref readPos));
            var len = WordReader.ReadNextNumber(line, ref readPos);
            return new(dir, len);
        }

        class RopeMemory {
            public Point initial;
            public Point[] body;
            public Point Tail => body[^1];

            public HashSet<Point> visited = new(256);

            public RopeMemory(int length) {
                if (length < 2) length = 2;
                body = new Point[length];
                visited.Add(Tail);
            }

            public RopeMemory ApplyMove(Move move) {
                for (int i = 0; i < move.len; i++) {
                    body[0] += move.dir;
                    ActualizeBody();
                }
                return this;
            }

            void ActualizeBody() {
                var prevTail = Tail;
                for (int i = 1; i < body.Length; i++) {
                    var prevPart = body[i - 1];
                    var dirBetween = Point.GetDirection(body[i], prevPart);
                    if (DirIsTooLong(dirBetween)) {
                        body[i] += dirBetween.Normalize();
                    }
                }
                if (!Tail.Equals(prevTail)) {
                    visited.Add(Tail);
                }
            }

            static bool DirIsTooLong(Point dir) {
                return !dir.IsInTouch;
            }
        }

        struct Move {
            public Direction dir;
            public int len;

            public Move(Direction dir, int len) {
                this.dir = dir;
                this.len = len;
            }

            public override string ToString() {
                return $"{dir} {len}";
            }
        }

        struct Rect {
            // bottom left
            public Point origin;
            public Point size;

            public Point Relate(Point absPoint) {
                return absPoint - origin;
            }

            public Rect GrowRelative(Point relPoint) {
                if (relPoint.x < 0) {
                    origin.x += relPoint.x;
                    size.x -= relPoint.x;
                } else if (relPoint.x > size.x) size.x = relPoint.x;

                if (relPoint.y < 0) {
                    origin.y += relPoint.y;
                    size.y -= relPoint.y;
                } else if (relPoint.y > size.y) size.y = relPoint.y;

                return this;
            }

            public Rect Grow(Point absPoint) {
                var relPoint = Relate(absPoint);
                return GrowRelative(relPoint);
            }

            public static Rect FromPoints(IEnumerable<Point> points) {
                Rect rect = default;

                return points.Aggregate(rect, (r, p) => r.Grow(p));
            }
        }

        static void Visualize(RopeMemory memory, string path) {
            var assotiated = (from v in memory.visited select (v, '#'))
                .Concat(memory.body.Select((b, i) => (b, i == 0 ? 'H' : (char)('0' + i))))
                .Append((memory.initial, 's'));

            var allPointsInfo = (from a in assotiated
                                 group a by a.Item1.y into g
                                 let gr = from b in g
                                          orderby b.Item2 descending
                                          select b
                                 select g)
                         .ToDictionary(g => g.Key);

            var rect = Rect.FromPoints(allPointsInfo.SelectMany(s => s.Value).Select(s => s.Item1));

            var buffer = new char[rect.size.x + 1];

            using var file = new StreamWriter(path);
            for (int altitude = 0; altitude <= rect.size.y; altitude++) {
                var line = new Span<char>(buffer);
                line.Fill('.');
                if (allPointsInfo.TryGetValue(altitude + rect.origin.y, out var pointsInfo)) {
                    foreach (var item in pointsInfo) {
                        line[rect.Relate(item.Item1).x] = item.Item2;
                    }
                }
                file.WriteLine((ReadOnlySpan<char>)line);
            }
        }
    }

    internal enum Direction { U, D, L, R }

    internal struct Point : IEquatable<Point> {
        public readonly static Point U = new(0, 1);
        public readonly static Point D = new(0, -1);
        public readonly static Point L = new(-1, 0);
        public readonly static Point R = new(1, 0);

        public int x;
        public int y;

        public Point(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public bool IsZero => x == 0 && y == 0;
        public bool IsInTouch => Math.Abs(x) <= 1 && Math.Abs(y) <= 1;

        public bool Equals(Point other) {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object? obj) {
            return obj is Point point && Equals(point);
        }

        public override int GetHashCode() {
            return x | (y << 16);
        }

        public override string ToString() {
            return string.Format("[{0},{1}]", x, y);
        }

        public static Point GetDirection(Point from, Point to) {
            var x = to.x - from.x;
            var y = to.y - from.y;

            return new(x, y);
        }

        public Point Normalize() {
            return new(Normalize(x), Normalize(y));
        }

        public static int Normalize(int num) {
            return num == 0 ? 0 : num > 0 ? 1 : -1;
        }

        public static Point operator +(Point a, Point b) {
            return new(a.x + b.x, a.y + b.y);
        }

        public static Point operator -(Point a, Point b) {
            return new(a.x - b.x, a.y - b.y);
        }

        public static Point operator *(Point a, int b) {
            return new(a.x * b, a.y * b);
        }

        public static Point operator *(Point a, Point b) {
            return new(a.x * b.x, a.y * b.y);
        }

        public static Point operator /(Point a, Point b) {
            return new(a.x / b.x, a.y / b.y);
        }

        public static implicit operator Point(Direction dir) {
            return dir switch {
                Direction.U => U,
                Direction.D => D,
                Direction.L => L,
                Direction.R => R,
                _ => default
            };
        }
    }
}
