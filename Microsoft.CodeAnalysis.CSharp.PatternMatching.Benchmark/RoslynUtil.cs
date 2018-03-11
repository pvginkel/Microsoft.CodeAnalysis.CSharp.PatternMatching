using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching.Benchmark
{
    internal static class RoslynUtil
    {
        public static SyntaxNode GetSyntax()
        {
            return CSharpSyntaxTree.ParseText(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Program
{
    public static void Main(string[] args)
    {
        int a;
        a = 2 + 3;
    }

    public static int Method2()
    {
        int b;
        b = 4 + 5;
        return b;
    }
}
            ").GetRoot();
        }
    }
}
