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
}
