using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code.Parsing.Parsers {
    public abstract class GenericParser<T> : IParser<T> {
        public abstract T Parse(string input);

        object IParser.Parse(string input) {
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
