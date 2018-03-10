using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public class AnyExpressionPattern : ExpressionPattern
    {
        private readonly Action<ExpressionSyntax> _action;

        public AnyExpressionPattern(Action<ExpressionSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            return node is ExpressionSyntax;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            _action?.Invoke((ExpressionSyntax)node);
        }
    }
}
