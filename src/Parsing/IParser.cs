using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code.Parsing {
    public interface IParser {
        object Parse(string input);
    }

    public interface IParser<T> : IParser {
        new T Parse(string input);
    }
}
