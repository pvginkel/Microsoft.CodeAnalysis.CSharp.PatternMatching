using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public struct PatternMatch<TResult>
    {
        public bool Success { get; }
        public TResult Result { get; }

        internal PatternMatch(bool success, TResult result)
        {
            Success = success;
            Result = result;
        }
    }
}
