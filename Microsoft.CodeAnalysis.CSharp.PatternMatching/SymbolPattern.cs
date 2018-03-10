using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public class SymbolPattern : ExpressionPattern
    {
        private readonly ISymbol _symbol;
        private readonly Action<ExpressionSyntax> _action;

        public SymbolPattern(ISymbol symbol, Action<ExpressionSyntax> action)
        {
            _symbol = symbol;
            _action = action;
        }

        public override bool IsMatch(SyntaxNode node, SemanticModel semanticModel = null)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (!(node is ExpressionSyntax typed))
                return false;

            if (!semanticModel.TryGetSymbol(typed, out var nodeSymbol))
                return false;
            if (_symbol != null && !_symbol.Equals(nodeSymbol))
                return false;

            _action?.Invoke(typed);

            return true;
        }
    }
}
