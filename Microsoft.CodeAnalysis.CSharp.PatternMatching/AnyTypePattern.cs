using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public class AnyTypePattern : TypePattern
    {
        private readonly Action<TypeSyntax> _action;

        public AnyTypePattern(Action<TypeSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            return node is TypeSyntax;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            _action?.Invoke((TypeSyntax)node);
        }
    }
}
