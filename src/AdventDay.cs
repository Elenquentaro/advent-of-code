namespace advent_of_code {
    internal abstract class AdventDay {
        public virtual int DayNumber {
            get {
                var typeName = GetType().Name;
                var firstDigit = typeName.First(char.IsDigit);
                return int.Parse(typeName.AsSpan(typeName.IndexOf(firstDigit)));
            }
        }

        public abstract void SolvePart1(string[] inputLines);
        public abstract void SolvePart2(string[] inputLines);
    }
}
