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

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            return node is StatementSyntax;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            _action?.Invoke((StatementSyntax)node);
        }
    }
}
