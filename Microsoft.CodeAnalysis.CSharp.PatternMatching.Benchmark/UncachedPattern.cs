using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using P = Microsoft.CodeAnalysis.CSharp.PatternMatching.Pattern;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching.Benchmark
{
    public class UncachedPattern
    {
        private readonly SyntaxNode _tree = RoslynUtil.GetSyntax();

        [Benchmark]
        public List<(int Left, int Right)> Benchmark()
        {
            var matches = new List<(int Left, int Right)>();

            foreach (var assignment in _tree.DescendantNodes().OfType<AssignmentExpressionSyntax>())
            {
                int left = 0;
                int right = 0;

                if (
                    P.AssignmentExpression(
                        right: P.BinaryExpression(
                            SyntaxKind.AddExpression,
                            P.LiteralExpression(
                                action: p => left = int.Parse(p.Token.ValueText)
                            ),
                            P.LiteralExpression(
                                action: p => right = int.Parse(p.Token.ValueText)
                            )
                        )
                    ).IsMatch(assignment)
                )
                    matches.Add((left, right));
            }

            return matches;
        }
    }
}
