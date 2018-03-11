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
    public class CachedPattern
    {
        private readonly SyntaxNode _tree = RoslynUtil.GetSyntax();
        private readonly PatternNode<(int Left, int Right)> _pattern = BuildPattern();

        private static PatternNode<(int Left, int Right)> BuildPattern()
        {
            var builder = new PatternBuilder<(int Left, int Right)>();

            return builder.AssignmentExpression(
                right: builder.BinaryExpression(
                    SyntaxKind.AddExpression,
                    builder.LiteralExpression(
                        action: (result, literal) => result.WithItem1(int.Parse(literal.Token.ValueText))
                    ),
                    builder.LiteralExpression(
                        action: (result, literal) => result.WithItem2(int.Parse(literal.Token.ValueText))
                    )
                )
            );
        }

        [Benchmark]
        public IList<(int Left, int Right)> Benchmark()
        {
            return _pattern.MatchDescendantNodes(_tree);
        }
    }
}
