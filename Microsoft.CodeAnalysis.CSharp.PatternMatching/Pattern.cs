using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public static partial class Pattern
    {
        public static AnyNodePattern<ExpressionSyntax> AnyExpression(Action<ExpressionSyntax> action = null)
        {
            return new AnyNodePattern<ExpressionSyntax>(action);
        }

        public static AnyNodePattern<StatementSyntax> AnyStatement(Action<StatementSyntax> action = null)
        {
            return new AnyNodePattern<StatementSyntax>(action);
        }

        public static AnySymbolPattern AnySymbol(Action<ExpressionSyntax, ISymbol> action = null)
        {
            return new AnySymbolPattern(action);
        }

        public static AnyNodePattern<TypeSyntax> AnyType(Action<TypeSyntax> action = null)
        {
            return new AnyNodePattern<TypeSyntax>(action);
        }

        public static VarTypePattern VarType(Action<TypeSyntax> action = null)
        {
            return new VarTypePattern(action);
        }

        public static AnyLambdaExpressionPattern AnyLambdaExpression(PatternNode body = null, ParameterListPattern parameterList = null, Action<LambdaExpressionSyntax> action = null)
        {
            return new AnyLambdaExpressionPattern(body, parameterList, action);
        }

        public static SingleStatementPattern SingleStatement(StatementPattern statement = null, Action<StatementSyntax> action = null)
        {
            return new SingleStatementPattern(statement, action);
        }

        public static SymbolPattern Symbol(ISymbol symbol = null, Action<ExpressionSyntax> action = null)
        {
            return new SymbolPattern(symbol, action);
        }

        public static NullStatementPattern NullStatement(Action action = null)
        {
            return new NullStatementPattern(action);
        }

        public static NullExpressionPattern NullExpression(Action action = null)
        {
            return new NullExpressionPattern(action);
        }

        private static NodeListPattern<T> NodeList<T>(IEnumerable<T> nodes)
            where T : PatternNode
        {
            if (nodes == null)
                return null;

            return new NodeListPattern<T>(nodes);
        }

        private static TokenListPattern TokenList(IEnumerable<string> tokens)
        {
            if (tokens == null)
                return null;

            return new TokenListPattern(tokens);
        }
    }
}
