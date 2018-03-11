using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using P = Microsoft.CodeAnalysis.CSharp.PatternMatching.Pattern;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching.Test
{
    [TestFixture]
    public class SimpleFixture
    {
        public void SimpleTest()
        {
            var syntaxTree = GetSimpleSyntaxTree();
            var methodDeclaration = syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Single();

            string expression = null;

            if (
                P.SingleStatement(
                    P.ExpressionStatement(
                        P.InvocationExpression(
                            P.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                P.IdentifierName("WriteLine"),
                                P.IdentifierName("Console")
                            ),
                            P.ArgumentList(
                                P.Argument(
                                    P.LiteralExpression(
                                        p => expression = p.Token.ValueText
                                    )
                                )
                            )
                        )
                    )
                ).IsMatch(methodDeclaration.Body)
            )
                Assert.AreEqual("Hello world!", expression);
        }

        [Test]
        public void RoslynTest()
        {
            var syntaxTree = GetSimpleSyntaxTree();
            var methodDeclaration = syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Single();

            if (
                methodDeclaration.Body is BlockSyntax block &&
                block.Statements[0] is ExpressionStatementSyntax expressionStatement &&
                expressionStatement.Expression is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name is IdentifierNameSyntax name &&
                name.Identifier.Text == "WriteLine" &&
                memberAccess.Expression is IdentifierNameSyntax expression &&
                expression.Identifier.Text == "Console" &&
                invocation.ArgumentList.Arguments.Count == 1 &&
                invocation.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax literalExpression
            )
            {
                Assert.AreEqual("Hello world!", literalExpression.Token.ValueText);
            }
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
        Console.WriteLine(""Hello world!"");
    }
}
            ");
            return syntaxTree;
        }
    }
}
