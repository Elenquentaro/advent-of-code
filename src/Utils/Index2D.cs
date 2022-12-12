using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code.Utils {
    public struct Index2D {
        public int d0;
        public int d1;

        public Index2D(int d0, int d1) {
            this.d0 = d0;
            this.d1 = d1;
        }

        public Index2D d0Only => new(d0, 0);
        public Index2D d1Only => new(0, d1);

        public Index2D Validate0() {
            if (d0 < 0) d0 = 0;
            if (d1 < 0) d1 = 0;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetAt<T>(T[,] arr) {
            return arr[d0, d1];
        }

        public int DistanceTo(Index2D other) {
            return Math.Abs(d0 - other.d0) + Math.Abs(d1 - other.d1);
        }

        public override string ToString() {
            return $"({d1},{d0})";
        }

        public static implicit operator Index2D((int, int) tuple) {
            return new Index2D(tuple.Item1, tuple.Item2);
        }

        public static Index2D operator +(Index2D left, Index2D right) {
            return new Index2D(left.d0 + right.d0, left.d1 + right.d1);
        }

        public static Index2D operator +(Index2D left, int value) {
            return new Index2D(left.d0 + value, left.d1 + value);
        }

        public static Index2D operator -(Index2D left, Index2D right) {
            var result = new Index2D(left.d0 - right.d0, left.d1 - right.d1);
            return result.Validate0();
        }

        public static Index2D operator -(Index2D left, int value) {
            var result = new Index2D(left.d0 - value, left.d1 - value);
            return result.Validate0();
        }

        public static Index2D operator *(Index2D left, int value) {
            return new Index2D(left.d0 * value, left.d1 * value);
        }
    }
}
