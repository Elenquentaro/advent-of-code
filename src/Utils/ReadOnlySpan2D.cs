//#define USE_SPAN_INSTEAD_ALLOC
using System.Collections;
using System.Runtime.CompilerServices;

namespace advent_of_code.Utils {
    public readonly struct ReadOnlySpan2D<T> : IReadOnlyList<T> {
        readonly T[,] _values;
        readonly Index2D _start;
        readonly Index2D _moveStep;
        readonly int _length;

        public ReadOnlySpan2D(T[,] values, Index2D start, Index2D moveStep, int length) {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            _start = start;
            _moveStep = moveStep;
            _length = length;
        }

        public T this[int index] {
            get {
                if (index < 0 || index >= _length) throw new ArgumentOutOfRangeException(nameof(index));
                return GetAtUnsafe(index);
            }
        }

        public int Length { get => _length; }
        public int Count { get => _length; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetAtUnsafe(int index) { return _values[_start.d0 + index * _moveStep.d0, _start.d1 + index * _moveStep.d1]; }

        /*public ReadOnlySpan2D<T> Slice(int start) {
            if (start > 0 && start < _length) {
                return new ReadOnlySpan2D<T>(_values, _start + _moveStep * start, _moveStep, _length - start);
            }
            // throw in future
            return this;
        }*/

        public IEnumerable<T> IterateReverse() {
            return IterateValuesReverse(_values, _start, _moveStep, _length);
        }

        public IEnumerable<T> Iterate() {
            return IterateValues(_values, _start, _moveStep, _length);
        }

        public IEnumerator<T> GetEnumerator() {
            return IterateValues(_values, _start, _moveStep, _length)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            return string.Join(",", Iterate());
        }

        public T[] ToArray() {
            var result = new T[_length];
            for (int i = 0; i < _length; i++) {
                result[i] = GetAtUnsafe(i);
            }
            return result;
        }

        public K[] ToArray<K>(Converter<T, K> converter) {
            var result = new K[_length];
            for (int i = 0; i < _length; i++) {
                result[i] = converter(GetAtUnsafe(i));
            }
            return result;
        }

        public static IEnumerable<T> IterateValues(T[,] values, Index2D start, Index2D step, int length) {
            for (int i = 0; i < length; i++) {
                yield return values[start.d0 + i * step.d0, start.d1 + i * step.d1];
            }
        }

        public static IEnumerable<T> IterateValuesReverse(T[,] values, Index2D start, Index2D step, int length) {
            for (int i = length - 1; i >= 0; i--) {
                yield return values[start.d0 + i * step.d0, start.d1 + i * step.d1];
            }
        }

        public static ReadOnlySpan2D<T> FromRow(T[,] values, int row) {
            var length = values.GetLength(1);
            var start = (row, 0);
            var step = (0, 1);
            return new ReadOnlySpan2D<T>(values, start, step, length);
        }

        public static ReadOnlySpan2D<T> FromCol(T[,] values, int col) {
            var length = values.GetLength(0);
            var start = (0, col);
            var step = (1, 0);
            return new ReadOnlySpan2D<T>(values, start, step, length);
        }
    }
}
