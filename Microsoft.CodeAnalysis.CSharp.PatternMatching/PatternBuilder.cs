using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public partial struct PatternBuilder<TResult>
    {
        public AnyNodePattern<ExpressionSyntax, TResult> AnyExpression(Func<TResult, ExpressionSyntax, TResult> action = null)
        {
            return new AnyNodePattern<ExpressionSyntax, TResult>(action);
        }

        public AnyNodePattern<StatementSyntax, TResult> AnyStatement(Func<TResult, StatementSyntax, TResult> action = null)
        {
            return new AnyNodePattern<StatementSyntax, TResult>(action);
        }

        public AnySymbolPattern<TResult> AnySymbol(Func<TResult, ExpressionSyntax, ISymbol, TResult> action = null)
        {
            return new AnySymbolPattern<TResult>(action);
        }

        public AnyNodePattern<TypeSyntax, TResult> AnyType(Func<TResult, TypeSyntax, TResult> action = null)
        {
            return new AnyNodePattern<TypeSyntax, TResult>(action);
        }

        public VarTypePattern<TResult> VarType(Func<TResult, TypeSyntax, TResult> action = null)
        {
            return new VarTypePattern<TResult>(action);
        }

        public AnyLambdaExpressionPattern<TResult> AnyLambdaExpression(PatternNode<TResult> body = null, ParameterListPattern<TResult> parameterList = null, Func<TResult, LambdaExpressionSyntax, TResult> action = null)
        {
            return new AnyLambdaExpressionPattern<TResult>(body, parameterList, action);
        }

        public SingleStatementPattern<TResult> SingleStatement(StatementPattern<TResult> statement = null, Func<TResult, StatementSyntax, TResult> action = null)
        {
            return new SingleStatementPattern<TResult>(statement, action);
        }

        public SymbolPattern<TResult> Symbol(ISymbol symbol = null, Func<TResult, ExpressionSyntax, TResult> action = null)
        {
            return new SymbolPattern<TResult>(symbol, action);
        }

        public NullStatementPattern<TResult> NullStatement(Func<TResult, TResult> action = null)
        {
            return new NullStatementPattern<TResult>(action);
        }

        public NullExpressionPattern<TResult> NullExpression(Func<TResult, TResult> action = null)
        {
            return new NullExpressionPattern<TResult>(action);
        }

        private NodeListPattern<T, TResult> NodeList<T>(IEnumerable<T> nodes)
            where T : PatternNode<TResult>
        {
            if (nodes == null)
                return null;

            return new NodeListPattern<T, TResult>(nodes);
        }

        private TokenListPattern TokenList(IEnumerable<string> tokens)
        {
            if (tokens == null)
                return null;

            return new TokenListPattern(tokens);
        }
    }
}
