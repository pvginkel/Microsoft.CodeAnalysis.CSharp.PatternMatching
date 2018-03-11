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

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;

            if (node is ParenthesizedLambdaExpressionSyntax parenthesized)
            {
                if (_parameterList != null && !_parameterList.Test(parenthesized.ParameterList, semanticModel))
                    return false;
            }
            else if (node is SimpleLambdaExpressionSyntax simple)
            {
                if (_parameterList != null && !_parameterList.Test(
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

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            if (node is ParenthesizedLambdaExpressionSyntax parenthesized)
            {
                _parameterList?.RunCallback(parenthesized.ParameterList, semanticModel);
            }
            else
            {
                _parameterList?.RunCallback(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(new[] { ((SimpleLambdaExpressionSyntax)node).Parameter })
                    ),
                    semanticModel
                );
            }

            _action?.Invoke((LambdaExpressionSyntax)node);
        }
    }

    public class AnyLambdaExpressionPattern<TResult> : LambdaExpressionPattern<TResult>
    {
        private readonly ParameterListPattern<TResult> _parameterList;
        private readonly Func<TResult, LambdaExpressionSyntax, TResult> _action;

        internal AnyLambdaExpressionPattern(PatternNode<TResult> body, ParameterListPattern<TResult> parameterList, Func<TResult, LambdaExpressionSyntax, TResult> action)
            : base(body)
        {
            _parameterList = parameterList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;

            if (node is ParenthesizedLambdaExpressionSyntax parenthesized)
            {
                if (_parameterList != null && !_parameterList.Test(parenthesized.ParameterList, semanticModel))
                    return false;
            }
            else if (node is SimpleLambdaExpressionSyntax simple)
            {
                if (_parameterList != null && !_parameterList.Test(
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

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            if (node is ParenthesizedLambdaExpressionSyntax parenthesized)
            {
                if (_parameterList != null)
                    result = _parameterList.RunCallback(result, parenthesized.ParameterList, semanticModel);
            }
            else
            {
                if (_parameterList != null)
                {
                    result = _parameterList.RunCallback(
                        result,
                        SyntaxFactory.ParameterList(
                            SyntaxFactory.SeparatedList(new[] { ((SimpleLambdaExpressionSyntax)node).Parameter })
                        ),
                        semanticModel
                    );
                }
            }

            if (_action != null)
                result = _action(result, (LambdaExpressionSyntax)node);

            return result;
        }
    }
}
