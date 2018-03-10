using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public class SingleStatementPattern : StatementPattern
    {
        private readonly StatementPattern _statement;
        private readonly Action<StatementSyntax> _action;

        internal SingleStatementPattern(StatementPattern statement, Action<StatementSyntax> action)
        {
            _statement = statement;
            _action = action;
        }

        public override bool IsMatch(SyntaxNode node, SemanticModel semanticModel = null)
        {
            if (node is BlockSyntax block)
            {
                if (block.Statements.Count == 1)
                    return IsMatch(block.Statements[0]);
            }
            else if (node is StatementSyntax statement)
            {
                if (_statement != null && !_statement.IsMatch(statement))
                    return false;

                _action?.Invoke(statement);

                return true;
            }

            return false;
        }
    }
}
