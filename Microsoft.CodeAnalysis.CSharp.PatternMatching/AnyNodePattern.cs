using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public class AnyNodePattern<TNode> : PatternNode
        where TNode : SyntaxNode
    {
        private readonly Action<TNode> _action;

        public AnyNodePattern(Action<TNode> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            return node is TNode;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            _action?.Invoke((TNode)node);
        }
    }

    public class AnyNodePattern<TNode, TResult> : PatternNode<TResult>
        where TNode : SyntaxNode
    {
        private readonly Func<TResult, TNode, TResult> _action;

        public AnyNodePattern(Func<TResult, TNode, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            return node is TNode;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            if (_action != null)
                result = _action(result, (TNode)node);

            return result;
        }
    }
}
