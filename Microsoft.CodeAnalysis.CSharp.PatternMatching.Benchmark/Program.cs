using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var re = args.Length == 0 ? null : new Regex(args[0], RegexOptions.IgnoreCase);

            var benchmarks = typeof(Program).Assembly
                .GetTypes()
                .Where(p =>
                    !p.IsAbstract &&
                    p.GetMethods().Any(p1 => p1.GetCustomAttributes(typeof(BenchmarkAttribute), true).Length > 0) &&
                    (re == null || re.IsMatch(p.FullName)))
                .OrderBy(p => p.FullName)
                .ToArray();

            new BenchmarkSwitcher(benchmarks).RunAllJoined();
        }
    }
}
