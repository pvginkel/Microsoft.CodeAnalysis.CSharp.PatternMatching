using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public class NullExpressionPattern : ExpressionPattern
    {
        private readonly Action _action;

        internal NullExpressionPattern(Action action)
        {
            _action = action;
        }

        public override bool IsMatch(SyntaxNode node, SemanticModel semanticModel = null)
        {
            if (node != null)
                return false;

            _action?.Invoke();

            return true;
        }
    }
}
