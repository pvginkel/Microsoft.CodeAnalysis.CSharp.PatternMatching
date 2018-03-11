using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching.Benchmark
{
    public class RoslynNative
    {
        private readonly SyntaxNode _tree = RoslynUtil.GetSyntax();

        [Benchmark]
        public List<(int Left, int Right)> Benchmark()
        {
            var matches = new List<(int Left, int Right)>();

            foreach (var assignment in _tree.DescendantNodes().OfType<AssignmentExpressionSyntax>())
            {
                if (
                    assignment.Right is BinaryExpressionSyntax binary &&
                    binary.IsKind(SyntaxKind.AddExpression) &&
                    binary.Left is LiteralExpressionSyntax left &&
                    binary.Right is LiteralExpressionSyntax right
                )
                    matches.Add((int.Parse(left.Token.ValueText), int.Parse(right.Token.ValueText)));
            }

            return matches;
        }
    }
}
