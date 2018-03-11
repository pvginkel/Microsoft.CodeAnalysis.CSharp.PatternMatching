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

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (!(node is ExpressionSyntax typed))
                return false;

            if (!semanticModel.TryGetSymbol(typed, out var nodeSymbol))
                return false;

            return _symbol == null || _symbol.Equals(nodeSymbol);
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            _action?.Invoke((ExpressionSyntax)node);
        }
    }

    public class SymbolPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ISymbol _symbol;
        private readonly Func<TResult, ExpressionSyntax, TResult> _action;

        public SymbolPattern(ISymbol symbol, Func<TResult, ExpressionSyntax, TResult> action)
        {
            _symbol = symbol;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (!(node is ExpressionSyntax typed))
                return false;

            if (!semanticModel.TryGetSymbol(typed, out var nodeSymbol))
                return false;

            return _symbol == null || _symbol.Equals(nodeSymbol);
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (_action != null)
                result = _action(result, (ExpressionSyntax)node);

            return result;
        }
    }
}
