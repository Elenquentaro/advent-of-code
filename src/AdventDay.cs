namespace advent_of_code {
    internal abstract class AdventDay {
        public virtual int DayNumber { get => 1; }
        public abstract void SolvePart1(string[] inputLines);
        public abstract void SolvePart2(string[] inputLines);
    }
}
