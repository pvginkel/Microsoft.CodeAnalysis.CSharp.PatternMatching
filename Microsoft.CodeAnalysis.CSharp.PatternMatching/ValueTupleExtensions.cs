using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public static class ValueTupleExtensions
    {
        public static ValueTuple<T1, T2> WithItem1<T1, T2>(this ValueTuple<T1, T2> self, T1 value)
        {
            return (value, self.Item2);
        }

        public static ValueTuple<T1, T2> WithItem2<T1, T2>(this ValueTuple<T1, T2> self, T2 value)
        {
            return (self.Item1, value);
        }

        public static ValueTuple<T1, T2, T3> WithItem1<T1, T2, T3>(this ValueTuple<T1, T2, T3> self, T1 value)
        {
            return (value, self.Item2, self.Item3);
        }

        public static ValueTuple<T1, T2, T3> WithItem2<T1, T2, T3>(this ValueTuple<T1, T2, T3> self, T2 value)
        {
            return (self.Item1, value, self.Item3);
        }

        public static ValueTuple<T1, T2, T3> WithItem3<T1, T2, T3>(this ValueTuple<T1, T2, T3> self, T3 value)
        {
            return (self.Item1, self.Item2, value);
        }

        public static ValueTuple<T1, T2, T3, T4> WithItem1<T1, T2, T3, T4>(this ValueTuple<T1, T2, T3, T4> self, T1 value)
        {
            return (value, self.Item2, self.Item3, self.Item4);
        }

        public static ValueTuple<T1, T2, T3, T4> WithItem2<T1, T2, T3, T4>(this ValueTuple<T1, T2, T3, T4> self, T2 value)
        {
            return (self.Item1, value, self.Item3, self.Item4);
        }

        public static ValueTuple<T1, T2, T3, T4> WithItem3<T1, T2, T3, T4>(this ValueTuple<T1, T2, T3, T4> self, T3 value)
        {
            return (self.Item1, self.Item2, value, self.Item4);
        }

        public static ValueTuple<T1, T2, T3, T4> WithItem4<T1, T2, T3, T4>(this ValueTuple<T1, T2, T3, T4> self, T4 value)
        {
            return (self.Item1, self.Item2, self.Item3, value);
        }

        public static ValueTuple<T1, T2, T3, T4, T5> WithItem1<T1, T2, T3, T4, T5>(this ValueTuple<T1, T2, T3, T4, T5> self, T1 value)
        {
            return (value, self.Item2, self.Item3, self.Item4, self.Item5);
        }

        public static ValueTuple<T1, T2, T3, T4, T5> WithItem2<T1, T2, T3, T4, T5>(this ValueTuple<T1, T2, T3, T4, T5> self, T2 value)
        {
            return (self.Item1, value, self.Item3, self.Item4, self.Item5);
        }

        public static ValueTuple<T1, T2, T3, T4, T5> WithItem3<T1, T2, T3, T4, T5>(this ValueTuple<T1, T2, T3, T4, T5> self, T3 value)
        {
            return (self.Item1, self.Item2, value, self.Item4, self.Item5);
        }

        public static ValueTuple<T1, T2, T3, T4, T5> WithItem4<T1, T2, T3, T4, T5>(this ValueTuple<T1, T2, T3, T4, T5> self, T4 value)
        {
            return (self.Item1, self.Item2, self.Item3, value, self.Item5);
        }

        public static ValueTuple<T1, T2, T3, T4, T5> WithItem5<T1, T2, T3, T4, T5>(this ValueTuple<T1, T2, T3, T4, T5> self, T5 value)
        {
            return (self.Item1, self.Item2, self.Item3, self.Item4, value);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6> WithItem1<T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> self, T1 value)
        {
            return (value, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6> WithItem2<T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> self, T2 value)
        {
            return (self.Item1, value, self.Item3, self.Item4, self.Item5, self.Item6);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6> WithItem3<T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> self, T3 value)
        {
            return (self.Item1, self.Item2, value, self.Item4, self.Item5, self.Item6);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6> WithItem4<T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> self, T4 value)
        {
            return (self.Item1, self.Item2, self.Item3, value, self.Item5, self.Item6);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6> WithItem5<T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> self, T5 value)
        {
            return (self.Item1, self.Item2, self.Item3, self.Item4, value, self.Item6);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6> WithItem6<T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> self, T6 value)
        {
            return (self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, value);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> WithItem1<T1, T2, T3, T4, T5, T6, T7>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7> self, T1 value)
        {
            return (value, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> WithItem2<T1, T2, T3, T4, T5, T6, T7>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7> self, T2 value)
        {
            return (self.Item1, value, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> WithItem3<T1, T2, T3, T4, T5, T6, T7>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7> self, T3 value)
        {
            return (self.Item1, self.Item2, value, self.Item4, self.Item5, self.Item6, self.Item7);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> WithItem4<T1, T2, T3, T4, T5, T6, T7>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7> self, T4 value)
        {
            return (self.Item1, self.Item2, self.Item3, value, self.Item5, self.Item6, self.Item7);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> WithItem5<T1, T2, T3, T4, T5, T6, T7>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7> self, T5 value)
        {
            return (self.Item1, self.Item2, self.Item3, self.Item4, value, self.Item6, self.Item7);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> WithItem6<T1, T2, T3, T4, T5, T6, T7>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7> self, T6 value)
        {
            return (self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, value, self.Item7);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> WithItem7<T1, T2, T3, T4, T5, T6, T7>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7> self, T7 value)
        {
            return (self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, value);
        }

    }
}
