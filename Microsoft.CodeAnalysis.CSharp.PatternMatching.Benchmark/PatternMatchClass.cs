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
    public class PatternMatchClass
    {
        private readonly SyntaxNode _tree = RoslynUtil.GetSyntax();
        private readonly Matcher _matcher = new Matcher();

        [Benchmark]
        public List<(int Left, int Right)> Benchmark()
        {
            var matches = new List<(int Left, int Right)>();

            foreach (var assignment in _tree.DescendantNodes().OfType<AssignmentExpressionSyntax>())
            {
                var result = _matcher.IsMatch(assignment);
                if (result.HasValue)
                    matches.Add(result.Value);
            }

            return matches;
        }

        private class Matcher
        {
            private int _left;
            private int _right;
            private readonly PatternNode _pattern;

            public Matcher()
            {
                _pattern = P.AssignmentExpression(
                    right: P.BinaryExpression(
                        SyntaxKind.AddExpression,
                        P.LiteralExpression(
                            action: p => _left = int.Parse(p.Token.ValueText)
                        ),
                        P.LiteralExpression(
                            action: p => _right = int.Parse(p.Token.ValueText)
                        )
                    )
                );
            }

            public (int Left, int Right)? IsMatch(AssignmentExpressionSyntax assignment)
            {
                if (_pattern.IsMatch(assignment))
                    return (_left, _right);

                return null;
            }
        }
    }
}
