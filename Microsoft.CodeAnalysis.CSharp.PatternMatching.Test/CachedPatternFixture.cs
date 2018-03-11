using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching.Test
{
    [TestFixture]
    public class CachedPatternFixture
    {
        [Test]
        public void CachedPattern()
        {
            var syntaxTree = GetSimpleSyntaxTree();
            var expressionStatement = syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<ExpressionStatementSyntax>()
                .Single();

            var builder = new PatternBuilder<(int Left, int Right)>();

            var pattern = builder.AssignmentExpression(
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

            var match = pattern.IsMatch(expressionStatement.Expression);

            Assert.IsTrue(match.Success);
            Assert.AreEqual((2, 3), match.Result);
        }

        private static SyntaxTree GetSimpleSyntaxTree()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
}
            ");
            return syntaxTree;
        }
    }
}
