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

        public override bool IsMatch(SyntaxNode node, SemanticModel semanticModel = null)
        {
            if (!(node is ExpressionSyntax typed))
                return false;

            _action?.Invoke(typed);

            return false;
        }
    }
}
