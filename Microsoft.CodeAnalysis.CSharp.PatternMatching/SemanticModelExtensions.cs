using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    internal static class SemanticModelExtensions
    {
        public static bool TryGetSymbol(this SemanticModel semanticModel, SyntaxNode node, out ISymbol result)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var symbol = semanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
            {
                result = symbol;
                return true;
            }

            var symbolInfo = semanticModel.GetSymbolInfo(node);
            if (symbolInfo.Symbol != null)
            {
                result = symbolInfo.Symbol;
                return true;
            }

            result = null;
            return false;
        }
    }
}
