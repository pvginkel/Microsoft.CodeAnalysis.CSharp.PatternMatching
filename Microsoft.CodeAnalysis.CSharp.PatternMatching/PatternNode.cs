using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public abstract class PatternNode
    {
        public virtual bool IsMatch(SyntaxNode node, SemanticModel semanticModel = null)
        {
            return true;
        }
    }
}
