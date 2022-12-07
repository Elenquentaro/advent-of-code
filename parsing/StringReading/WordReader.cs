using System.Globalization;

namespace Parsing.StringReading {
    public ref struct WordReader {
        static readonly char[] _defaultWordSeparators = { ' ', '\t', '\n', '\r' };

        private readonly ReadOnlySpan<char> _chars;
        private readonly ReadOnlySpan<char> _wordSeparators;

        private int _readPosition;

        public WordReader(string line, int initialPosition, char[]? wordSeparator = null)
            : this(line == null ? default : line.AsSpan(initialPosition), wordSeparator) {
        }

        public WordReader(string line, char[]? wordSeparator = null)
            : this(line == null ? default : line.AsSpan(), wordSeparator) {
        }

        public WordReader(ReadOnlySpan<char> chars, ReadOnlySpan<char> wordSeparator) {
            _chars = chars;
            _wordSeparators = wordSeparator;
            if (_wordSeparators.IsEmpty) _wordSeparators = _defaultWordSeparators;
            // if reading started not from 0 position, use Span.Slice before ctor
            _readPosition = 0;
        }

        public int Position => _readPosition;
        public int Length => _chars.Length;
        public bool ReadCompleted => _readPosition >= _chars.Length;

        public bool TryReadWord(out ReadOnlySpan<char> word) {
            bool hasChars = _chars.Length > _readPosition;
            if (hasChars) {
                int startPosition = _readPosition;
                var slice = _chars[_readPosition..];
                var sepIdx = slice.IndexOfAny(_wordSeparators);
                if (sepIdx > -1) {
                    // in case more than 1 sep symbol
                    while (sepIdx == 0 && slice.Length > 1) {
                        startPosition++;
                        slice = slice[1..];
                        sepIdx = slice.IndexOfAny(_wordSeparators);
                    }

                    if (sepIdx > -1) {
                        slice = slice[..sepIdx];
                    }
                }
                word = slice;
                _readPosition = startPosition + word.Length;
            } else word = default;
            return hasChars;
        }

        public bool TryReadNumber(out int number, NumberStyles style = (NumberStyles)3, IFormatProvider? formatProvider = null) {
            bool result = TryReadWord(out var word);
            if (result) {
                result = int.TryParse(word, style, formatProvider, out number);
            } else number = 0;
            return result;
        }

        public string ReadWord() {
            TryReadWord(out var word);
            return word.ToString();
        }

        // rude way to read word from line
        public static ReadOnlySpan<char> ReadNextWord(string line, ref int readPosition) {
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

        // rude way
        public static int ReadNextNumber(string line, ref int readPosition) {
            int result = 0;
            bool success = false;
            while (!success && readPosition < line.Length) {
                var span = ReadNextWord(line, ref readPosition);
                success = int.TryParse(span, (System.Globalization.NumberStyles)3, null, out result);
            }
            return result;
        }

        // rude way
        public static IEnumerable<int> ReadNumbers(string line) {
            int readPos = 0;
            while (readPos < line.Length) {
                yield return ReadNextNumber(line, ref readPos);
            }
        }
    }
}
