using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public class AnySymbolPattern : ExpressionPattern
    {
        private readonly Action<ExpressionSyntax, ISymbol> _action;

        internal AnySymbolPattern(Action<ExpressionSyntax, ISymbol> action)
        {
            _action = action;
        }

        public override bool IsMatch(SyntaxNode node, SemanticModel semanticModel = null)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (!(node is ExpressionSyntax typed))
                return false;

            if (!semanticModel.TryGetSymbol(typed, out var symbol))
                return false;

            _action?.Invoke(typed, symbol);

            return true;
        }
    }
}
