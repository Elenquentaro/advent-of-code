namespace Parsing {
    public interface IParser {
        object? Parse(string input);
    }

    public interface IParser<T> : IParser {
        new T Parse(string input);
    }
}
