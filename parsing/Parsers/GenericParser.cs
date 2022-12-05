namespace Parsing.Parsers {
    public abstract class GenericParser<T> : IParser<T> {
        public abstract T Parse(string input);

        object? IParser.Parse(string input) {
            return Parse(input);
        }

        public virtual string[] SplitInput(string input) {
            return input.Split(GetDefaultSeparator());
        }

        public virtual char GetDefaultSeparator() {
            // space by default
            return ' ';
        }
    }
}
