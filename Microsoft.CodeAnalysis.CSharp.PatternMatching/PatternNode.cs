using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public abstract class PatternNode
    {
        public bool IsMatch(SyntaxNode node, SemanticModel semanticModel = null)
        {
            if (Test(node, semanticModel))
            {
                RunCallback(node, semanticModel);
                return true;
            }

            return false;
        }

        internal virtual bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            return true;
        }

        internal abstract void RunCallback(SyntaxNode node, SemanticModel semanticModel);
    }

    public abstract class PatternNode<TResult>
    {
        public PatternMatch<TResult> IsMatch(SyntaxNode node, SemanticModel semanticModel = null)
        {
            if (Test(node, semanticModel))
                return new PatternMatch<TResult>(true, RunCallback(default(TResult), node, semanticModel));

            return default(PatternMatch<TResult>);
        }

        internal virtual bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            return true;
        }

        internal abstract TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel);
    }
}
