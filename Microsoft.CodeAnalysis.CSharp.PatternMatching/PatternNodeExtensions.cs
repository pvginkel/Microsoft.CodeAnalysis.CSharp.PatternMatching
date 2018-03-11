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
        public static IList<SyntaxNode> Match(this PatternNode self, IEnumerable<SyntaxNode> nodes, SemanticModel semanticModel = null)
        {
            return self.Enumerate(nodes, semanticModel).ToList();
        }

        public static IEnumerable<SyntaxNode> Enumerate(this PatternNode self, IEnumerable<SyntaxNode> nodes, SemanticModel semanticModel = null)
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

        public static IList<SyntaxNode> MatchAncestors(this PatternNode self, SyntaxNode node, bool ascendOutOfTrivia = true, SemanticModel semanticModel = null)
        {
            return self.EnumerateAncestors(node, ascendOutOfTrivia, semanticModel).ToList();
        }

        public static IEnumerable<SyntaxNode> EnumerateAncestors(this PatternNode self, SyntaxNode node, bool ascendOutOfTrivia = true, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.Ancestors(ascendOutOfTrivia), semanticModel);
        }

        public static IList<SyntaxNode> MatchAncestorsAndSelf(this PatternNode self, SyntaxNode node, bool ascendOutOfTrivia = true, SemanticModel semanticModel = null)
        {
            return self.EnumerateAncestorsAndSelf(node, ascendOutOfTrivia, semanticModel).ToList();
        }

        public static IEnumerable<SyntaxNode> EnumerateAncestorsAndSelf(this PatternNode self, SyntaxNode node, bool ascendOutOfTrivia = true, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.AncestorsAndSelf(ascendOutOfTrivia), semanticModel);
        }

        public static IList<SyntaxNode> MatchChildNodes(this PatternNode self, SyntaxNode node, SemanticModel semanticModel = null)
        {
            return self.EnumerateChildNodes(node, semanticModel).ToList();
        }

        public static IEnumerable<SyntaxNode> EnumerateChildNodes(this PatternNode self, SyntaxNode node, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.ChildNodes(), semanticModel);
        }

        public static IList<SyntaxNode> MatchDescendantNodes(this PatternNode self, SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.EnumerateDescendantNodes(node, descendIntoChildren, descendIntoTrivia, semanticModel).ToList();
        }

        public static IEnumerable<SyntaxNode> EnumerateDescendantNodes(this PatternNode self, SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.DescendantNodes(descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IList<SyntaxNode> MatchDescendantNodes(this PatternNode self, SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.EnumerateDescendantNodes(node, span, descendIntoChildren, descendIntoTrivia, semanticModel).ToList();
        }

        public static IEnumerable<SyntaxNode> EnumerateDescendantNodes(this PatternNode self, SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.DescendantNodes(span, descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IList<SyntaxNode> MatchDescendantNodesAndSelf(this PatternNode self, SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.EnumerateDescendantNodesAndSelf(node, descendIntoChildren, descendIntoTrivia, semanticModel).ToList();
        }

        public static IEnumerable<SyntaxNode> EnumerateDescendantNodesAndSelf(this PatternNode self, SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.DescendantNodesAndSelf(descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IList<SyntaxNode> MatchDescendantNodesAndSelf(this PatternNode self, SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.EnumerateDescendantNodesAndSelf(node, span, descendIntoChildren, descendIntoTrivia, semanticModel).ToList();
        }

        public static IEnumerable<SyntaxNode> EnumerateDescendantNodesAndSelf(this PatternNode self, SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.DescendantNodesAndSelf(span, descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IList<SyntaxNode> MatchGetAnnotatedNodes(this PatternNode self, SyntaxNode node, string annotationKind, SemanticModel semanticModel = null)
        {
            return self.EnumerateGetAnnotatedNodes(node, annotationKind, semanticModel).ToList();
        }

        public static IEnumerable<SyntaxNode> EnumerateGetAnnotatedNodes(this PatternNode self, SyntaxNode node, string annotationKind, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.GetAnnotatedNodes(annotationKind), semanticModel);
        }

        public static IList<SyntaxNode> MatchGetAnnotatedNodes(this PatternNode self, SyntaxNode node, SyntaxAnnotation syntaxAnnotation, SemanticModel semanticModel = null)
        {
            return self.EnumerateGetAnnotatedNodes(node, syntaxAnnotation, semanticModel).ToList();
        }

        public static IEnumerable<SyntaxNode> EnumerateGetAnnotatedNodes(this PatternNode self, SyntaxNode node, SyntaxAnnotation syntaxAnnotation, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.GetAnnotatedNodes(syntaxAnnotation), semanticModel);
        }

        public static IList<TResult> Match<TResult>(this PatternNode<TResult> self, IEnumerable<SyntaxNode> nodes, SemanticModel semanticModel = null)
        {
            return self.Enumerate(nodes, semanticModel).ToList();
        }

        public static IEnumerable<TResult> Enumerate<TResult>(this PatternNode<TResult> self, IEnumerable<SyntaxNode> nodes, SemanticModel semanticModel = null)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            foreach (var node in nodes)
            {
                var result = self.IsMatch(node, semanticModel);
                if (result.Success)
                    yield return result.Result;
            }
        }

        public static IList<TResult> MatchAncestors<TResult>(this PatternNode<TResult> self, SyntaxNode node, bool ascendOutOfTrivia = true, SemanticModel semanticModel = null)
        {
            return self.EnumerateAncestors(node, ascendOutOfTrivia, semanticModel).ToList();
        }

        public static IEnumerable<TResult> EnumerateAncestors<TResult>(this PatternNode<TResult> self, SyntaxNode node, bool ascendOutOfTrivia = true, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.Ancestors(ascendOutOfTrivia), semanticModel);
        }

        public static IList<TResult> MatchAncestorsAndSelf<TResult>(this PatternNode<TResult> self, SyntaxNode node, bool ascendOutOfTrivia = true, SemanticModel semanticModel = null)
        {
            return self.EnumerateAncestorsAndSelf(node, ascendOutOfTrivia, semanticModel).ToList();
        }

        public static IEnumerable<TResult> EnumerateAncestorsAndSelf<TResult>(this PatternNode<TResult> self, SyntaxNode node, bool ascendOutOfTrivia = true, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.AncestorsAndSelf(ascendOutOfTrivia), semanticModel);
        }

        public static IList<TResult> MatchChildNodes<TResult>(this PatternNode<TResult> self, SyntaxNode node, SemanticModel semanticModel = null)
        {
            return self.EnumerateChildNodes(node, semanticModel).ToList();
        }

        public static IEnumerable<TResult> EnumerateChildNodes<TResult>(this PatternNode<TResult> self, SyntaxNode node, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.ChildNodes(), semanticModel);
        }

        public static IList<TResult> MatchDescendantNodes<TResult>(this PatternNode<TResult> self, SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.EnumerateDescendantNodes(node, descendIntoChildren, descendIntoTrivia, semanticModel).ToList();
        }

        public static IEnumerable<TResult> EnumerateDescendantNodes<TResult>(this PatternNode<TResult> self, SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.DescendantNodes(descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IList<TResult> MatchDescendantNodes<TResult>(this PatternNode<TResult> self, SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.EnumerateDescendantNodes(node, span, descendIntoChildren, descendIntoTrivia, semanticModel).ToList();
        }

        public static IEnumerable<TResult> EnumerateDescendantNodes<TResult>(this PatternNode<TResult> self, SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.DescendantNodes(span, descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IList<TResult> MatchDescendantNodesAndSelf<TResult>(this PatternNode<TResult> self, SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.EnumerateDescendantNodesAndSelf(node, descendIntoChildren, descendIntoTrivia, semanticModel).ToList();
        }

        public static IEnumerable<TResult> EnumerateDescendantNodesAndSelf<TResult>(this PatternNode<TResult> self, SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.DescendantNodesAndSelf(descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IList<TResult> MatchDescendantNodesAndSelf<TResult>(this PatternNode<TResult> self, SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.EnumerateDescendantNodesAndSelf(node, span, descendIntoChildren, descendIntoTrivia, semanticModel).ToList();
        }

        public static IEnumerable<TResult> EnumerateDescendantNodesAndSelf<TResult>(this PatternNode<TResult> self, SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.DescendantNodesAndSelf(span, descendIntoChildren, descendIntoTrivia), semanticModel);
        }

        public static IList<TResult> MatchGetAnnotatedNodes<TResult>(this PatternNode<TResult> self, SyntaxNode node, string annotationKind, SemanticModel semanticModel = null)
        {
            return self.EnumerateGetAnnotatedNodes(node, annotationKind, semanticModel).ToList();
        }

        public static IEnumerable<TResult> EnumerateGetAnnotatedNodes<TResult>(this PatternNode<TResult> self, SyntaxNode node, string annotationKind, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.GetAnnotatedNodes(annotationKind), semanticModel);
        }

        public static IList<TResult> MatchGetAnnotatedNodes<TResult>(this PatternNode<TResult> self, SyntaxNode node, SyntaxAnnotation syntaxAnnotation, SemanticModel semanticModel = null)
        {
            return self.EnumerateGetAnnotatedNodes(node, syntaxAnnotation, semanticModel).ToList();
        }

        public static IEnumerable<TResult> EnumerateGetAnnotatedNodes<TResult>(this PatternNode<TResult> self, SyntaxNode node, SyntaxAnnotation syntaxAnnotation, SemanticModel semanticModel = null)
        {
            return self.Enumerate(node.GetAnnotatedNodes(syntaxAnnotation), semanticModel);
        }
    }
}
