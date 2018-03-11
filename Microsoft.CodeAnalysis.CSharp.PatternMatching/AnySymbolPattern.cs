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

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (!(node is ExpressionSyntax typed))
                return false;

            return semanticModel.TryGetSymbol(typed, out var _);
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (_action == null)
                return;

            var typed = (ExpressionSyntax)node;

            semanticModel.TryGetSymbol(typed, out var symbol);

            _action(typed, symbol);
        }
    }

    public class AnySymbolPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly Func<TResult, ExpressionSyntax, ISymbol, TResult> _action;

        internal AnySymbolPattern(Func<TResult, ExpressionSyntax, ISymbol, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (!(node is ExpressionSyntax typed))
                return false;

            return semanticModel.TryGetSymbol(typed, out var _);
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (_action == null)
                return result;

            var typed = (ExpressionSyntax)node;

            semanticModel.TryGetSymbol(typed, out var symbol);

            result = _action(result, typed, symbol);

            return result;
        }
    }
}
