using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public static class PatternNodeExtensions
    {
        public static IEnumerable<SyntaxNode> Match(this PatternNode self, IEnumerable<SyntaxNode> nodes, SemanticModel semanticModel = null)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            foreach (var node in nodes)
            {
                if (self.IsMatch(node, semanticModel))
                    yield return node;
            }
        }

        public static IEnumerable<SyntaxNode> MatchAncestors(this PatternNode self, SyntaxNode node, bool ascendOutOfTrivia = true, SemanticModel semanticModel = null)
        {
            return self.Match(node.Ancestors(ascendOutOfTrivia), semanticModel);
        }

        public static IEnumerable<SyntaxNode> MatchAncestorsAndSelf(this PatternNode self, SyntaxNode node, bool ascendOutOfTrivia = true, SemanticModel semanticModel = null)
        {
            return self.Match(node.AncestorsAndSelf(ascendOutOfTrivia), semanticModel);
        }

        public static IEnumerable<SyntaxNode> MatchChildNodes(this PatternNode self, SyntaxNode node, SemanticModel semanticModel = null)
        {
            return self.Match(node.ChildNodes(), semanticModel);
        }

        public static IEnumerable<SyntaxNode> MatchDescendantNodes(this PatternNode self, SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Match(node.DescendantNodes(descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IEnumerable<SyntaxNode> MatchDescendantNodes(this PatternNode self, SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Match(node.DescendantNodes(span, descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IEnumerable<SyntaxNode> MatchDescendantNodesAndSelf(this PatternNode self, SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Match(node.DescendantNodesAndSelf(descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IEnumerable<SyntaxNode> MatchDescendantNodesAndSelf(this PatternNode self, SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Match(node.DescendantNodesAndSelf(span, descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IEnumerable<SyntaxNode> MatchGetAnnotatedNodes(this PatternNode self, SyntaxNode node, string annotationKind, SemanticModel semanticModel = null)
        {
            return self.Match(node.GetAnnotatedNodes(annotationKind), semanticModel);
        }

        public static IEnumerable<SyntaxNode> MatchGetAnnotatedNodes(this PatternNode self, SyntaxNode node, SyntaxAnnotation syntaxAnnotation, SemanticModel semanticModel = null)
        {
            return self.Match(node.GetAnnotatedNodes(syntaxAnnotation), semanticModel);
        }
    }
}
