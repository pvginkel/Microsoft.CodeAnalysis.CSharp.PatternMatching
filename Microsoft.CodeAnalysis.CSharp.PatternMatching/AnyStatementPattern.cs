using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public class AnyStatementPattern : StatementPattern
    {
        private readonly Action<StatementSyntax> _action;

        public AnyStatementPattern(Action<StatementSyntax> action)
        {
            _action = action;
        }

        public override bool IsMatch(SyntaxNode node, SemanticModel semanticModel = null)
        {
            if (!(node is StatementSyntax typed))
                return false;

            _action?.Invoke(typed);

            return true;
        }
    }
}
