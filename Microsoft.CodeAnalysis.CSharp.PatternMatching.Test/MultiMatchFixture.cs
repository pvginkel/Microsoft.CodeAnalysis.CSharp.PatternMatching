using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using P = Microsoft.CodeAnalysis.CSharp.PatternMatching.Pattern;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching.Test
{
    [TestFixture]
    public class MultiMatchFixture
    {
        public void MatchAllStringInvocations()
        {
            var syntaxTree = GetSimpleSyntaxTree();
            var methodDeclaration = syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Single();

            var strings = new List<string>();

            int matches =
                P.InvocationExpression(
                    P.MemberAccessExpression(),
                    P.ArgumentList(
                        P.Argument(
                            P.LiteralExpression(
                                p => strings.Add(p.Token.ValueText)
                            )
                        )
                    )
                )
                .MatchDescendantNodes(methodDeclaration.Body)
                .Count;

            Assert.AreEqual(2, matches);
            Assert.AreEqual(new List<string> { "Hello ", "world!" }, strings);
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
        Console.Write(""Hello "");
        Console.WriteLine(""world!"");
    }
}
            ");
            return syntaxTree;
        }
    }
}
