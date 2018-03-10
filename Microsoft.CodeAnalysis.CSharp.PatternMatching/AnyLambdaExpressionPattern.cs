using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public class AnyLambdaExpressionPattern : LambdaExpressionPattern
    {
        private readonly ParameterListPattern _parameterList;
        private readonly Action<LambdaExpressionSyntax> _action;

        internal AnyLambdaExpressionPattern(PatternNode body, ParameterListPattern parameterList, Action<LambdaExpressionSyntax> action)
            : base(body)
        {
            _parameterList = parameterList;
            _action = action;
        }

        public override bool IsMatch(SyntaxNode node, SemanticModel semanticModel = null)
        {
            if (!base.IsMatch(node, semanticModel))
                return false;

            if (node is ParenthesizedLambdaExpressionSyntax parenthesized)
            {
                if (_parameterList != null && !_parameterList.IsMatch(parenthesized.ParameterList, semanticModel))
                    return false;
            }
            else if (node is SimpleLambdaExpressionSyntax simple)
            {
                if (_parameterList != null && !_parameterList.IsMatch(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(new[] { simple.Parameter })
                    ),
                    semanticModel
                ))
                    return false;
            }
            else
            {
                return false;
            }

            _action?.Invoke((LambdaExpressionSyntax)node);

            return true;
        }
    }
}
