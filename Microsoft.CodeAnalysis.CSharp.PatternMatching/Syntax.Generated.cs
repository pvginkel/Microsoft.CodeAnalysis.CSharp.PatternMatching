using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public abstract partial class NamePattern<TResult> : TypePattern<TResult>
    {

        internal NamePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NameSyntax typed))
                return false;


            return true;
        }
    }

    public abstract partial class SimpleNamePattern<TResult> : NamePattern<TResult>
    {
        private readonly string _identifier;

        internal SimpleNamePattern(string identifier)
        {
            _identifier = identifier;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SimpleNameSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }
    }

    public partial class IdentifierNamePattern<TResult> : SimpleNamePattern<TResult>
    {
        private readonly Func<TResult, IdentifierNameSyntax, TResult> _action;

        internal IdentifierNamePattern(string identifier, Func<TResult, IdentifierNameSyntax, TResult> action)
            : base(identifier)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IdentifierNameSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (IdentifierNameSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class QualifiedNamePattern<TResult> : NamePattern<TResult>
    {
        private readonly NamePattern<TResult> _left;
        private readonly SimpleNamePattern<TResult> _right;
        private readonly Func<TResult, QualifiedNameSyntax, TResult> _action;

        internal QualifiedNamePattern(NamePattern<TResult> left, SimpleNamePattern<TResult> right, Func<TResult, QualifiedNameSyntax, TResult> action)
        {
            _left = left;
            _right = right;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QualifiedNameSyntax typed))
                return false;

            if (_left != null && !_left.Test(typed.Left, semanticModel))
                return false;
            if (_right != null && !_right.Test(typed.Right, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QualifiedNameSyntax)node;

            if (_left != null)
                result = _left.RunCallback(result, typed.Left, semanticModel);
            if (_right != null)
                result = _right.RunCallback(result, typed.Right, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class GenericNamePattern<TResult> : SimpleNamePattern<TResult>
    {
        private readonly TypeArgumentListPattern<TResult> _typeArgumentList;
        private readonly Func<TResult, GenericNameSyntax, TResult> _action;

        internal GenericNamePattern(string identifier, TypeArgumentListPattern<TResult> typeArgumentList, Func<TResult, GenericNameSyntax, TResult> action)
            : base(identifier)
        {
            _typeArgumentList = typeArgumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GenericNameSyntax typed))
                return false;

            if (_typeArgumentList != null && !_typeArgumentList.Test(typed.TypeArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (GenericNameSyntax)node;

            if (_typeArgumentList != null)
                result = _typeArgumentList.RunCallback(result, typed.TypeArgumentList, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class TypeArgumentListPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<TypePattern<TResult>, TResult> _arguments;
        private readonly Func<TResult, TypeArgumentListSyntax, TResult> _action;

        internal TypeArgumentListPattern(NodeListPattern<TypePattern<TResult>, TResult> arguments, Func<TResult, TypeArgumentListSyntax, TResult> action)
        {
            _arguments = arguments;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeArgumentListSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeArgumentListSyntax)node;

            if (_arguments != null)
                result = _arguments.RunCallback(result, typed.Arguments, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AliasQualifiedNamePattern<TResult> : NamePattern<TResult>
    {
        private readonly IdentifierNamePattern<TResult> _alias;
        private readonly SimpleNamePattern<TResult> _name;
        private readonly Func<TResult, AliasQualifiedNameSyntax, TResult> _action;

        internal AliasQualifiedNamePattern(IdentifierNamePattern<TResult> alias, SimpleNamePattern<TResult> name, Func<TResult, AliasQualifiedNameSyntax, TResult> action)
        {
            _alias = alias;
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AliasQualifiedNameSyntax typed))
                return false;

            if (_alias != null && !_alias.Test(typed.Alias, semanticModel))
                return false;
            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AliasQualifiedNameSyntax)node;

            if (_alias != null)
                result = _alias.RunCallback(result, typed.Alias, semanticModel);
            if (_name != null)
                result = _name.RunCallback(result, typed.Name, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class TypePattern<TResult> : ExpressionPattern<TResult>
    {

        internal TypePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeSyntax typed))
                return false;


            return true;
        }
    }

    public partial class PredefinedTypePattern<TResult> : TypePattern<TResult>
    {
        private readonly string _keyword;
        private readonly Func<TResult, PredefinedTypeSyntax, TResult> _action;

        internal PredefinedTypePattern(string keyword, Func<TResult, PredefinedTypeSyntax, TResult> action)
        {
            _keyword = keyword;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PredefinedTypeSyntax typed))
                return false;

            if (_keyword != null && _keyword != typed.Keyword.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PredefinedTypeSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ArrayTypePattern<TResult> : TypePattern<TResult>
    {
        private readonly TypePattern<TResult> _elementType;
        private readonly NodeListPattern<ArrayRankSpecifierPattern<TResult>, TResult> _rankSpecifiers;
        private readonly Func<TResult, ArrayTypeSyntax, TResult> _action;

        internal ArrayTypePattern(TypePattern<TResult> elementType, NodeListPattern<ArrayRankSpecifierPattern<TResult>, TResult> rankSpecifiers, Func<TResult, ArrayTypeSyntax, TResult> action)
        {
            _elementType = elementType;
            _rankSpecifiers = rankSpecifiers;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrayTypeSyntax typed))
                return false;

            if (_elementType != null && !_elementType.Test(typed.ElementType, semanticModel))
                return false;
            if (_rankSpecifiers != null && !_rankSpecifiers.Test(typed.RankSpecifiers, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrayTypeSyntax)node;

            if (_elementType != null)
                result = _elementType.RunCallback(result, typed.ElementType, semanticModel);
            if (_rankSpecifiers != null)
                result = _rankSpecifiers.RunCallback(result, typed.RankSpecifiers, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ArrayRankSpecifierPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<ExpressionPattern<TResult>, TResult> _sizes;
        private readonly Func<TResult, ArrayRankSpecifierSyntax, TResult> _action;

        internal ArrayRankSpecifierPattern(NodeListPattern<ExpressionPattern<TResult>, TResult> sizes, Func<TResult, ArrayRankSpecifierSyntax, TResult> action)
        {
            _sizes = sizes;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrayRankSpecifierSyntax typed))
                return false;

            if (_sizes != null && !_sizes.Test(typed.Sizes, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrayRankSpecifierSyntax)node;

            if (_sizes != null)
                result = _sizes.RunCallback(result, typed.Sizes, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class PointerTypePattern<TResult> : TypePattern<TResult>
    {
        private readonly TypePattern<TResult> _elementType;
        private readonly Func<TResult, PointerTypeSyntax, TResult> _action;

        internal PointerTypePattern(TypePattern<TResult> elementType, Func<TResult, PointerTypeSyntax, TResult> action)
        {
            _elementType = elementType;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PointerTypeSyntax typed))
                return false;

            if (_elementType != null && !_elementType.Test(typed.ElementType, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PointerTypeSyntax)node;

            if (_elementType != null)
                result = _elementType.RunCallback(result, typed.ElementType, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class NullableTypePattern<TResult> : TypePattern<TResult>
    {
        private readonly TypePattern<TResult> _elementType;
        private readonly Func<TResult, NullableTypeSyntax, TResult> _action;

        internal NullableTypePattern(TypePattern<TResult> elementType, Func<TResult, NullableTypeSyntax, TResult> action)
        {
            _elementType = elementType;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NullableTypeSyntax typed))
                return false;

            if (_elementType != null && !_elementType.Test(typed.ElementType, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NullableTypeSyntax)node;

            if (_elementType != null)
                result = _elementType.RunCallback(result, typed.ElementType, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class TupleTypePattern<TResult> : TypePattern<TResult>
    {
        private readonly NodeListPattern<TupleElementPattern<TResult>, TResult> _elements;
        private readonly Func<TResult, TupleTypeSyntax, TResult> _action;

        internal TupleTypePattern(NodeListPattern<TupleElementPattern<TResult>, TResult> elements, Func<TResult, TupleTypeSyntax, TResult> action)
        {
            _elements = elements;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TupleTypeSyntax typed))
                return false;

            if (_elements != null && !_elements.Test(typed.Elements, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TupleTypeSyntax)node;

            if (_elements != null)
                result = _elements.RunCallback(result, typed.Elements, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class TupleElementPattern<TResult> : PatternNode<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly string _identifier;
        private readonly Func<TResult, TupleElementSyntax, TResult> _action;

        internal TupleElementPattern(TypePattern<TResult> type, string identifier, Func<TResult, TupleElementSyntax, TResult> action)
        {
            _type = type;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TupleElementSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TupleElementSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class OmittedTypeArgumentPattern<TResult> : TypePattern<TResult>
    {
        private readonly Func<TResult, OmittedTypeArgumentSyntax, TResult> _action;

        internal OmittedTypeArgumentPattern(Func<TResult, OmittedTypeArgumentSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OmittedTypeArgumentSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OmittedTypeArgumentSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class RefTypePattern<TResult> : TypePattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly Func<TResult, RefTypeSyntax, TResult> _action;

        internal RefTypePattern(TypePattern<TResult> type, Func<TResult, RefTypeSyntax, TResult> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefTypeSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefTypeSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class ExpressionPattern<TResult> : PatternNode<TResult>
    {

        internal ExpressionPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExpressionSyntax typed))
                return false;


            return true;
        }
    }

    public partial class ParenthesizedExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, ParenthesizedExpressionSyntax, TResult> _action;

        internal ParenthesizedExpressionPattern(ExpressionPattern<TResult> expression, Func<TResult, ParenthesizedExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParenthesizedExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParenthesizedExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class TupleExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly NodeListPattern<ArgumentPattern<TResult>, TResult> _arguments;
        private readonly Func<TResult, TupleExpressionSyntax, TResult> _action;

        internal TupleExpressionPattern(NodeListPattern<ArgumentPattern<TResult>, TResult> arguments, Func<TResult, TupleExpressionSyntax, TResult> action)
        {
            _arguments = arguments;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TupleExpressionSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TupleExpressionSyntax)node;

            if (_arguments != null)
                result = _arguments.RunCallback(result, typed.Arguments, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class PrefixUnaryExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern<TResult> _operand;
        private readonly Func<TResult, PrefixUnaryExpressionSyntax, TResult> _action;

        internal PrefixUnaryExpressionPattern(SyntaxKind kind, ExpressionPattern<TResult> operand, Func<TResult, PrefixUnaryExpressionSyntax, TResult> action)
        {
            _kind = kind;
            _operand = operand;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PrefixUnaryExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_operand != null && !_operand.Test(typed.Operand, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PrefixUnaryExpressionSyntax)node;

            if (_operand != null)
                result = _operand.RunCallback(result, typed.Operand, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AwaitExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, AwaitExpressionSyntax, TResult> _action;

        internal AwaitExpressionPattern(ExpressionPattern<TResult> expression, Func<TResult, AwaitExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AwaitExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AwaitExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class PostfixUnaryExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern<TResult> _operand;
        private readonly Func<TResult, PostfixUnaryExpressionSyntax, TResult> _action;

        internal PostfixUnaryExpressionPattern(SyntaxKind kind, ExpressionPattern<TResult> operand, Func<TResult, PostfixUnaryExpressionSyntax, TResult> action)
        {
            _kind = kind;
            _operand = operand;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PostfixUnaryExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_operand != null && !_operand.Test(typed.Operand, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PostfixUnaryExpressionSyntax)node;

            if (_operand != null)
                result = _operand.RunCallback(result, typed.Operand, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class MemberAccessExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly SimpleNamePattern<TResult> _name;
        private readonly Func<TResult, MemberAccessExpressionSyntax, TResult> _action;

        internal MemberAccessExpressionPattern(SyntaxKind kind, ExpressionPattern<TResult> expression, SimpleNamePattern<TResult> name, Func<TResult, MemberAccessExpressionSyntax, TResult> action)
        {
            _kind = kind;
            _expression = expression;
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MemberAccessExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (MemberAccessExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_name != null)
                result = _name.RunCallback(result, typed.Name, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ConditionalAccessExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly ExpressionPattern<TResult> _whenNotNull;
        private readonly Func<TResult, ConditionalAccessExpressionSyntax, TResult> _action;

        internal ConditionalAccessExpressionPattern(ExpressionPattern<TResult> expression, ExpressionPattern<TResult> whenNotNull, Func<TResult, ConditionalAccessExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _whenNotNull = whenNotNull;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConditionalAccessExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_whenNotNull != null && !_whenNotNull.Test(typed.WhenNotNull, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConditionalAccessExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_whenNotNull != null)
                result = _whenNotNull.RunCallback(result, typed.WhenNotNull, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class MemberBindingExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly SimpleNamePattern<TResult> _name;
        private readonly Func<TResult, MemberBindingExpressionSyntax, TResult> _action;

        internal MemberBindingExpressionPattern(SimpleNamePattern<TResult> name, Func<TResult, MemberBindingExpressionSyntax, TResult> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MemberBindingExpressionSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (MemberBindingExpressionSyntax)node;

            if (_name != null)
                result = _name.RunCallback(result, typed.Name, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ElementBindingExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly BracketedArgumentListPattern<TResult> _argumentList;
        private readonly Func<TResult, ElementBindingExpressionSyntax, TResult> _action;

        internal ElementBindingExpressionPattern(BracketedArgumentListPattern<TResult> argumentList, Func<TResult, ElementBindingExpressionSyntax, TResult> action)
        {
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ElementBindingExpressionSyntax typed))
                return false;

            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ElementBindingExpressionSyntax)node;

            if (_argumentList != null)
                result = _argumentList.RunCallback(result, typed.ArgumentList, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ImplicitElementAccessPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly BracketedArgumentListPattern<TResult> _argumentList;
        private readonly Func<TResult, ImplicitElementAccessSyntax, TResult> _action;

        internal ImplicitElementAccessPattern(BracketedArgumentListPattern<TResult> argumentList, Func<TResult, ImplicitElementAccessSyntax, TResult> action)
        {
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ImplicitElementAccessSyntax typed))
                return false;

            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ImplicitElementAccessSyntax)node;

            if (_argumentList != null)
                result = _argumentList.RunCallback(result, typed.ArgumentList, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class BinaryExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern<TResult> _left;
        private readonly ExpressionPattern<TResult> _right;
        private readonly Func<TResult, BinaryExpressionSyntax, TResult> _action;

        internal BinaryExpressionPattern(SyntaxKind kind, ExpressionPattern<TResult> left, ExpressionPattern<TResult> right, Func<TResult, BinaryExpressionSyntax, TResult> action)
        {
            _kind = kind;
            _left = left;
            _right = right;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BinaryExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_left != null && !_left.Test(typed.Left, semanticModel))
                return false;
            if (_right != null && !_right.Test(typed.Right, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BinaryExpressionSyntax)node;

            if (_left != null)
                result = _left.RunCallback(result, typed.Left, semanticModel);
            if (_right != null)
                result = _right.RunCallback(result, typed.Right, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AssignmentExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern<TResult> _left;
        private readonly ExpressionPattern<TResult> _right;
        private readonly Func<TResult, AssignmentExpressionSyntax, TResult> _action;

        internal AssignmentExpressionPattern(SyntaxKind kind, ExpressionPattern<TResult> left, ExpressionPattern<TResult> right, Func<TResult, AssignmentExpressionSyntax, TResult> action)
        {
            _kind = kind;
            _left = left;
            _right = right;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AssignmentExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_left != null && !_left.Test(typed.Left, semanticModel))
                return false;
            if (_right != null && !_right.Test(typed.Right, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AssignmentExpressionSyntax)node;

            if (_left != null)
                result = _left.RunCallback(result, typed.Left, semanticModel);
            if (_right != null)
                result = _right.RunCallback(result, typed.Right, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ConditionalExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _condition;
        private readonly ExpressionPattern<TResult> _whenTrue;
        private readonly ExpressionPattern<TResult> _whenFalse;
        private readonly Func<TResult, ConditionalExpressionSyntax, TResult> _action;

        internal ConditionalExpressionPattern(ExpressionPattern<TResult> condition, ExpressionPattern<TResult> whenTrue, ExpressionPattern<TResult> whenFalse, Func<TResult, ConditionalExpressionSyntax, TResult> action)
        {
            _condition = condition;
            _whenTrue = whenTrue;
            _whenFalse = whenFalse;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConditionalExpressionSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_whenTrue != null && !_whenTrue.Test(typed.WhenTrue, semanticModel))
                return false;
            if (_whenFalse != null && !_whenFalse.Test(typed.WhenFalse, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConditionalExpressionSyntax)node;

            if (_condition != null)
                result = _condition.RunCallback(result, typed.Condition, semanticModel);
            if (_whenTrue != null)
                result = _whenTrue.RunCallback(result, typed.WhenTrue, semanticModel);
            if (_whenFalse != null)
                result = _whenFalse.RunCallback(result, typed.WhenFalse, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class InstanceExpressionPattern<TResult> : ExpressionPattern<TResult>
    {

        internal InstanceExpressionPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InstanceExpressionSyntax typed))
                return false;


            return true;
        }
    }

    public partial class ThisExpressionPattern<TResult> : InstanceExpressionPattern<TResult>
    {
        private readonly Func<TResult, ThisExpressionSyntax, TResult> _action;

        internal ThisExpressionPattern(Func<TResult, ThisExpressionSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ThisExpressionSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ThisExpressionSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class BaseExpressionPattern<TResult> : InstanceExpressionPattern<TResult>
    {
        private readonly Func<TResult, BaseExpressionSyntax, TResult> _action;

        internal BaseExpressionPattern(Func<TResult, BaseExpressionSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseExpressionSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseExpressionSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class LiteralExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly Func<TResult, LiteralExpressionSyntax, TResult> _action;

        internal LiteralExpressionPattern(SyntaxKind kind, Func<TResult, LiteralExpressionSyntax, TResult> action)
        {
            _kind = kind;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LiteralExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LiteralExpressionSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class MakeRefExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, MakeRefExpressionSyntax, TResult> _action;

        internal MakeRefExpressionPattern(ExpressionPattern<TResult> expression, Func<TResult, MakeRefExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MakeRefExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (MakeRefExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class RefTypeExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, RefTypeExpressionSyntax, TResult> _action;

        internal RefTypeExpressionPattern(ExpressionPattern<TResult> expression, Func<TResult, RefTypeExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefTypeExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefTypeExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class RefValueExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly TypePattern<TResult> _type;
        private readonly Func<TResult, RefValueExpressionSyntax, TResult> _action;

        internal RefValueExpressionPattern(ExpressionPattern<TResult> expression, TypePattern<TResult> type, Func<TResult, RefValueExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefValueExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefValueExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class CheckedExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, CheckedExpressionSyntax, TResult> _action;

        internal CheckedExpressionPattern(SyntaxKind kind, ExpressionPattern<TResult> expression, Func<TResult, CheckedExpressionSyntax, TResult> action)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CheckedExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CheckedExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class DefaultExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly Func<TResult, DefaultExpressionSyntax, TResult> _action;

        internal DefaultExpressionPattern(TypePattern<TResult> type, Func<TResult, DefaultExpressionSyntax, TResult> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DefaultExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DefaultExpressionSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class TypeOfExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly Func<TResult, TypeOfExpressionSyntax, TResult> _action;

        internal TypeOfExpressionPattern(TypePattern<TResult> type, Func<TResult, TypeOfExpressionSyntax, TResult> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeOfExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeOfExpressionSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class SizeOfExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly Func<TResult, SizeOfExpressionSyntax, TResult> _action;

        internal SizeOfExpressionPattern(TypePattern<TResult> type, Func<TResult, SizeOfExpressionSyntax, TResult> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SizeOfExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SizeOfExpressionSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class InvocationExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly ArgumentListPattern<TResult> _argumentList;
        private readonly Func<TResult, InvocationExpressionSyntax, TResult> _action;

        internal InvocationExpressionPattern(ExpressionPattern<TResult> expression, ArgumentListPattern<TResult> argumentList, Func<TResult, InvocationExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InvocationExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InvocationExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_argumentList != null)
                result = _argumentList.RunCallback(result, typed.ArgumentList, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ElementAccessExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly BracketedArgumentListPattern<TResult> _argumentList;
        private readonly Func<TResult, ElementAccessExpressionSyntax, TResult> _action;

        internal ElementAccessExpressionPattern(ExpressionPattern<TResult> expression, BracketedArgumentListPattern<TResult> argumentList, Func<TResult, ElementAccessExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ElementAccessExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ElementAccessExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_argumentList != null)
                result = _argumentList.RunCallback(result, typed.ArgumentList, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class BaseArgumentListPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<ArgumentPattern<TResult>, TResult> _arguments;

        internal BaseArgumentListPattern(NodeListPattern<ArgumentPattern<TResult>, TResult> arguments)
        {
            _arguments = arguments;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseArgumentListSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseArgumentListSyntax)node;

            if (_arguments != null)
                result = _arguments.RunCallback(result, typed.Arguments, semanticModel);
            return result;
        }
    }

    public partial class ArgumentListPattern<TResult> : BaseArgumentListPattern<TResult>
    {
        private readonly Func<TResult, ArgumentListSyntax, TResult> _action;

        internal ArgumentListPattern(NodeListPattern<ArgumentPattern<TResult>, TResult> arguments, Func<TResult, ArgumentListSyntax, TResult> action)
            : base(arguments)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArgumentListSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (ArgumentListSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class BracketedArgumentListPattern<TResult> : BaseArgumentListPattern<TResult>
    {
        private readonly Func<TResult, BracketedArgumentListSyntax, TResult> _action;

        internal BracketedArgumentListPattern(NodeListPattern<ArgumentPattern<TResult>, TResult> arguments, Func<TResult, BracketedArgumentListSyntax, TResult> action)
            : base(arguments)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BracketedArgumentListSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (BracketedArgumentListSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ArgumentPattern<TResult> : PatternNode<TResult>
    {
        private readonly NameColonPattern<TResult> _nameColon;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, ArgumentSyntax, TResult> _action;

        internal ArgumentPattern(NameColonPattern<TResult> nameColon, ExpressionPattern<TResult> expression, Func<TResult, ArgumentSyntax, TResult> action)
        {
            _nameColon = nameColon;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArgumentSyntax typed))
                return false;

            if (_nameColon != null && !_nameColon.Test(typed.NameColon, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArgumentSyntax)node;

            if (_nameColon != null)
                result = _nameColon.RunCallback(result, typed.NameColon, semanticModel);
            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class NameColonPattern<TResult> : PatternNode<TResult>
    {
        private readonly IdentifierNamePattern<TResult> _name;
        private readonly Func<TResult, NameColonSyntax, TResult> _action;

        internal NameColonPattern(IdentifierNamePattern<TResult> name, Func<TResult, NameColonSyntax, TResult> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NameColonSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NameColonSyntax)node;

            if (_name != null)
                result = _name.RunCallback(result, typed.Name, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class DeclarationExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly VariableDesignationPattern<TResult> _designation;
        private readonly Func<TResult, DeclarationExpressionSyntax, TResult> _action;

        internal DeclarationExpressionPattern(TypePattern<TResult> type, VariableDesignationPattern<TResult> designation, Func<TResult, DeclarationExpressionSyntax, TResult> action)
        {
            _type = type;
            _designation = designation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DeclarationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_designation != null && !_designation.Test(typed.Designation, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DeclarationExpressionSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            if (_designation != null)
                result = _designation.RunCallback(result, typed.Designation, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class CastExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, CastExpressionSyntax, TResult> _action;

        internal CastExpressionPattern(TypePattern<TResult> type, ExpressionPattern<TResult> expression, Func<TResult, CastExpressionSyntax, TResult> action)
        {
            _type = type;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CastExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CastExpressionSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class AnonymousFunctionExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly PatternNode<TResult> _body;

        internal AnonymousFunctionExpressionPattern(PatternNode<TResult> body)
        {
            _body = body;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousFunctionExpressionSyntax typed))
                return false;

            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AnonymousFunctionExpressionSyntax)node;

            if (_body != null)
                result = _body.RunCallback(result, typed.Body, semanticModel);
            return result;
        }
    }

    public partial class AnonymousMethodExpressionPattern<TResult> : AnonymousFunctionExpressionPattern<TResult>
    {
        private readonly ParameterListPattern<TResult> _parameterList;
        private readonly Func<TResult, AnonymousMethodExpressionSyntax, TResult> _action;

        internal AnonymousMethodExpressionPattern(PatternNode<TResult> body, ParameterListPattern<TResult> parameterList, Func<TResult, AnonymousMethodExpressionSyntax, TResult> action)
            : base(body)
        {
            _parameterList = parameterList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousMethodExpressionSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (AnonymousMethodExpressionSyntax)node;

            if (_parameterList != null)
                result = _parameterList.RunCallback(result, typed.ParameterList, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class LambdaExpressionPattern<TResult> : AnonymousFunctionExpressionPattern<TResult>
    {

        internal LambdaExpressionPattern(PatternNode<TResult> body)
            : base(body)
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LambdaExpressionSyntax typed))
                return false;


            return true;
        }
    }

    public partial class SimpleLambdaExpressionPattern<TResult> : LambdaExpressionPattern<TResult>
    {
        private readonly ParameterPattern<TResult> _parameter;
        private readonly Func<TResult, SimpleLambdaExpressionSyntax, TResult> _action;

        internal SimpleLambdaExpressionPattern(PatternNode<TResult> body, ParameterPattern<TResult> parameter, Func<TResult, SimpleLambdaExpressionSyntax, TResult> action)
            : base(body)
        {
            _parameter = parameter;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SimpleLambdaExpressionSyntax typed))
                return false;

            if (_parameter != null && !_parameter.Test(typed.Parameter, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SimpleLambdaExpressionSyntax)node;

            if (_parameter != null)
                result = _parameter.RunCallback(result, typed.Parameter, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class RefExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, RefExpressionSyntax, TResult> _action;

        internal RefExpressionPattern(ExpressionPattern<TResult> expression, Func<TResult, RefExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ParenthesizedLambdaExpressionPattern<TResult> : LambdaExpressionPattern<TResult>
    {
        private readonly ParameterListPattern<TResult> _parameterList;
        private readonly Func<TResult, ParenthesizedLambdaExpressionSyntax, TResult> _action;

        internal ParenthesizedLambdaExpressionPattern(PatternNode<TResult> body, ParameterListPattern<TResult> parameterList, Func<TResult, ParenthesizedLambdaExpressionSyntax, TResult> action)
            : base(body)
        {
            _parameterList = parameterList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParenthesizedLambdaExpressionSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParenthesizedLambdaExpressionSyntax)node;

            if (_parameterList != null)
                result = _parameterList.RunCallback(result, typed.ParameterList, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class InitializerExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly NodeListPattern<ExpressionPattern<TResult>, TResult> _expressions;
        private readonly Func<TResult, InitializerExpressionSyntax, TResult> _action;

        internal InitializerExpressionPattern(SyntaxKind kind, NodeListPattern<ExpressionPattern<TResult>, TResult> expressions, Func<TResult, InitializerExpressionSyntax, TResult> action)
        {
            _kind = kind;
            _expressions = expressions;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InitializerExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expressions != null && !_expressions.Test(typed.Expressions, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InitializerExpressionSyntax)node;

            if (_expressions != null)
                result = _expressions.RunCallback(result, typed.Expressions, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ObjectCreationExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly ArgumentListPattern<TResult> _argumentList;
        private readonly InitializerExpressionPattern<TResult> _initializer;
        private readonly Func<TResult, ObjectCreationExpressionSyntax, TResult> _action;

        internal ObjectCreationExpressionPattern(TypePattern<TResult> type, ArgumentListPattern<TResult> argumentList, InitializerExpressionPattern<TResult> initializer, Func<TResult, ObjectCreationExpressionSyntax, TResult> action)
        {
            _type = type;
            _argumentList = argumentList;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ObjectCreationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ObjectCreationExpressionSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            if (_argumentList != null)
                result = _argumentList.RunCallback(result, typed.ArgumentList, semanticModel);
            if (_initializer != null)
                result = _initializer.RunCallback(result, typed.Initializer, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AnonymousObjectMemberDeclaratorPattern<TResult> : PatternNode<TResult>
    {
        private readonly NameEqualsPattern<TResult> _nameEquals;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, AnonymousObjectMemberDeclaratorSyntax, TResult> _action;

        internal AnonymousObjectMemberDeclaratorPattern(NameEqualsPattern<TResult> nameEquals, ExpressionPattern<TResult> expression, Func<TResult, AnonymousObjectMemberDeclaratorSyntax, TResult> action)
        {
            _nameEquals = nameEquals;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousObjectMemberDeclaratorSyntax typed))
                return false;

            if (_nameEquals != null && !_nameEquals.Test(typed.NameEquals, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AnonymousObjectMemberDeclaratorSyntax)node;

            if (_nameEquals != null)
                result = _nameEquals.RunCallback(result, typed.NameEquals, semanticModel);
            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AnonymousObjectCreationExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly NodeListPattern<AnonymousObjectMemberDeclaratorPattern<TResult>, TResult> _initializers;
        private readonly Func<TResult, AnonymousObjectCreationExpressionSyntax, TResult> _action;

        internal AnonymousObjectCreationExpressionPattern(NodeListPattern<AnonymousObjectMemberDeclaratorPattern<TResult>, TResult> initializers, Func<TResult, AnonymousObjectCreationExpressionSyntax, TResult> action)
        {
            _initializers = initializers;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousObjectCreationExpressionSyntax typed))
                return false;

            if (_initializers != null && !_initializers.Test(typed.Initializers, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AnonymousObjectCreationExpressionSyntax)node;

            if (_initializers != null)
                result = _initializers.RunCallback(result, typed.Initializers, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ArrayCreationExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ArrayTypePattern<TResult> _type;
        private readonly InitializerExpressionPattern<TResult> _initializer;
        private readonly Func<TResult, ArrayCreationExpressionSyntax, TResult> _action;

        internal ArrayCreationExpressionPattern(ArrayTypePattern<TResult> type, InitializerExpressionPattern<TResult> initializer, Func<TResult, ArrayCreationExpressionSyntax, TResult> action)
        {
            _type = type;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrayCreationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrayCreationExpressionSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            if (_initializer != null)
                result = _initializer.RunCallback(result, typed.Initializer, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ImplicitArrayCreationExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly InitializerExpressionPattern<TResult> _initializer;
        private readonly Func<TResult, ImplicitArrayCreationExpressionSyntax, TResult> _action;

        internal ImplicitArrayCreationExpressionPattern(InitializerExpressionPattern<TResult> initializer, Func<TResult, ImplicitArrayCreationExpressionSyntax, TResult> action)
        {
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ImplicitArrayCreationExpressionSyntax typed))
                return false;

            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ImplicitArrayCreationExpressionSyntax)node;

            if (_initializer != null)
                result = _initializer.RunCallback(result, typed.Initializer, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class StackAllocArrayCreationExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly Func<TResult, StackAllocArrayCreationExpressionSyntax, TResult> _action;

        internal StackAllocArrayCreationExpressionPattern(TypePattern<TResult> type, Func<TResult, StackAllocArrayCreationExpressionSyntax, TResult> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is StackAllocArrayCreationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (StackAllocArrayCreationExpressionSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class QueryClausePattern<TResult> : PatternNode<TResult>
    {

        internal QueryClausePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryClauseSyntax typed))
                return false;


            return true;
        }
    }

    public abstract partial class SelectOrGroupClausePattern<TResult> : PatternNode<TResult>
    {

        internal SelectOrGroupClausePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SelectOrGroupClauseSyntax typed))
                return false;


            return true;
        }
    }

    public partial class QueryExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly FromClausePattern<TResult> _fromClause;
        private readonly QueryBodyPattern<TResult> _body;
        private readonly Func<TResult, QueryExpressionSyntax, TResult> _action;

        internal QueryExpressionPattern(FromClausePattern<TResult> fromClause, QueryBodyPattern<TResult> body, Func<TResult, QueryExpressionSyntax, TResult> action)
        {
            _fromClause = fromClause;
            _body = body;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryExpressionSyntax typed))
                return false;

            if (_fromClause != null && !_fromClause.Test(typed.FromClause, semanticModel))
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QueryExpressionSyntax)node;

            if (_fromClause != null)
                result = _fromClause.RunCallback(result, typed.FromClause, semanticModel);
            if (_body != null)
                result = _body.RunCallback(result, typed.Body, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class QueryBodyPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<QueryClausePattern<TResult>, TResult> _clauses;
        private readonly SelectOrGroupClausePattern<TResult> _selectOrGroup;
        private readonly QueryContinuationPattern<TResult> _continuation;
        private readonly Func<TResult, QueryBodySyntax, TResult> _action;

        internal QueryBodyPattern(NodeListPattern<QueryClausePattern<TResult>, TResult> clauses, SelectOrGroupClausePattern<TResult> selectOrGroup, QueryContinuationPattern<TResult> continuation, Func<TResult, QueryBodySyntax, TResult> action)
        {
            _clauses = clauses;
            _selectOrGroup = selectOrGroup;
            _continuation = continuation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryBodySyntax typed))
                return false;

            if (_clauses != null && !_clauses.Test(typed.Clauses, semanticModel))
                return false;
            if (_selectOrGroup != null && !_selectOrGroup.Test(typed.SelectOrGroup, semanticModel))
                return false;
            if (_continuation != null && !_continuation.Test(typed.Continuation, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QueryBodySyntax)node;

            if (_clauses != null)
                result = _clauses.RunCallback(result, typed.Clauses, semanticModel);
            if (_selectOrGroup != null)
                result = _selectOrGroup.RunCallback(result, typed.SelectOrGroup, semanticModel);
            if (_continuation != null)
                result = _continuation.RunCallback(result, typed.Continuation, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class FromClausePattern<TResult> : QueryClausePattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly string _identifier;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, FromClauseSyntax, TResult> _action;

        internal FromClausePattern(TypePattern<TResult> type, string identifier, ExpressionPattern<TResult> expression, Func<TResult, FromClauseSyntax, TResult> action)
        {
            _type = type;
            _identifier = identifier;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FromClauseSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FromClauseSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class LetClausePattern<TResult> : QueryClausePattern<TResult>
    {
        private readonly string _identifier;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, LetClauseSyntax, TResult> _action;

        internal LetClausePattern(string identifier, ExpressionPattern<TResult> expression, Func<TResult, LetClauseSyntax, TResult> action)
        {
            _identifier = identifier;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LetClauseSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LetClauseSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class JoinClausePattern<TResult> : QueryClausePattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly string _identifier;
        private readonly ExpressionPattern<TResult> _inExpression;
        private readonly ExpressionPattern<TResult> _leftExpression;
        private readonly ExpressionPattern<TResult> _rightExpression;
        private readonly JoinIntoClausePattern<TResult> _into;
        private readonly Func<TResult, JoinClauseSyntax, TResult> _action;

        internal JoinClausePattern(TypePattern<TResult> type, string identifier, ExpressionPattern<TResult> inExpression, ExpressionPattern<TResult> leftExpression, ExpressionPattern<TResult> rightExpression, JoinIntoClausePattern<TResult> into, Func<TResult, JoinClauseSyntax, TResult> action)
        {
            _type = type;
            _identifier = identifier;
            _inExpression = inExpression;
            _leftExpression = leftExpression;
            _rightExpression = rightExpression;
            _into = into;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is JoinClauseSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_inExpression != null && !_inExpression.Test(typed.InExpression, semanticModel))
                return false;
            if (_leftExpression != null && !_leftExpression.Test(typed.LeftExpression, semanticModel))
                return false;
            if (_rightExpression != null && !_rightExpression.Test(typed.RightExpression, semanticModel))
                return false;
            if (_into != null && !_into.Test(typed.Into, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (JoinClauseSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            if (_inExpression != null)
                result = _inExpression.RunCallback(result, typed.InExpression, semanticModel);
            if (_leftExpression != null)
                result = _leftExpression.RunCallback(result, typed.LeftExpression, semanticModel);
            if (_rightExpression != null)
                result = _rightExpression.RunCallback(result, typed.RightExpression, semanticModel);
            if (_into != null)
                result = _into.RunCallback(result, typed.Into, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class JoinIntoClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly string _identifier;
        private readonly Func<TResult, JoinIntoClauseSyntax, TResult> _action;

        internal JoinIntoClausePattern(string identifier, Func<TResult, JoinIntoClauseSyntax, TResult> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is JoinIntoClauseSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (JoinIntoClauseSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class WhereClausePattern<TResult> : QueryClausePattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _condition;
        private readonly Func<TResult, WhereClauseSyntax, TResult> _action;

        internal WhereClausePattern(ExpressionPattern<TResult> condition, Func<TResult, WhereClauseSyntax, TResult> action)
        {
            _condition = condition;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is WhereClauseSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (WhereClauseSyntax)node;

            if (_condition != null)
                result = _condition.RunCallback(result, typed.Condition, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class OrderByClausePattern<TResult> : QueryClausePattern<TResult>
    {
        private readonly NodeListPattern<OrderingPattern<TResult>, TResult> _orderings;
        private readonly Func<TResult, OrderByClauseSyntax, TResult> _action;

        internal OrderByClausePattern(NodeListPattern<OrderingPattern<TResult>, TResult> orderings, Func<TResult, OrderByClauseSyntax, TResult> action)
        {
            _orderings = orderings;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OrderByClauseSyntax typed))
                return false;

            if (_orderings != null && !_orderings.Test(typed.Orderings, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OrderByClauseSyntax)node;

            if (_orderings != null)
                result = _orderings.RunCallback(result, typed.Orderings, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class OrderingPattern<TResult> : PatternNode<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, OrderingSyntax, TResult> _action;

        internal OrderingPattern(SyntaxKind kind, ExpressionPattern<TResult> expression, Func<TResult, OrderingSyntax, TResult> action)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OrderingSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OrderingSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class SelectClausePattern<TResult> : SelectOrGroupClausePattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, SelectClauseSyntax, TResult> _action;

        internal SelectClausePattern(ExpressionPattern<TResult> expression, Func<TResult, SelectClauseSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SelectClauseSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SelectClauseSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class GroupClausePattern<TResult> : SelectOrGroupClausePattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _groupExpression;
        private readonly ExpressionPattern<TResult> _byExpression;
        private readonly Func<TResult, GroupClauseSyntax, TResult> _action;

        internal GroupClausePattern(ExpressionPattern<TResult> groupExpression, ExpressionPattern<TResult> byExpression, Func<TResult, GroupClauseSyntax, TResult> action)
        {
            _groupExpression = groupExpression;
            _byExpression = byExpression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GroupClauseSyntax typed))
                return false;

            if (_groupExpression != null && !_groupExpression.Test(typed.GroupExpression, semanticModel))
                return false;
            if (_byExpression != null && !_byExpression.Test(typed.ByExpression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (GroupClauseSyntax)node;

            if (_groupExpression != null)
                result = _groupExpression.RunCallback(result, typed.GroupExpression, semanticModel);
            if (_byExpression != null)
                result = _byExpression.RunCallback(result, typed.ByExpression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class QueryContinuationPattern<TResult> : PatternNode<TResult>
    {
        private readonly string _identifier;
        private readonly QueryBodyPattern<TResult> _body;
        private readonly Func<TResult, QueryContinuationSyntax, TResult> _action;

        internal QueryContinuationPattern(string identifier, QueryBodyPattern<TResult> body, Func<TResult, QueryContinuationSyntax, TResult> action)
        {
            _identifier = identifier;
            _body = body;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryContinuationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QueryContinuationSyntax)node;

            if (_body != null)
                result = _body.RunCallback(result, typed.Body, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class OmittedArraySizeExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly Func<TResult, OmittedArraySizeExpressionSyntax, TResult> _action;

        internal OmittedArraySizeExpressionPattern(Func<TResult, OmittedArraySizeExpressionSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OmittedArraySizeExpressionSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OmittedArraySizeExpressionSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class InterpolatedStringExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly NodeListPattern<InterpolatedStringContentPattern<TResult>, TResult> _contents;
        private readonly Func<TResult, InterpolatedStringExpressionSyntax, TResult> _action;

        internal InterpolatedStringExpressionPattern(NodeListPattern<InterpolatedStringContentPattern<TResult>, TResult> contents, Func<TResult, InterpolatedStringExpressionSyntax, TResult> action)
        {
            _contents = contents;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolatedStringExpressionSyntax typed))
                return false;

            if (_contents != null && !_contents.Test(typed.Contents, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolatedStringExpressionSyntax)node;

            if (_contents != null)
                result = _contents.RunCallback(result, typed.Contents, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class IsPatternExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly PatternPattern<TResult> _pattern;
        private readonly Func<TResult, IsPatternExpressionSyntax, TResult> _action;

        internal IsPatternExpressionPattern(ExpressionPattern<TResult> expression, PatternPattern<TResult> pattern, Func<TResult, IsPatternExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _pattern = pattern;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IsPatternExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_pattern != null && !_pattern.Test(typed.Pattern, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (IsPatternExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_pattern != null)
                result = _pattern.RunCallback(result, typed.Pattern, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ThrowExpressionPattern<TResult> : ExpressionPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, ThrowExpressionSyntax, TResult> _action;

        internal ThrowExpressionPattern(ExpressionPattern<TResult> expression, Func<TResult, ThrowExpressionSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ThrowExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ThrowExpressionSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class WhenClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly ExpressionPattern<TResult> _condition;
        private readonly Func<TResult, WhenClauseSyntax, TResult> _action;

        internal WhenClausePattern(ExpressionPattern<TResult> condition, Func<TResult, WhenClauseSyntax, TResult> action)
        {
            _condition = condition;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is WhenClauseSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (WhenClauseSyntax)node;

            if (_condition != null)
                result = _condition.RunCallback(result, typed.Condition, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class PatternPattern<TResult> : PatternNode<TResult>
    {

        internal PatternPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PatternSyntax typed))
                return false;


            return true;
        }
    }

    public partial class DeclarationPatternPattern<TResult> : PatternPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly VariableDesignationPattern<TResult> _designation;
        private readonly Func<TResult, DeclarationPatternSyntax, TResult> _action;

        internal DeclarationPatternPattern(TypePattern<TResult> type, VariableDesignationPattern<TResult> designation, Func<TResult, DeclarationPatternSyntax, TResult> action)
        {
            _type = type;
            _designation = designation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DeclarationPatternSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_designation != null && !_designation.Test(typed.Designation, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DeclarationPatternSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            if (_designation != null)
                result = _designation.RunCallback(result, typed.Designation, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ConstantPatternPattern<TResult> : PatternPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, ConstantPatternSyntax, TResult> _action;

        internal ConstantPatternPattern(ExpressionPattern<TResult> expression, Func<TResult, ConstantPatternSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstantPatternSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConstantPatternSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class InterpolatedStringContentPattern<TResult> : PatternNode<TResult>
    {

        internal InterpolatedStringContentPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolatedStringContentSyntax typed))
                return false;


            return true;
        }
    }

    public partial class InterpolatedStringTextPattern<TResult> : InterpolatedStringContentPattern<TResult>
    {
        private readonly Func<TResult, InterpolatedStringTextSyntax, TResult> _action;

        internal InterpolatedStringTextPattern(Func<TResult, InterpolatedStringTextSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolatedStringTextSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolatedStringTextSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class InterpolationPattern<TResult> : InterpolatedStringContentPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly InterpolationAlignmentClausePattern<TResult> _alignmentClause;
        private readonly InterpolationFormatClausePattern<TResult> _formatClause;
        private readonly Func<TResult, InterpolationSyntax, TResult> _action;

        internal InterpolationPattern(ExpressionPattern<TResult> expression, InterpolationAlignmentClausePattern<TResult> alignmentClause, InterpolationFormatClausePattern<TResult> formatClause, Func<TResult, InterpolationSyntax, TResult> action)
        {
            _expression = expression;
            _alignmentClause = alignmentClause;
            _formatClause = formatClause;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolationSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_alignmentClause != null && !_alignmentClause.Test(typed.AlignmentClause, semanticModel))
                return false;
            if (_formatClause != null && !_formatClause.Test(typed.FormatClause, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolationSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_alignmentClause != null)
                result = _alignmentClause.RunCallback(result, typed.AlignmentClause, semanticModel);
            if (_formatClause != null)
                result = _formatClause.RunCallback(result, typed.FormatClause, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class InterpolationAlignmentClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly ExpressionPattern<TResult> _value;
        private readonly Func<TResult, InterpolationAlignmentClauseSyntax, TResult> _action;

        internal InterpolationAlignmentClausePattern(ExpressionPattern<TResult> value, Func<TResult, InterpolationAlignmentClauseSyntax, TResult> action)
        {
            _value = value;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolationAlignmentClauseSyntax typed))
                return false;

            if (_value != null && !_value.Test(typed.Value, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolationAlignmentClauseSyntax)node;

            if (_value != null)
                result = _value.RunCallback(result, typed.Value, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class InterpolationFormatClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly Func<TResult, InterpolationFormatClauseSyntax, TResult> _action;

        internal InterpolationFormatClausePattern(Func<TResult, InterpolationFormatClauseSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolationFormatClauseSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolationFormatClauseSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class GlobalStatementPattern<TResult> : MemberDeclarationPattern<TResult>
    {
        private readonly StatementPattern<TResult> _statement;
        private readonly Func<TResult, GlobalStatementSyntax, TResult> _action;

        internal GlobalStatementPattern(StatementPattern<TResult> statement, Func<TResult, GlobalStatementSyntax, TResult> action)
        {
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GlobalStatementSyntax typed))
                return false;

            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (GlobalStatementSyntax)node;

            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class StatementPattern<TResult> : PatternNode<TResult>
    {

        internal StatementPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is StatementSyntax typed))
                return false;


            return true;
        }
    }

    public partial class BlockPattern<TResult> : StatementPattern<TResult>
    {
        private readonly NodeListPattern<StatementPattern<TResult>, TResult> _statements;
        private readonly Func<TResult, BlockSyntax, TResult> _action;

        internal BlockPattern(NodeListPattern<StatementPattern<TResult>, TResult> statements, Func<TResult, BlockSyntax, TResult> action)
        {
            _statements = statements;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BlockSyntax typed))
                return false;

            if (_statements != null && !_statements.Test(typed.Statements, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BlockSyntax)node;

            if (_statements != null)
                result = _statements.RunCallback(result, typed.Statements, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class LocalFunctionStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern<TResult> _returnType;
        private readonly string _identifier;
        private readonly TypeParameterListPattern<TResult> _typeParameterList;
        private readonly ParameterListPattern<TResult> _parameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> _constraintClauses;
        private readonly BlockPattern<TResult> _body;
        private readonly ArrowExpressionClausePattern<TResult> _expressionBody;
        private readonly Func<TResult, LocalFunctionStatementSyntax, TResult> _action;

        internal LocalFunctionStatementPattern(TokenListPattern modifiers, TypePattern<TResult> returnType, string identifier, TypeParameterListPattern<TResult> typeParameterList, ParameterListPattern<TResult> parameterList, NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> constraintClauses, BlockPattern<TResult> body, ArrowExpressionClausePattern<TResult> expressionBody, Func<TResult, LocalFunctionStatementSyntax, TResult> action)
        {
            _modifiers = modifiers;
            _returnType = returnType;
            _identifier = identifier;
            _typeParameterList = typeParameterList;
            _parameterList = parameterList;
            _constraintClauses = constraintClauses;
            _body = body;
            _expressionBody = expressionBody;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LocalFunctionStatementSyntax typed))
                return false;

            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;
            if (_expressionBody != null && !_expressionBody.Test(typed.ExpressionBody, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LocalFunctionStatementSyntax)node;

            if (_returnType != null)
                result = _returnType.RunCallback(result, typed.ReturnType, semanticModel);
            if (_typeParameterList != null)
                result = _typeParameterList.RunCallback(result, typed.TypeParameterList, semanticModel);
            if (_parameterList != null)
                result = _parameterList.RunCallback(result, typed.ParameterList, semanticModel);
            if (_constraintClauses != null)
                result = _constraintClauses.RunCallback(result, typed.ConstraintClauses, semanticModel);
            if (_body != null)
                result = _body.RunCallback(result, typed.Body, semanticModel);
            if (_expressionBody != null)
                result = _expressionBody.RunCallback(result, typed.ExpressionBody, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class LocalDeclarationStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly TokenListPattern _modifiers;
        private readonly VariableDeclarationPattern<TResult> _declaration;
        private readonly Func<TResult, LocalDeclarationStatementSyntax, TResult> _action;

        internal LocalDeclarationStatementPattern(TokenListPattern modifiers, VariableDeclarationPattern<TResult> declaration, Func<TResult, LocalDeclarationStatementSyntax, TResult> action)
        {
            _modifiers = modifiers;
            _declaration = declaration;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LocalDeclarationStatementSyntax typed))
                return false;

            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LocalDeclarationStatementSyntax)node;

            if (_declaration != null)
                result = _declaration.RunCallback(result, typed.Declaration, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class VariableDeclarationPattern<TResult> : PatternNode<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly NodeListPattern<VariableDeclaratorPattern<TResult>, TResult> _variables;
        private readonly Func<TResult, VariableDeclarationSyntax, TResult> _action;

        internal VariableDeclarationPattern(TypePattern<TResult> type, NodeListPattern<VariableDeclaratorPattern<TResult>, TResult> variables, Func<TResult, VariableDeclarationSyntax, TResult> action)
        {
            _type = type;
            _variables = variables;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is VariableDeclarationSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_variables != null && !_variables.Test(typed.Variables, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (VariableDeclarationSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            if (_variables != null)
                result = _variables.RunCallback(result, typed.Variables, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class VariableDeclaratorPattern<TResult> : PatternNode<TResult>
    {
        private readonly string _identifier;
        private readonly BracketedArgumentListPattern<TResult> _argumentList;
        private readonly EqualsValueClausePattern<TResult> _initializer;
        private readonly Func<TResult, VariableDeclaratorSyntax, TResult> _action;

        internal VariableDeclaratorPattern(string identifier, BracketedArgumentListPattern<TResult> argumentList, EqualsValueClausePattern<TResult> initializer, Func<TResult, VariableDeclaratorSyntax, TResult> action)
        {
            _identifier = identifier;
            _argumentList = argumentList;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is VariableDeclaratorSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (VariableDeclaratorSyntax)node;

            if (_argumentList != null)
                result = _argumentList.RunCallback(result, typed.ArgumentList, semanticModel);
            if (_initializer != null)
                result = _initializer.RunCallback(result, typed.Initializer, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class EqualsValueClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly ExpressionPattern<TResult> _value;
        private readonly Func<TResult, EqualsValueClauseSyntax, TResult> _action;

        internal EqualsValueClausePattern(ExpressionPattern<TResult> value, Func<TResult, EqualsValueClauseSyntax, TResult> action)
        {
            _value = value;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EqualsValueClauseSyntax typed))
                return false;

            if (_value != null && !_value.Test(typed.Value, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (EqualsValueClauseSyntax)node;

            if (_value != null)
                result = _value.RunCallback(result, typed.Value, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class VariableDesignationPattern<TResult> : PatternNode<TResult>
    {

        internal VariableDesignationPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is VariableDesignationSyntax typed))
                return false;


            return true;
        }
    }

    public partial class SingleVariableDesignationPattern<TResult> : VariableDesignationPattern<TResult>
    {
        private readonly string _identifier;
        private readonly Func<TResult, SingleVariableDesignationSyntax, TResult> _action;

        internal SingleVariableDesignationPattern(string identifier, Func<TResult, SingleVariableDesignationSyntax, TResult> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SingleVariableDesignationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SingleVariableDesignationSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class DiscardDesignationPattern<TResult> : VariableDesignationPattern<TResult>
    {
        private readonly Func<TResult, DiscardDesignationSyntax, TResult> _action;

        internal DiscardDesignationPattern(Func<TResult, DiscardDesignationSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DiscardDesignationSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DiscardDesignationSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ParenthesizedVariableDesignationPattern<TResult> : VariableDesignationPattern<TResult>
    {
        private readonly NodeListPattern<VariableDesignationPattern<TResult>, TResult> _variables;
        private readonly Func<TResult, ParenthesizedVariableDesignationSyntax, TResult> _action;

        internal ParenthesizedVariableDesignationPattern(NodeListPattern<VariableDesignationPattern<TResult>, TResult> variables, Func<TResult, ParenthesizedVariableDesignationSyntax, TResult> action)
        {
            _variables = variables;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParenthesizedVariableDesignationSyntax typed))
                return false;

            if (_variables != null && !_variables.Test(typed.Variables, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParenthesizedVariableDesignationSyntax)node;

            if (_variables != null)
                result = _variables.RunCallback(result, typed.Variables, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ExpressionStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, ExpressionStatementSyntax, TResult> _action;

        internal ExpressionStatementPattern(ExpressionPattern<TResult> expression, Func<TResult, ExpressionStatementSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExpressionStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ExpressionStatementSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class EmptyStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly Func<TResult, EmptyStatementSyntax, TResult> _action;

        internal EmptyStatementPattern(Func<TResult, EmptyStatementSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EmptyStatementSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (EmptyStatementSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class LabeledStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly string _identifier;
        private readonly StatementPattern<TResult> _statement;
        private readonly Func<TResult, LabeledStatementSyntax, TResult> _action;

        internal LabeledStatementPattern(string identifier, StatementPattern<TResult> statement, Func<TResult, LabeledStatementSyntax, TResult> action)
        {
            _identifier = identifier;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LabeledStatementSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LabeledStatementSyntax)node;

            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class GotoStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, GotoStatementSyntax, TResult> _action;

        internal GotoStatementPattern(SyntaxKind kind, ExpressionPattern<TResult> expression, Func<TResult, GotoStatementSyntax, TResult> action)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GotoStatementSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (GotoStatementSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class BreakStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly Func<TResult, BreakStatementSyntax, TResult> _action;

        internal BreakStatementPattern(Func<TResult, BreakStatementSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BreakStatementSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BreakStatementSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ContinueStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly Func<TResult, ContinueStatementSyntax, TResult> _action;

        internal ContinueStatementPattern(Func<TResult, ContinueStatementSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ContinueStatementSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ContinueStatementSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ReturnStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, ReturnStatementSyntax, TResult> _action;

        internal ReturnStatementPattern(ExpressionPattern<TResult> expression, Func<TResult, ReturnStatementSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ReturnStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ReturnStatementSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ThrowStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, ThrowStatementSyntax, TResult> _action;

        internal ThrowStatementPattern(ExpressionPattern<TResult> expression, Func<TResult, ThrowStatementSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ThrowStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ThrowStatementSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class YieldStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, YieldStatementSyntax, TResult> _action;

        internal YieldStatementPattern(SyntaxKind kind, ExpressionPattern<TResult> expression, Func<TResult, YieldStatementSyntax, TResult> action)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is YieldStatementSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (YieldStatementSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class WhileStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _condition;
        private readonly StatementPattern<TResult> _statement;
        private readonly Func<TResult, WhileStatementSyntax, TResult> _action;

        internal WhileStatementPattern(ExpressionPattern<TResult> condition, StatementPattern<TResult> statement, Func<TResult, WhileStatementSyntax, TResult> action)
        {
            _condition = condition;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is WhileStatementSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (WhileStatementSyntax)node;

            if (_condition != null)
                result = _condition.RunCallback(result, typed.Condition, semanticModel);
            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class DoStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly StatementPattern<TResult> _statement;
        private readonly ExpressionPattern<TResult> _condition;
        private readonly Func<TResult, DoStatementSyntax, TResult> _action;

        internal DoStatementPattern(StatementPattern<TResult> statement, ExpressionPattern<TResult> condition, Func<TResult, DoStatementSyntax, TResult> action)
        {
            _statement = statement;
            _condition = condition;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DoStatementSyntax typed))
                return false;

            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;
            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DoStatementSyntax)node;

            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);
            if (_condition != null)
                result = _condition.RunCallback(result, typed.Condition, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ForStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly VariableDeclarationPattern<TResult> _declaration;
        private readonly NodeListPattern<ExpressionPattern<TResult>, TResult> _initializers;
        private readonly ExpressionPattern<TResult> _condition;
        private readonly NodeListPattern<ExpressionPattern<TResult>, TResult> _incrementors;
        private readonly StatementPattern<TResult> _statement;
        private readonly Func<TResult, ForStatementSyntax, TResult> _action;

        internal ForStatementPattern(VariableDeclarationPattern<TResult> declaration, NodeListPattern<ExpressionPattern<TResult>, TResult> initializers, ExpressionPattern<TResult> condition, NodeListPattern<ExpressionPattern<TResult>, TResult> incrementors, StatementPattern<TResult> statement, Func<TResult, ForStatementSyntax, TResult> action)
        {
            _declaration = declaration;
            _initializers = initializers;
            _condition = condition;
            _incrementors = incrementors;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ForStatementSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;
            if (_initializers != null && !_initializers.Test(typed.Initializers, semanticModel))
                return false;
            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_incrementors != null && !_incrementors.Test(typed.Incrementors, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ForStatementSyntax)node;

            if (_declaration != null)
                result = _declaration.RunCallback(result, typed.Declaration, semanticModel);
            if (_initializers != null)
                result = _initializers.RunCallback(result, typed.Initializers, semanticModel);
            if (_condition != null)
                result = _condition.RunCallback(result, typed.Condition, semanticModel);
            if (_incrementors != null)
                result = _incrementors.RunCallback(result, typed.Incrementors, semanticModel);
            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class CommonForEachStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly StatementPattern<TResult> _statement;

        internal CommonForEachStatementPattern(ExpressionPattern<TResult> expression, StatementPattern<TResult> statement)
        {
            _expression = expression;
            _statement = statement;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CommonForEachStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CommonForEachStatementSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);
            return result;
        }
    }

    public partial class ForEachStatementPattern<TResult> : CommonForEachStatementPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly string _identifier;
        private readonly Func<TResult, ForEachStatementSyntax, TResult> _action;

        internal ForEachStatementPattern(ExpressionPattern<TResult> expression, StatementPattern<TResult> statement, TypePattern<TResult> type, string identifier, Func<TResult, ForEachStatementSyntax, TResult> action)
            : base(expression, statement)
        {
            _type = type;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ForEachStatementSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (ForEachStatementSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ForEachVariableStatementPattern<TResult> : CommonForEachStatementPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _variable;
        private readonly Func<TResult, ForEachVariableStatementSyntax, TResult> _action;

        internal ForEachVariableStatementPattern(ExpressionPattern<TResult> expression, StatementPattern<TResult> statement, ExpressionPattern<TResult> variable, Func<TResult, ForEachVariableStatementSyntax, TResult> action)
            : base(expression, statement)
        {
            _variable = variable;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ForEachVariableStatementSyntax typed))
                return false;

            if (_variable != null && !_variable.Test(typed.Variable, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (ForEachVariableStatementSyntax)node;

            if (_variable != null)
                result = _variable.RunCallback(result, typed.Variable, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class UsingStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly VariableDeclarationPattern<TResult> _declaration;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly StatementPattern<TResult> _statement;
        private readonly Func<TResult, UsingStatementSyntax, TResult> _action;

        internal UsingStatementPattern(VariableDeclarationPattern<TResult> declaration, ExpressionPattern<TResult> expression, StatementPattern<TResult> statement, Func<TResult, UsingStatementSyntax, TResult> action)
        {
            _declaration = declaration;
            _expression = expression;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is UsingStatementSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (UsingStatementSyntax)node;

            if (_declaration != null)
                result = _declaration.RunCallback(result, typed.Declaration, semanticModel);
            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class FixedStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly VariableDeclarationPattern<TResult> _declaration;
        private readonly StatementPattern<TResult> _statement;
        private readonly Func<TResult, FixedStatementSyntax, TResult> _action;

        internal FixedStatementPattern(VariableDeclarationPattern<TResult> declaration, StatementPattern<TResult> statement, Func<TResult, FixedStatementSyntax, TResult> action)
        {
            _declaration = declaration;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FixedStatementSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FixedStatementSyntax)node;

            if (_declaration != null)
                result = _declaration.RunCallback(result, typed.Declaration, semanticModel);
            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class CheckedStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly BlockPattern<TResult> _block;
        private readonly Func<TResult, CheckedStatementSyntax, TResult> _action;

        internal CheckedStatementPattern(SyntaxKind kind, BlockPattern<TResult> block, Func<TResult, CheckedStatementSyntax, TResult> action)
        {
            _kind = kind;
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CheckedStatementSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CheckedStatementSyntax)node;

            if (_block != null)
                result = _block.RunCallback(result, typed.Block, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class UnsafeStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly BlockPattern<TResult> _block;
        private readonly Func<TResult, UnsafeStatementSyntax, TResult> _action;

        internal UnsafeStatementPattern(BlockPattern<TResult> block, Func<TResult, UnsafeStatementSyntax, TResult> action)
        {
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is UnsafeStatementSyntax typed))
                return false;

            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (UnsafeStatementSyntax)node;

            if (_block != null)
                result = _block.RunCallback(result, typed.Block, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class LockStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly StatementPattern<TResult> _statement;
        private readonly Func<TResult, LockStatementSyntax, TResult> _action;

        internal LockStatementPattern(ExpressionPattern<TResult> expression, StatementPattern<TResult> statement, Func<TResult, LockStatementSyntax, TResult> action)
        {
            _expression = expression;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LockStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LockStatementSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class IfStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _condition;
        private readonly StatementPattern<TResult> _statement;
        private readonly ElseClausePattern<TResult> _else;
        private readonly Func<TResult, IfStatementSyntax, TResult> _action;

        internal IfStatementPattern(ExpressionPattern<TResult> condition, StatementPattern<TResult> statement, ElseClausePattern<TResult> @else, Func<TResult, IfStatementSyntax, TResult> action)
        {
            _condition = condition;
            _statement = statement;
            _else = @else;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IfStatementSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;
            if (_else != null && !_else.Test(typed.Else, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (IfStatementSyntax)node;

            if (_condition != null)
                result = _condition.RunCallback(result, typed.Condition, semanticModel);
            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);
            if (_else != null)
                result = _else.RunCallback(result, typed.Else, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ElseClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly StatementPattern<TResult> _statement;
        private readonly Func<TResult, ElseClauseSyntax, TResult> _action;

        internal ElseClausePattern(StatementPattern<TResult> statement, Func<TResult, ElseClauseSyntax, TResult> action)
        {
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ElseClauseSyntax typed))
                return false;

            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ElseClauseSyntax)node;

            if (_statement != null)
                result = _statement.RunCallback(result, typed.Statement, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class SwitchStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly NodeListPattern<SwitchSectionPattern<TResult>, TResult> _sections;
        private readonly Func<TResult, SwitchStatementSyntax, TResult> _action;

        internal SwitchStatementPattern(ExpressionPattern<TResult> expression, NodeListPattern<SwitchSectionPattern<TResult>, TResult> sections, Func<TResult, SwitchStatementSyntax, TResult> action)
        {
            _expression = expression;
            _sections = sections;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_sections != null && !_sections.Test(typed.Sections, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SwitchStatementSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);
            if (_sections != null)
                result = _sections.RunCallback(result, typed.Sections, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class SwitchSectionPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<SwitchLabelPattern<TResult>, TResult> _labels;
        private readonly NodeListPattern<StatementPattern<TResult>, TResult> _statements;
        private readonly Func<TResult, SwitchSectionSyntax, TResult> _action;

        internal SwitchSectionPattern(NodeListPattern<SwitchLabelPattern<TResult>, TResult> labels, NodeListPattern<StatementPattern<TResult>, TResult> statements, Func<TResult, SwitchSectionSyntax, TResult> action)
        {
            _labels = labels;
            _statements = statements;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchSectionSyntax typed))
                return false;

            if (_labels != null && !_labels.Test(typed.Labels, semanticModel))
                return false;
            if (_statements != null && !_statements.Test(typed.Statements, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SwitchSectionSyntax)node;

            if (_labels != null)
                result = _labels.RunCallback(result, typed.Labels, semanticModel);
            if (_statements != null)
                result = _statements.RunCallback(result, typed.Statements, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class SwitchLabelPattern<TResult> : PatternNode<TResult>
    {

        internal SwitchLabelPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchLabelSyntax typed))
                return false;


            return true;
        }
    }

    public partial class CasePatternSwitchLabelPattern<TResult> : SwitchLabelPattern<TResult>
    {
        private readonly PatternPattern<TResult> _pattern;
        private readonly WhenClausePattern<TResult> _whenClause;
        private readonly Func<TResult, CasePatternSwitchLabelSyntax, TResult> _action;

        internal CasePatternSwitchLabelPattern(PatternPattern<TResult> pattern, WhenClausePattern<TResult> whenClause, Func<TResult, CasePatternSwitchLabelSyntax, TResult> action)
        {
            _pattern = pattern;
            _whenClause = whenClause;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CasePatternSwitchLabelSyntax typed))
                return false;

            if (_pattern != null && !_pattern.Test(typed.Pattern, semanticModel))
                return false;
            if (_whenClause != null && !_whenClause.Test(typed.WhenClause, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CasePatternSwitchLabelSyntax)node;

            if (_pattern != null)
                result = _pattern.RunCallback(result, typed.Pattern, semanticModel);
            if (_whenClause != null)
                result = _whenClause.RunCallback(result, typed.WhenClause, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class CaseSwitchLabelPattern<TResult> : SwitchLabelPattern<TResult>
    {
        private readonly ExpressionPattern<TResult> _value;
        private readonly Func<TResult, CaseSwitchLabelSyntax, TResult> _action;

        internal CaseSwitchLabelPattern(ExpressionPattern<TResult> value, Func<TResult, CaseSwitchLabelSyntax, TResult> action)
        {
            _value = value;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CaseSwitchLabelSyntax typed))
                return false;

            if (_value != null && !_value.Test(typed.Value, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CaseSwitchLabelSyntax)node;

            if (_value != null)
                result = _value.RunCallback(result, typed.Value, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class DefaultSwitchLabelPattern<TResult> : SwitchLabelPattern<TResult>
    {
        private readonly Func<TResult, DefaultSwitchLabelSyntax, TResult> _action;

        internal DefaultSwitchLabelPattern(Func<TResult, DefaultSwitchLabelSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DefaultSwitchLabelSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DefaultSwitchLabelSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class TryStatementPattern<TResult> : StatementPattern<TResult>
    {
        private readonly BlockPattern<TResult> _block;
        private readonly NodeListPattern<CatchClausePattern<TResult>, TResult> _catches;
        private readonly FinallyClausePattern<TResult> _finally;
        private readonly Func<TResult, TryStatementSyntax, TResult> _action;

        internal TryStatementPattern(BlockPattern<TResult> block, NodeListPattern<CatchClausePattern<TResult>, TResult> catches, FinallyClausePattern<TResult> @finally, Func<TResult, TryStatementSyntax, TResult> action)
        {
            _block = block;
            _catches = catches;
            _finally = @finally;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TryStatementSyntax typed))
                return false;

            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;
            if (_catches != null && !_catches.Test(typed.Catches, semanticModel))
                return false;
            if (_finally != null && !_finally.Test(typed.Finally, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TryStatementSyntax)node;

            if (_block != null)
                result = _block.RunCallback(result, typed.Block, semanticModel);
            if (_catches != null)
                result = _catches.RunCallback(result, typed.Catches, semanticModel);
            if (_finally != null)
                result = _finally.RunCallback(result, typed.Finally, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class CatchClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly CatchDeclarationPattern<TResult> _declaration;
        private readonly CatchFilterClausePattern<TResult> _filter;
        private readonly BlockPattern<TResult> _block;
        private readonly Func<TResult, CatchClauseSyntax, TResult> _action;

        internal CatchClausePattern(CatchDeclarationPattern<TResult> declaration, CatchFilterClausePattern<TResult> filter, BlockPattern<TResult> block, Func<TResult, CatchClauseSyntax, TResult> action)
        {
            _declaration = declaration;
            _filter = filter;
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CatchClauseSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;
            if (_filter != null && !_filter.Test(typed.Filter, semanticModel))
                return false;
            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CatchClauseSyntax)node;

            if (_declaration != null)
                result = _declaration.RunCallback(result, typed.Declaration, semanticModel);
            if (_filter != null)
                result = _filter.RunCallback(result, typed.Filter, semanticModel);
            if (_block != null)
                result = _block.RunCallback(result, typed.Block, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class CatchDeclarationPattern<TResult> : PatternNode<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly string _identifier;
        private readonly Func<TResult, CatchDeclarationSyntax, TResult> _action;

        internal CatchDeclarationPattern(TypePattern<TResult> type, string identifier, Func<TResult, CatchDeclarationSyntax, TResult> action)
        {
            _type = type;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CatchDeclarationSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CatchDeclarationSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class CatchFilterClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly ExpressionPattern<TResult> _filterExpression;
        private readonly Func<TResult, CatchFilterClauseSyntax, TResult> _action;

        internal CatchFilterClausePattern(ExpressionPattern<TResult> filterExpression, Func<TResult, CatchFilterClauseSyntax, TResult> action)
        {
            _filterExpression = filterExpression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CatchFilterClauseSyntax typed))
                return false;

            if (_filterExpression != null && !_filterExpression.Test(typed.FilterExpression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CatchFilterClauseSyntax)node;

            if (_filterExpression != null)
                result = _filterExpression.RunCallback(result, typed.FilterExpression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class FinallyClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly BlockPattern<TResult> _block;
        private readonly Func<TResult, FinallyClauseSyntax, TResult> _action;

        internal FinallyClausePattern(BlockPattern<TResult> block, Func<TResult, FinallyClauseSyntax, TResult> action)
        {
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FinallyClauseSyntax typed))
                return false;

            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FinallyClauseSyntax)node;

            if (_block != null)
                result = _block.RunCallback(result, typed.Block, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class CompilationUnitPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<ExternAliasDirectivePattern<TResult>, TResult> _externs;
        private readonly NodeListPattern<UsingDirectivePattern<TResult>, TResult> _usings;
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly NodeListPattern<MemberDeclarationPattern<TResult>, TResult> _members;
        private readonly Func<TResult, CompilationUnitSyntax, TResult> _action;

        internal CompilationUnitPattern(NodeListPattern<ExternAliasDirectivePattern<TResult>, TResult> externs, NodeListPattern<UsingDirectivePattern<TResult>, TResult> usings, NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, NodeListPattern<MemberDeclarationPattern<TResult>, TResult> members, Func<TResult, CompilationUnitSyntax, TResult> action)
        {
            _externs = externs;
            _usings = usings;
            _attributeLists = attributeLists;
            _members = members;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CompilationUnitSyntax typed))
                return false;

            if (_externs != null && !_externs.Test(typed.Externs, semanticModel))
                return false;
            if (_usings != null && !_usings.Test(typed.Usings, semanticModel))
                return false;
            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CompilationUnitSyntax)node;

            if (_externs != null)
                result = _externs.RunCallback(result, typed.Externs, semanticModel);
            if (_usings != null)
                result = _usings.RunCallback(result, typed.Usings, semanticModel);
            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);
            if (_members != null)
                result = _members.RunCallback(result, typed.Members, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ExternAliasDirectivePattern<TResult> : PatternNode<TResult>
    {
        private readonly string _identifier;
        private readonly Func<TResult, ExternAliasDirectiveSyntax, TResult> _action;

        internal ExternAliasDirectivePattern(string identifier, Func<TResult, ExternAliasDirectiveSyntax, TResult> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExternAliasDirectiveSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ExternAliasDirectiveSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class UsingDirectivePattern<TResult> : PatternNode<TResult>
    {
        private readonly NameEqualsPattern<TResult> _alias;
        private readonly NamePattern<TResult> _name;
        private readonly Func<TResult, UsingDirectiveSyntax, TResult> _action;

        internal UsingDirectivePattern(NameEqualsPattern<TResult> alias, NamePattern<TResult> name, Func<TResult, UsingDirectiveSyntax, TResult> action)
        {
            _alias = alias;
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is UsingDirectiveSyntax typed))
                return false;

            if (_alias != null && !_alias.Test(typed.Alias, semanticModel))
                return false;
            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (UsingDirectiveSyntax)node;

            if (_alias != null)
                result = _alias.RunCallback(result, typed.Alias, semanticModel);
            if (_name != null)
                result = _name.RunCallback(result, typed.Name, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class MemberDeclarationPattern<TResult> : PatternNode<TResult>
    {

        internal MemberDeclarationPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MemberDeclarationSyntax typed))
                return false;


            return true;
        }
    }

    public partial class NamespaceDeclarationPattern<TResult> : MemberDeclarationPattern<TResult>
    {
        private readonly NamePattern<TResult> _name;
        private readonly NodeListPattern<ExternAliasDirectivePattern<TResult>, TResult> _externs;
        private readonly NodeListPattern<UsingDirectivePattern<TResult>, TResult> _usings;
        private readonly NodeListPattern<MemberDeclarationPattern<TResult>, TResult> _members;
        private readonly Func<TResult, NamespaceDeclarationSyntax, TResult> _action;

        internal NamespaceDeclarationPattern(NamePattern<TResult> name, NodeListPattern<ExternAliasDirectivePattern<TResult>, TResult> externs, NodeListPattern<UsingDirectivePattern<TResult>, TResult> usings, NodeListPattern<MemberDeclarationPattern<TResult>, TResult> members, Func<TResult, NamespaceDeclarationSyntax, TResult> action)
        {
            _name = name;
            _externs = externs;
            _usings = usings;
            _members = members;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NamespaceDeclarationSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;
            if (_externs != null && !_externs.Test(typed.Externs, semanticModel))
                return false;
            if (_usings != null && !_usings.Test(typed.Usings, semanticModel))
                return false;
            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NamespaceDeclarationSyntax)node;

            if (_name != null)
                result = _name.RunCallback(result, typed.Name, semanticModel);
            if (_externs != null)
                result = _externs.RunCallback(result, typed.Externs, semanticModel);
            if (_usings != null)
                result = _usings.RunCallback(result, typed.Usings, semanticModel);
            if (_members != null)
                result = _members.RunCallback(result, typed.Members, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AttributeListPattern<TResult> : PatternNode<TResult>
    {
        private readonly AttributeTargetSpecifierPattern<TResult> _target;
        private readonly NodeListPattern<AttributePattern<TResult>, TResult> _attributes;
        private readonly Func<TResult, AttributeListSyntax, TResult> _action;

        internal AttributeListPattern(AttributeTargetSpecifierPattern<TResult> target, NodeListPattern<AttributePattern<TResult>, TResult> attributes, Func<TResult, AttributeListSyntax, TResult> action)
        {
            _target = target;
            _attributes = attributes;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeListSyntax typed))
                return false;

            if (_target != null && !_target.Test(typed.Target, semanticModel))
                return false;
            if (_attributes != null && !_attributes.Test(typed.Attributes, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeListSyntax)node;

            if (_target != null)
                result = _target.RunCallback(result, typed.Target, semanticModel);
            if (_attributes != null)
                result = _attributes.RunCallback(result, typed.Attributes, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AttributeTargetSpecifierPattern<TResult> : PatternNode<TResult>
    {
        private readonly string _identifier;
        private readonly Func<TResult, AttributeTargetSpecifierSyntax, TResult> _action;

        internal AttributeTargetSpecifierPattern(string identifier, Func<TResult, AttributeTargetSpecifierSyntax, TResult> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeTargetSpecifierSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeTargetSpecifierSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AttributePattern<TResult> : PatternNode<TResult>
    {
        private readonly NamePattern<TResult> _name;
        private readonly AttributeArgumentListPattern<TResult> _argumentList;
        private readonly Func<TResult, AttributeSyntax, TResult> _action;

        internal AttributePattern(NamePattern<TResult> name, AttributeArgumentListPattern<TResult> argumentList, Func<TResult, AttributeSyntax, TResult> action)
        {
            _name = name;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeSyntax)node;

            if (_name != null)
                result = _name.RunCallback(result, typed.Name, semanticModel);
            if (_argumentList != null)
                result = _argumentList.RunCallback(result, typed.ArgumentList, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AttributeArgumentListPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<AttributeArgumentPattern<TResult>, TResult> _arguments;
        private readonly Func<TResult, AttributeArgumentListSyntax, TResult> _action;

        internal AttributeArgumentListPattern(NodeListPattern<AttributeArgumentPattern<TResult>, TResult> arguments, Func<TResult, AttributeArgumentListSyntax, TResult> action)
        {
            _arguments = arguments;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeArgumentListSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeArgumentListSyntax)node;

            if (_arguments != null)
                result = _arguments.RunCallback(result, typed.Arguments, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AttributeArgumentPattern<TResult> : PatternNode<TResult>
    {
        private readonly NameEqualsPattern<TResult> _nameEquals;
        private readonly NameColonPattern<TResult> _nameColon;
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, AttributeArgumentSyntax, TResult> _action;

        internal AttributeArgumentPattern(NameEqualsPattern<TResult> nameEquals, NameColonPattern<TResult> nameColon, ExpressionPattern<TResult> expression, Func<TResult, AttributeArgumentSyntax, TResult> action)
        {
            _nameEquals = nameEquals;
            _nameColon = nameColon;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeArgumentSyntax typed))
                return false;

            if (_nameEquals != null && !_nameEquals.Test(typed.NameEquals, semanticModel))
                return false;
            if (_nameColon != null && !_nameColon.Test(typed.NameColon, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeArgumentSyntax)node;

            if (_nameEquals != null)
                result = _nameEquals.RunCallback(result, typed.NameEquals, semanticModel);
            if (_nameColon != null)
                result = _nameColon.RunCallback(result, typed.NameColon, semanticModel);
            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class NameEqualsPattern<TResult> : PatternNode<TResult>
    {
        private readonly IdentifierNamePattern<TResult> _name;
        private readonly Func<TResult, NameEqualsSyntax, TResult> _action;

        internal NameEqualsPattern(IdentifierNamePattern<TResult> name, Func<TResult, NameEqualsSyntax, TResult> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NameEqualsSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NameEqualsSyntax)node;

            if (_name != null)
                result = _name.RunCallback(result, typed.Name, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class TypeParameterListPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<TypeParameterPattern<TResult>, TResult> _parameters;
        private readonly Func<TResult, TypeParameterListSyntax, TResult> _action;

        internal TypeParameterListPattern(NodeListPattern<TypeParameterPattern<TResult>, TResult> parameters, Func<TResult, TypeParameterListSyntax, TResult> action)
        {
            _parameters = parameters;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterListSyntax typed))
                return false;

            if (_parameters != null && !_parameters.Test(typed.Parameters, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeParameterListSyntax)node;

            if (_parameters != null)
                result = _parameters.RunCallback(result, typed.Parameters, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class TypeParameterPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly string _identifier;
        private readonly Func<TResult, TypeParameterSyntax, TResult> _action;

        internal TypeParameterPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, string identifier, Func<TResult, TypeParameterSyntax, TResult> action)
        {
            _attributeLists = attributeLists;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeParameterSyntax)node;

            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class BaseTypeDeclarationPattern<TResult> : MemberDeclarationPattern<TResult>
    {
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly string _identifier;
        private readonly BaseListPattern<TResult> _baseList;

        internal BaseTypeDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern<TResult> baseList)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _identifier = identifier;
            _baseList = baseList;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseTypeDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_baseList != null && !_baseList.Test(typed.BaseList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseTypeDeclarationSyntax)node;

            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);
            if (_baseList != null)
                result = _baseList.RunCallback(result, typed.BaseList, semanticModel);
            return result;
        }
    }

    public abstract partial class TypeDeclarationPattern<TResult> : BaseTypeDeclarationPattern<TResult>
    {
        private readonly TypeParameterListPattern<TResult> _typeParameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> _constraintClauses;
        private readonly NodeListPattern<MemberDeclarationPattern<TResult>, TResult> _members;

        internal TypeDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern<TResult> baseList, TypeParameterListPattern<TResult> typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> constraintClauses, NodeListPattern<MemberDeclarationPattern<TResult>, TResult> members)
            : base(attributeLists, modifiers, identifier, baseList)
        {
            _typeParameterList = typeParameterList;
            _constraintClauses = constraintClauses;
            _members = members;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeDeclarationSyntax typed))
                return false;

            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;
            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (TypeDeclarationSyntax)node;

            if (_typeParameterList != null)
                result = _typeParameterList.RunCallback(result, typed.TypeParameterList, semanticModel);
            if (_constraintClauses != null)
                result = _constraintClauses.RunCallback(result, typed.ConstraintClauses, semanticModel);
            if (_members != null)
                result = _members.RunCallback(result, typed.Members, semanticModel);
            return result;
        }
    }

    public partial class ClassDeclarationPattern<TResult> : TypeDeclarationPattern<TResult>
    {
        private readonly Func<TResult, ClassDeclarationSyntax, TResult> _action;

        internal ClassDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern<TResult> baseList, TypeParameterListPattern<TResult> typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> constraintClauses, NodeListPattern<MemberDeclarationPattern<TResult>, TResult> members, Func<TResult, ClassDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, identifier, baseList, typeParameterList, constraintClauses, members)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ClassDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (ClassDeclarationSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class StructDeclarationPattern<TResult> : TypeDeclarationPattern<TResult>
    {
        private readonly Func<TResult, StructDeclarationSyntax, TResult> _action;

        internal StructDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern<TResult> baseList, TypeParameterListPattern<TResult> typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> constraintClauses, NodeListPattern<MemberDeclarationPattern<TResult>, TResult> members, Func<TResult, StructDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, identifier, baseList, typeParameterList, constraintClauses, members)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is StructDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (StructDeclarationSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class InterfaceDeclarationPattern<TResult> : TypeDeclarationPattern<TResult>
    {
        private readonly Func<TResult, InterfaceDeclarationSyntax, TResult> _action;

        internal InterfaceDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern<TResult> baseList, TypeParameterListPattern<TResult> typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> constraintClauses, NodeListPattern<MemberDeclarationPattern<TResult>, TResult> members, Func<TResult, InterfaceDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, identifier, baseList, typeParameterList, constraintClauses, members)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterfaceDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (InterfaceDeclarationSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class EnumDeclarationPattern<TResult> : BaseTypeDeclarationPattern<TResult>
    {
        private readonly NodeListPattern<EnumMemberDeclarationPattern<TResult>, TResult> _members;
        private readonly Func<TResult, EnumDeclarationSyntax, TResult> _action;

        internal EnumDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern<TResult> baseList, NodeListPattern<EnumMemberDeclarationPattern<TResult>, TResult> members, Func<TResult, EnumDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, identifier, baseList)
        {
            _members = members;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EnumDeclarationSyntax typed))
                return false;

            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (EnumDeclarationSyntax)node;

            if (_members != null)
                result = _members.RunCallback(result, typed.Members, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class DelegateDeclarationPattern<TResult> : MemberDeclarationPattern<TResult>
    {
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern<TResult> _returnType;
        private readonly string _identifier;
        private readonly TypeParameterListPattern<TResult> _typeParameterList;
        private readonly ParameterListPattern<TResult> _parameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> _constraintClauses;
        private readonly Func<TResult, DelegateDeclarationSyntax, TResult> _action;

        internal DelegateDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, TypePattern<TResult> returnType, string identifier, TypeParameterListPattern<TResult> typeParameterList, ParameterListPattern<TResult> parameterList, NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> constraintClauses, Func<TResult, DelegateDeclarationSyntax, TResult> action)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _returnType = returnType;
            _identifier = identifier;
            _typeParameterList = typeParameterList;
            _parameterList = parameterList;
            _constraintClauses = constraintClauses;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DelegateDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DelegateDeclarationSyntax)node;

            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);
            if (_returnType != null)
                result = _returnType.RunCallback(result, typed.ReturnType, semanticModel);
            if (_typeParameterList != null)
                result = _typeParameterList.RunCallback(result, typed.TypeParameterList, semanticModel);
            if (_parameterList != null)
                result = _parameterList.RunCallback(result, typed.ParameterList, semanticModel);
            if (_constraintClauses != null)
                result = _constraintClauses.RunCallback(result, typed.ConstraintClauses, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class EnumMemberDeclarationPattern<TResult> : MemberDeclarationPattern<TResult>
    {
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly string _identifier;
        private readonly EqualsValueClausePattern<TResult> _equalsValue;
        private readonly Func<TResult, EnumMemberDeclarationSyntax, TResult> _action;

        internal EnumMemberDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, string identifier, EqualsValueClausePattern<TResult> equalsValue, Func<TResult, EnumMemberDeclarationSyntax, TResult> action)
        {
            _attributeLists = attributeLists;
            _identifier = identifier;
            _equalsValue = equalsValue;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EnumMemberDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_equalsValue != null && !_equalsValue.Test(typed.EqualsValue, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (EnumMemberDeclarationSyntax)node;

            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);
            if (_equalsValue != null)
                result = _equalsValue.RunCallback(result, typed.EqualsValue, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class BaseListPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<BaseTypePattern<TResult>, TResult> _types;
        private readonly Func<TResult, BaseListSyntax, TResult> _action;

        internal BaseListPattern(NodeListPattern<BaseTypePattern<TResult>, TResult> types, Func<TResult, BaseListSyntax, TResult> action)
        {
            _types = types;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseListSyntax typed))
                return false;

            if (_types != null && !_types.Test(typed.Types, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseListSyntax)node;

            if (_types != null)
                result = _types.RunCallback(result, typed.Types, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class BaseTypePattern<TResult> : PatternNode<TResult>
    {
        private readonly TypePattern<TResult> _type;

        internal BaseTypePattern(TypePattern<TResult> type)
        {
            _type = type;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseTypeSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseTypeSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            return result;
        }
    }

    public partial class SimpleBaseTypePattern<TResult> : BaseTypePattern<TResult>
    {
        private readonly Func<TResult, SimpleBaseTypeSyntax, TResult> _action;

        internal SimpleBaseTypePattern(TypePattern<TResult> type, Func<TResult, SimpleBaseTypeSyntax, TResult> action)
            : base(type)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SimpleBaseTypeSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (SimpleBaseTypeSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class TypeParameterConstraintClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly IdentifierNamePattern<TResult> _name;
        private readonly NodeListPattern<TypeParameterConstraintPattern<TResult>, TResult> _constraints;
        private readonly Func<TResult, TypeParameterConstraintClauseSyntax, TResult> _action;

        internal TypeParameterConstraintClausePattern(IdentifierNamePattern<TResult> name, NodeListPattern<TypeParameterConstraintPattern<TResult>, TResult> constraints, Func<TResult, TypeParameterConstraintClauseSyntax, TResult> action)
        {
            _name = name;
            _constraints = constraints;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterConstraintClauseSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;
            if (_constraints != null && !_constraints.Test(typed.Constraints, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeParameterConstraintClauseSyntax)node;

            if (_name != null)
                result = _name.RunCallback(result, typed.Name, semanticModel);
            if (_constraints != null)
                result = _constraints.RunCallback(result, typed.Constraints, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class TypeParameterConstraintPattern<TResult> : PatternNode<TResult>
    {

        internal TypeParameterConstraintPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterConstraintSyntax typed))
                return false;


            return true;
        }
    }

    public partial class ConstructorConstraintPattern<TResult> : TypeParameterConstraintPattern<TResult>
    {
        private readonly Func<TResult, ConstructorConstraintSyntax, TResult> _action;

        internal ConstructorConstraintPattern(Func<TResult, ConstructorConstraintSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstructorConstraintSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConstructorConstraintSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ClassOrStructConstraintPattern<TResult> : TypeParameterConstraintPattern<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly Func<TResult, ClassOrStructConstraintSyntax, TResult> _action;

        internal ClassOrStructConstraintPattern(SyntaxKind kind, Func<TResult, ClassOrStructConstraintSyntax, TResult> action)
        {
            _kind = kind;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ClassOrStructConstraintSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ClassOrStructConstraintSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class TypeConstraintPattern<TResult> : TypeParameterConstraintPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly Func<TResult, TypeConstraintSyntax, TResult> _action;

        internal TypeConstraintPattern(TypePattern<TResult> type, Func<TResult, TypeConstraintSyntax, TResult> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeConstraintSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeConstraintSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class BaseFieldDeclarationPattern<TResult> : MemberDeclarationPattern<TResult>
    {
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly VariableDeclarationPattern<TResult> _declaration;

        internal BaseFieldDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, VariableDeclarationPattern<TResult> declaration)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _declaration = declaration;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseFieldDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseFieldDeclarationSyntax)node;

            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);
            if (_declaration != null)
                result = _declaration.RunCallback(result, typed.Declaration, semanticModel);
            return result;
        }
    }

    public partial class FieldDeclarationPattern<TResult> : BaseFieldDeclarationPattern<TResult>
    {
        private readonly Func<TResult, FieldDeclarationSyntax, TResult> _action;

        internal FieldDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, VariableDeclarationPattern<TResult> declaration, Func<TResult, FieldDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, declaration)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FieldDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (FieldDeclarationSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class EventFieldDeclarationPattern<TResult> : BaseFieldDeclarationPattern<TResult>
    {
        private readonly Func<TResult, EventFieldDeclarationSyntax, TResult> _action;

        internal EventFieldDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, VariableDeclarationPattern<TResult> declaration, Func<TResult, EventFieldDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, declaration)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EventFieldDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (EventFieldDeclarationSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ExplicitInterfaceSpecifierPattern<TResult> : PatternNode<TResult>
    {
        private readonly NamePattern<TResult> _name;
        private readonly Func<TResult, ExplicitInterfaceSpecifierSyntax, TResult> _action;

        internal ExplicitInterfaceSpecifierPattern(NamePattern<TResult> name, Func<TResult, ExplicitInterfaceSpecifierSyntax, TResult> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExplicitInterfaceSpecifierSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ExplicitInterfaceSpecifierSyntax)node;

            if (_name != null)
                result = _name.RunCallback(result, typed.Name, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class BaseMethodDeclarationPattern<TResult> : MemberDeclarationPattern<TResult>
    {
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly ParameterListPattern<TResult> _parameterList;
        private readonly BlockPattern<TResult> _body;
        private readonly ArrowExpressionClausePattern<TResult> _expressionBody;

        internal BaseMethodDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, ParameterListPattern<TResult> parameterList, BlockPattern<TResult> body, ArrowExpressionClausePattern<TResult> expressionBody)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _parameterList = parameterList;
            _body = body;
            _expressionBody = expressionBody;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseMethodDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;
            if (_expressionBody != null && !_expressionBody.Test(typed.ExpressionBody, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseMethodDeclarationSyntax)node;

            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);
            if (_parameterList != null)
                result = _parameterList.RunCallback(result, typed.ParameterList, semanticModel);
            if (_body != null)
                result = _body.RunCallback(result, typed.Body, semanticModel);
            if (_expressionBody != null)
                result = _expressionBody.RunCallback(result, typed.ExpressionBody, semanticModel);
            return result;
        }
    }

    public partial class MethodDeclarationPattern<TResult> : BaseMethodDeclarationPattern<TResult>
    {
        private readonly TypePattern<TResult> _returnType;
        private readonly ExplicitInterfaceSpecifierPattern<TResult> _explicitInterfaceSpecifier;
        private readonly string _identifier;
        private readonly TypeParameterListPattern<TResult> _typeParameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> _constraintClauses;
        private readonly Func<TResult, MethodDeclarationSyntax, TResult> _action;

        internal MethodDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, ParameterListPattern<TResult> parameterList, BlockPattern<TResult> body, ArrowExpressionClausePattern<TResult> expressionBody, TypePattern<TResult> returnType, ExplicitInterfaceSpecifierPattern<TResult> explicitInterfaceSpecifier, string identifier, TypeParameterListPattern<TResult> typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern<TResult>, TResult> constraintClauses, Func<TResult, MethodDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, parameterList, body, expressionBody)
        {
            _returnType = returnType;
            _explicitInterfaceSpecifier = explicitInterfaceSpecifier;
            _identifier = identifier;
            _typeParameterList = typeParameterList;
            _constraintClauses = constraintClauses;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MethodDeclarationSyntax typed))
                return false;

            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;
            if (_explicitInterfaceSpecifier != null && !_explicitInterfaceSpecifier.Test(typed.ExplicitInterfaceSpecifier, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (MethodDeclarationSyntax)node;

            if (_returnType != null)
                result = _returnType.RunCallback(result, typed.ReturnType, semanticModel);
            if (_explicitInterfaceSpecifier != null)
                result = _explicitInterfaceSpecifier.RunCallback(result, typed.ExplicitInterfaceSpecifier, semanticModel);
            if (_typeParameterList != null)
                result = _typeParameterList.RunCallback(result, typed.TypeParameterList, semanticModel);
            if (_constraintClauses != null)
                result = _constraintClauses.RunCallback(result, typed.ConstraintClauses, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class OperatorDeclarationPattern<TResult> : BaseMethodDeclarationPattern<TResult>
    {
        private readonly TypePattern<TResult> _returnType;
        private readonly Func<TResult, OperatorDeclarationSyntax, TResult> _action;

        internal OperatorDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, ParameterListPattern<TResult> parameterList, BlockPattern<TResult> body, ArrowExpressionClausePattern<TResult> expressionBody, TypePattern<TResult> returnType, Func<TResult, OperatorDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, parameterList, body, expressionBody)
        {
            _returnType = returnType;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OperatorDeclarationSyntax typed))
                return false;

            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (OperatorDeclarationSyntax)node;

            if (_returnType != null)
                result = _returnType.RunCallback(result, typed.ReturnType, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ConversionOperatorDeclarationPattern<TResult> : BaseMethodDeclarationPattern<TResult>
    {
        private readonly TypePattern<TResult> _type;
        private readonly Func<TResult, ConversionOperatorDeclarationSyntax, TResult> _action;

        internal ConversionOperatorDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, ParameterListPattern<TResult> parameterList, BlockPattern<TResult> body, ArrowExpressionClausePattern<TResult> expressionBody, TypePattern<TResult> type, Func<TResult, ConversionOperatorDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, parameterList, body, expressionBody)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConversionOperatorDeclarationSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (ConversionOperatorDeclarationSyntax)node;

            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ConstructorDeclarationPattern<TResult> : BaseMethodDeclarationPattern<TResult>
    {
        private readonly string _identifier;
        private readonly ConstructorInitializerPattern<TResult> _initializer;
        private readonly Func<TResult, ConstructorDeclarationSyntax, TResult> _action;

        internal ConstructorDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, ParameterListPattern<TResult> parameterList, BlockPattern<TResult> body, ArrowExpressionClausePattern<TResult> expressionBody, string identifier, ConstructorInitializerPattern<TResult> initializer, Func<TResult, ConstructorDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, parameterList, body, expressionBody)
        {
            _identifier = identifier;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstructorDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (ConstructorDeclarationSyntax)node;

            if (_initializer != null)
                result = _initializer.RunCallback(result, typed.Initializer, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ConstructorInitializerPattern<TResult> : PatternNode<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly ArgumentListPattern<TResult> _argumentList;
        private readonly Func<TResult, ConstructorInitializerSyntax, TResult> _action;

        internal ConstructorInitializerPattern(SyntaxKind kind, ArgumentListPattern<TResult> argumentList, Func<TResult, ConstructorInitializerSyntax, TResult> action)
        {
            _kind = kind;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstructorInitializerSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConstructorInitializerSyntax)node;

            if (_argumentList != null)
                result = _argumentList.RunCallback(result, typed.ArgumentList, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class DestructorDeclarationPattern<TResult> : BaseMethodDeclarationPattern<TResult>
    {
        private readonly string _identifier;
        private readonly Func<TResult, DestructorDeclarationSyntax, TResult> _action;

        internal DestructorDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, ParameterListPattern<TResult> parameterList, BlockPattern<TResult> body, ArrowExpressionClausePattern<TResult> expressionBody, string identifier, Func<TResult, DestructorDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, parameterList, body, expressionBody)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DestructorDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (DestructorDeclarationSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class BasePropertyDeclarationPattern<TResult> : MemberDeclarationPattern<TResult>
    {
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern<TResult> _type;
        private readonly ExplicitInterfaceSpecifierPattern<TResult> _explicitInterfaceSpecifier;
        private readonly AccessorListPattern<TResult> _accessorList;

        internal BasePropertyDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, TypePattern<TResult> type, ExplicitInterfaceSpecifierPattern<TResult> explicitInterfaceSpecifier, AccessorListPattern<TResult> accessorList)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _type = type;
            _explicitInterfaceSpecifier = explicitInterfaceSpecifier;
            _accessorList = accessorList;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BasePropertyDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_explicitInterfaceSpecifier != null && !_explicitInterfaceSpecifier.Test(typed.ExplicitInterfaceSpecifier, semanticModel))
                return false;
            if (_accessorList != null && !_accessorList.Test(typed.AccessorList, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BasePropertyDeclarationSyntax)node;

            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);
            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            if (_explicitInterfaceSpecifier != null)
                result = _explicitInterfaceSpecifier.RunCallback(result, typed.ExplicitInterfaceSpecifier, semanticModel);
            if (_accessorList != null)
                result = _accessorList.RunCallback(result, typed.AccessorList, semanticModel);
            return result;
        }
    }

    public partial class PropertyDeclarationPattern<TResult> : BasePropertyDeclarationPattern<TResult>
    {
        private readonly string _identifier;
        private readonly ArrowExpressionClausePattern<TResult> _expressionBody;
        private readonly EqualsValueClausePattern<TResult> _initializer;
        private readonly Func<TResult, PropertyDeclarationSyntax, TResult> _action;

        internal PropertyDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, TypePattern<TResult> type, ExplicitInterfaceSpecifierPattern<TResult> explicitInterfaceSpecifier, AccessorListPattern<TResult> accessorList, string identifier, ArrowExpressionClausePattern<TResult> expressionBody, EqualsValueClausePattern<TResult> initializer, Func<TResult, PropertyDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, type, explicitInterfaceSpecifier, accessorList)
        {
            _identifier = identifier;
            _expressionBody = expressionBody;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PropertyDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_expressionBody != null && !_expressionBody.Test(typed.ExpressionBody, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (PropertyDeclarationSyntax)node;

            if (_expressionBody != null)
                result = _expressionBody.RunCallback(result, typed.ExpressionBody, semanticModel);
            if (_initializer != null)
                result = _initializer.RunCallback(result, typed.Initializer, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ArrowExpressionClausePattern<TResult> : PatternNode<TResult>
    {
        private readonly ExpressionPattern<TResult> _expression;
        private readonly Func<TResult, ArrowExpressionClauseSyntax, TResult> _action;

        internal ArrowExpressionClausePattern(ExpressionPattern<TResult> expression, Func<TResult, ArrowExpressionClauseSyntax, TResult> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrowExpressionClauseSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrowExpressionClauseSyntax)node;

            if (_expression != null)
                result = _expression.RunCallback(result, typed.Expression, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class EventDeclarationPattern<TResult> : BasePropertyDeclarationPattern<TResult>
    {
        private readonly string _identifier;
        private readonly Func<TResult, EventDeclarationSyntax, TResult> _action;

        internal EventDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, TypePattern<TResult> type, ExplicitInterfaceSpecifierPattern<TResult> explicitInterfaceSpecifier, AccessorListPattern<TResult> accessorList, string identifier, Func<TResult, EventDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, type, explicitInterfaceSpecifier, accessorList)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EventDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (EventDeclarationSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class IndexerDeclarationPattern<TResult> : BasePropertyDeclarationPattern<TResult>
    {
        private readonly BracketedParameterListPattern<TResult> _parameterList;
        private readonly ArrowExpressionClausePattern<TResult> _expressionBody;
        private readonly Func<TResult, IndexerDeclarationSyntax, TResult> _action;

        internal IndexerDeclarationPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, TypePattern<TResult> type, ExplicitInterfaceSpecifierPattern<TResult> explicitInterfaceSpecifier, AccessorListPattern<TResult> accessorList, BracketedParameterListPattern<TResult> parameterList, ArrowExpressionClausePattern<TResult> expressionBody, Func<TResult, IndexerDeclarationSyntax, TResult> action)
            : base(attributeLists, modifiers, type, explicitInterfaceSpecifier, accessorList)
        {
            _parameterList = parameterList;
            _expressionBody = expressionBody;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IndexerDeclarationSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;
            if (_expressionBody != null && !_expressionBody.Test(typed.ExpressionBody, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (IndexerDeclarationSyntax)node;

            if (_parameterList != null)
                result = _parameterList.RunCallback(result, typed.ParameterList, semanticModel);
            if (_expressionBody != null)
                result = _expressionBody.RunCallback(result, typed.ExpressionBody, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AccessorListPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<AccessorDeclarationPattern<TResult>, TResult> _accessors;
        private readonly Func<TResult, AccessorListSyntax, TResult> _action;

        internal AccessorListPattern(NodeListPattern<AccessorDeclarationPattern<TResult>, TResult> accessors, Func<TResult, AccessorListSyntax, TResult> action)
        {
            _accessors = accessors;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AccessorListSyntax typed))
                return false;

            if (_accessors != null && !_accessors.Test(typed.Accessors, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AccessorListSyntax)node;

            if (_accessors != null)
                result = _accessors.RunCallback(result, typed.Accessors, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class AccessorDeclarationPattern<TResult> : PatternNode<TResult>
    {
        private readonly SyntaxKind _kind;
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly BlockPattern<TResult> _body;
        private readonly ArrowExpressionClausePattern<TResult> _expressionBody;
        private readonly Func<TResult, AccessorDeclarationSyntax, TResult> _action;

        internal AccessorDeclarationPattern(SyntaxKind kind, NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, BlockPattern<TResult> body, ArrowExpressionClausePattern<TResult> expressionBody, Func<TResult, AccessorDeclarationSyntax, TResult> action)
        {
            _kind = kind;
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _body = body;
            _expressionBody = expressionBody;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AccessorDeclarationSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;
            if (_expressionBody != null && !_expressionBody.Test(typed.ExpressionBody, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AccessorDeclarationSyntax)node;

            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);
            if (_body != null)
                result = _body.RunCallback(result, typed.Body, semanticModel);
            if (_expressionBody != null)
                result = _expressionBody.RunCallback(result, typed.ExpressionBody, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public abstract partial class BaseParameterListPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<ParameterPattern<TResult>, TResult> _parameters;

        internal BaseParameterListPattern(NodeListPattern<ParameterPattern<TResult>, TResult> parameters)
        {
            _parameters = parameters;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseParameterListSyntax typed))
                return false;

            if (_parameters != null && !_parameters.Test(typed.Parameters, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseParameterListSyntax)node;

            if (_parameters != null)
                result = _parameters.RunCallback(result, typed.Parameters, semanticModel);
            return result;
        }
    }

    public partial class ParameterListPattern<TResult> : BaseParameterListPattern<TResult>
    {
        private readonly Func<TResult, ParameterListSyntax, TResult> _action;

        internal ParameterListPattern(NodeListPattern<ParameterPattern<TResult>, TResult> parameters, Func<TResult, ParameterListSyntax, TResult> action)
            : base(parameters)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParameterListSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (ParameterListSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class BracketedParameterListPattern<TResult> : BaseParameterListPattern<TResult>
    {
        private readonly Func<TResult, BracketedParameterListSyntax, TResult> _action;

        internal BracketedParameterListPattern(NodeListPattern<ParameterPattern<TResult>, TResult> parameters, Func<TResult, BracketedParameterListSyntax, TResult> action)
            : base(parameters)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BracketedParameterListSyntax typed))
                return false;


            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            result = base.RunCallback(result, node, semanticModel);

            var typed = (BracketedParameterListSyntax)node;


            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class ParameterPattern<TResult> : PatternNode<TResult>
    {
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern<TResult> _type;
        private readonly string _identifier;
        private readonly EqualsValueClausePattern<TResult> _default;
        private readonly Func<TResult, ParameterSyntax, TResult> _action;

        internal ParameterPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, TypePattern<TResult> type, string identifier, EqualsValueClausePattern<TResult> @default, Func<TResult, ParameterSyntax, TResult> action)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _type = type;
            _identifier = identifier;
            _default = @default;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParameterSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_default != null && !_default.Test(typed.Default, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParameterSyntax)node;

            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);
            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);
            if (_default != null)
                result = _default.RunCallback(result, typed.Default, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    public partial class IncompleteMemberPattern<TResult> : MemberDeclarationPattern<TResult>
    {
        private readonly NodeListPattern<AttributeListPattern<TResult>, TResult> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern<TResult> _type;
        private readonly Func<TResult, IncompleteMemberSyntax, TResult> _action;

        internal IncompleteMemberPattern(NodeListPattern<AttributeListPattern<TResult>, TResult> attributeLists, TokenListPattern modifiers, TypePattern<TResult> type, Func<TResult, IncompleteMemberSyntax, TResult> action)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IncompleteMemberSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (IncompleteMemberSyntax)node;

            if (_attributeLists != null)
                result = _attributeLists.RunCallback(result, typed.AttributeLists, semanticModel);
            if (_type != null)
                result = _type.RunCallback(result, typed.Type, semanticModel);

            if (_action != null)
                result = _action(result, typed);

            return result;
        }
    }

    partial struct PatternBuilder<TResult>
    {
        public IdentifierNamePattern<TResult> IdentifierName(string identifier = null, Func<TResult, IdentifierNameSyntax, TResult> action = null)
        {
            return new IdentifierNamePattern<TResult>(identifier, action);
        }
        public QualifiedNamePattern<TResult> QualifiedName(NamePattern<TResult> left = null, SimpleNamePattern<TResult> right = null, Func<TResult, QualifiedNameSyntax, TResult> action = null)
        {
            return new QualifiedNamePattern<TResult>(left, right, action);
        }
        public GenericNamePattern<TResult> GenericName(string identifier = null, TypeArgumentListPattern<TResult> typeArgumentList = null, Func<TResult, GenericNameSyntax, TResult> action = null)
        {
            return new GenericNamePattern<TResult>(identifier, typeArgumentList, action);
        }
        public TypeArgumentListPattern<TResult> TypeArgumentList(IEnumerable<TypePattern<TResult>> arguments = null, Func<TResult, TypeArgumentListSyntax, TResult> action = null)
        {
            return new TypeArgumentListPattern<TResult>(NodeList(arguments), action);
        }

        public TypeArgumentListPattern<TResult> TypeArgumentList(params TypePattern<TResult>[] arguments)
        {
            return new TypeArgumentListPattern<TResult>(NodeList(arguments), null);
        }
        public AliasQualifiedNamePattern<TResult> AliasQualifiedName(IdentifierNamePattern<TResult> alias = null, SimpleNamePattern<TResult> name = null, Func<TResult, AliasQualifiedNameSyntax, TResult> action = null)
        {
            return new AliasQualifiedNamePattern<TResult>(alias, name, action);
        }
        public PredefinedTypePattern<TResult> PredefinedType(string keyword = null, Func<TResult, PredefinedTypeSyntax, TResult> action = null)
        {
            return new PredefinedTypePattern<TResult>(keyword, action);
        }
        public ArrayTypePattern<TResult> ArrayType(TypePattern<TResult> elementType = null, IEnumerable<ArrayRankSpecifierPattern<TResult>> rankSpecifiers = null, Func<TResult, ArrayTypeSyntax, TResult> action = null)
        {
            return new ArrayTypePattern<TResult>(elementType, NodeList(rankSpecifiers), action);
        }
        public ArrayRankSpecifierPattern<TResult> ArrayRankSpecifier(IEnumerable<ExpressionPattern<TResult>> sizes = null, Func<TResult, ArrayRankSpecifierSyntax, TResult> action = null)
        {
            return new ArrayRankSpecifierPattern<TResult>(NodeList(sizes), action);
        }

        public ArrayRankSpecifierPattern<TResult> ArrayRankSpecifier(params ExpressionPattern<TResult>[] sizes)
        {
            return new ArrayRankSpecifierPattern<TResult>(NodeList(sizes), null);
        }
        public PointerTypePattern<TResult> PointerType(TypePattern<TResult> elementType = null, Func<TResult, PointerTypeSyntax, TResult> action = null)
        {
            return new PointerTypePattern<TResult>(elementType, action);
        }
        public NullableTypePattern<TResult> NullableType(TypePattern<TResult> elementType = null, Func<TResult, NullableTypeSyntax, TResult> action = null)
        {
            return new NullableTypePattern<TResult>(elementType, action);
        }
        public TupleTypePattern<TResult> TupleType(IEnumerable<TupleElementPattern<TResult>> elements = null, Func<TResult, TupleTypeSyntax, TResult> action = null)
        {
            return new TupleTypePattern<TResult>(NodeList(elements), action);
        }

        public TupleTypePattern<TResult> TupleType(params TupleElementPattern<TResult>[] elements)
        {
            return new TupleTypePattern<TResult>(NodeList(elements), null);
        }
        public TupleElementPattern<TResult> TupleElement(TypePattern<TResult> type = null, string identifier = null, Func<TResult, TupleElementSyntax, TResult> action = null)
        {
            return new TupleElementPattern<TResult>(type, identifier, action);
        }
        public OmittedTypeArgumentPattern<TResult> OmittedTypeArgument(Func<TResult, OmittedTypeArgumentSyntax, TResult> action = null)
        {
            return new OmittedTypeArgumentPattern<TResult>(action);
        }
        public RefTypePattern<TResult> RefType(TypePattern<TResult> type = null, Func<TResult, RefTypeSyntax, TResult> action = null)
        {
            return new RefTypePattern<TResult>(type, action);
        }
        public ParenthesizedExpressionPattern<TResult> ParenthesizedExpression(ExpressionPattern<TResult> expression = null, Func<TResult, ParenthesizedExpressionSyntax, TResult> action = null)
        {
            return new ParenthesizedExpressionPattern<TResult>(expression, action);
        }
        public TupleExpressionPattern<TResult> TupleExpression(IEnumerable<ArgumentPattern<TResult>> arguments = null, Func<TResult, TupleExpressionSyntax, TResult> action = null)
        {
            return new TupleExpressionPattern<TResult>(NodeList(arguments), action);
        }

        public TupleExpressionPattern<TResult> TupleExpression(params ArgumentPattern<TResult>[] arguments)
        {
            return new TupleExpressionPattern<TResult>(NodeList(arguments), null);
        }
        public PrefixUnaryExpressionPattern<TResult> PrefixUnaryExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern<TResult> operand = null, Func<TResult, PrefixUnaryExpressionSyntax, TResult> action = null)
        {
            return new PrefixUnaryExpressionPattern<TResult>(kind, operand, action);
        }
        public AwaitExpressionPattern<TResult> AwaitExpression(ExpressionPattern<TResult> expression = null, Func<TResult, AwaitExpressionSyntax, TResult> action = null)
        {
            return new AwaitExpressionPattern<TResult>(expression, action);
        }
        public PostfixUnaryExpressionPattern<TResult> PostfixUnaryExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern<TResult> operand = null, Func<TResult, PostfixUnaryExpressionSyntax, TResult> action = null)
        {
            return new PostfixUnaryExpressionPattern<TResult>(kind, operand, action);
        }
        public MemberAccessExpressionPattern<TResult> MemberAccessExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern<TResult> expression = null, SimpleNamePattern<TResult> name = null, Func<TResult, MemberAccessExpressionSyntax, TResult> action = null)
        {
            return new MemberAccessExpressionPattern<TResult>(kind, expression, name, action);
        }
        public ConditionalAccessExpressionPattern<TResult> ConditionalAccessExpression(ExpressionPattern<TResult> expression = null, ExpressionPattern<TResult> whenNotNull = null, Func<TResult, ConditionalAccessExpressionSyntax, TResult> action = null)
        {
            return new ConditionalAccessExpressionPattern<TResult>(expression, whenNotNull, action);
        }
        public MemberBindingExpressionPattern<TResult> MemberBindingExpression(SimpleNamePattern<TResult> name = null, Func<TResult, MemberBindingExpressionSyntax, TResult> action = null)
        {
            return new MemberBindingExpressionPattern<TResult>(name, action);
        }
        public ElementBindingExpressionPattern<TResult> ElementBindingExpression(BracketedArgumentListPattern<TResult> argumentList = null, Func<TResult, ElementBindingExpressionSyntax, TResult> action = null)
        {
            return new ElementBindingExpressionPattern<TResult>(argumentList, action);
        }
        public ImplicitElementAccessPattern<TResult> ImplicitElementAccess(BracketedArgumentListPattern<TResult> argumentList = null, Func<TResult, ImplicitElementAccessSyntax, TResult> action = null)
        {
            return new ImplicitElementAccessPattern<TResult>(argumentList, action);
        }
        public BinaryExpressionPattern<TResult> BinaryExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern<TResult> left = null, ExpressionPattern<TResult> right = null, Func<TResult, BinaryExpressionSyntax, TResult> action = null)
        {
            return new BinaryExpressionPattern<TResult>(kind, left, right, action);
        }
        public AssignmentExpressionPattern<TResult> AssignmentExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern<TResult> left = null, ExpressionPattern<TResult> right = null, Func<TResult, AssignmentExpressionSyntax, TResult> action = null)
        {
            return new AssignmentExpressionPattern<TResult>(kind, left, right, action);
        }
        public ConditionalExpressionPattern<TResult> ConditionalExpression(ExpressionPattern<TResult> condition = null, ExpressionPattern<TResult> whenTrue = null, ExpressionPattern<TResult> whenFalse = null, Func<TResult, ConditionalExpressionSyntax, TResult> action = null)
        {
            return new ConditionalExpressionPattern<TResult>(condition, whenTrue, whenFalse, action);
        }
        public ThisExpressionPattern<TResult> ThisExpression(Func<TResult, ThisExpressionSyntax, TResult> action = null)
        {
            return new ThisExpressionPattern<TResult>(action);
        }
        public BaseExpressionPattern<TResult> BaseExpression(Func<TResult, BaseExpressionSyntax, TResult> action = null)
        {
            return new BaseExpressionPattern<TResult>(action);
        }
        public LiteralExpressionPattern<TResult> LiteralExpression(SyntaxKind kind = default(SyntaxKind), Func<TResult, LiteralExpressionSyntax, TResult> action = null)
        {
            return new LiteralExpressionPattern<TResult>(kind, action);
        }
        public MakeRefExpressionPattern<TResult> MakeRefExpression(ExpressionPattern<TResult> expression = null, Func<TResult, MakeRefExpressionSyntax, TResult> action = null)
        {
            return new MakeRefExpressionPattern<TResult>(expression, action);
        }
        public RefTypeExpressionPattern<TResult> RefTypeExpression(ExpressionPattern<TResult> expression = null, Func<TResult, RefTypeExpressionSyntax, TResult> action = null)
        {
            return new RefTypeExpressionPattern<TResult>(expression, action);
        }
        public RefValueExpressionPattern<TResult> RefValueExpression(ExpressionPattern<TResult> expression = null, TypePattern<TResult> type = null, Func<TResult, RefValueExpressionSyntax, TResult> action = null)
        {
            return new RefValueExpressionPattern<TResult>(expression, type, action);
        }
        public CheckedExpressionPattern<TResult> CheckedExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern<TResult> expression = null, Func<TResult, CheckedExpressionSyntax, TResult> action = null)
        {
            return new CheckedExpressionPattern<TResult>(kind, expression, action);
        }
        public DefaultExpressionPattern<TResult> DefaultExpression(TypePattern<TResult> type = null, Func<TResult, DefaultExpressionSyntax, TResult> action = null)
        {
            return new DefaultExpressionPattern<TResult>(type, action);
        }
        public TypeOfExpressionPattern<TResult> TypeOfExpression(TypePattern<TResult> type = null, Func<TResult, TypeOfExpressionSyntax, TResult> action = null)
        {
            return new TypeOfExpressionPattern<TResult>(type, action);
        }
        public SizeOfExpressionPattern<TResult> SizeOfExpression(TypePattern<TResult> type = null, Func<TResult, SizeOfExpressionSyntax, TResult> action = null)
        {
            return new SizeOfExpressionPattern<TResult>(type, action);
        }
        public InvocationExpressionPattern<TResult> InvocationExpression(ExpressionPattern<TResult> expression = null, ArgumentListPattern<TResult> argumentList = null, Func<TResult, InvocationExpressionSyntax, TResult> action = null)
        {
            return new InvocationExpressionPattern<TResult>(expression, argumentList, action);
        }
        public ElementAccessExpressionPattern<TResult> ElementAccessExpression(ExpressionPattern<TResult> expression = null, BracketedArgumentListPattern<TResult> argumentList = null, Func<TResult, ElementAccessExpressionSyntax, TResult> action = null)
        {
            return new ElementAccessExpressionPattern<TResult>(expression, argumentList, action);
        }
        public ArgumentListPattern<TResult> ArgumentList(IEnumerable<ArgumentPattern<TResult>> arguments = null, Func<TResult, ArgumentListSyntax, TResult> action = null)
        {
            return new ArgumentListPattern<TResult>(NodeList(arguments), action);
        }

        public ArgumentListPattern<TResult> ArgumentList(params ArgumentPattern<TResult>[] arguments)
        {
            return new ArgumentListPattern<TResult>(NodeList(arguments), null);
        }
        public BracketedArgumentListPattern<TResult> BracketedArgumentList(IEnumerable<ArgumentPattern<TResult>> arguments = null, Func<TResult, BracketedArgumentListSyntax, TResult> action = null)
        {
            return new BracketedArgumentListPattern<TResult>(NodeList(arguments), action);
        }

        public BracketedArgumentListPattern<TResult> BracketedArgumentList(params ArgumentPattern<TResult>[] arguments)
        {
            return new BracketedArgumentListPattern<TResult>(NodeList(arguments), null);
        }
        public ArgumentPattern<TResult> Argument(NameColonPattern<TResult> nameColon = null, ExpressionPattern<TResult> expression = null, Func<TResult, ArgumentSyntax, TResult> action = null)
        {
            return new ArgumentPattern<TResult>(nameColon, expression, action);
        }
        public NameColonPattern<TResult> NameColon(IdentifierNamePattern<TResult> name = null, Func<TResult, NameColonSyntax, TResult> action = null)
        {
            return new NameColonPattern<TResult>(name, action);
        }
        public DeclarationExpressionPattern<TResult> DeclarationExpression(TypePattern<TResult> type = null, VariableDesignationPattern<TResult> designation = null, Func<TResult, DeclarationExpressionSyntax, TResult> action = null)
        {
            return new DeclarationExpressionPattern<TResult>(type, designation, action);
        }
        public CastExpressionPattern<TResult> CastExpression(TypePattern<TResult> type = null, ExpressionPattern<TResult> expression = null, Func<TResult, CastExpressionSyntax, TResult> action = null)
        {
            return new CastExpressionPattern<TResult>(type, expression, action);
        }
        public AnonymousMethodExpressionPattern<TResult> AnonymousMethodExpression(PatternNode<TResult> body = null, ParameterListPattern<TResult> parameterList = null, Func<TResult, AnonymousMethodExpressionSyntax, TResult> action = null)
        {
            return new AnonymousMethodExpressionPattern<TResult>(body, parameterList, action);
        }
        public SimpleLambdaExpressionPattern<TResult> SimpleLambdaExpression(PatternNode<TResult> body = null, ParameterPattern<TResult> parameter = null, Func<TResult, SimpleLambdaExpressionSyntax, TResult> action = null)
        {
            return new SimpleLambdaExpressionPattern<TResult>(body, parameter, action);
        }
        public RefExpressionPattern<TResult> RefExpression(ExpressionPattern<TResult> expression = null, Func<TResult, RefExpressionSyntax, TResult> action = null)
        {
            return new RefExpressionPattern<TResult>(expression, action);
        }
        public ParenthesizedLambdaExpressionPattern<TResult> ParenthesizedLambdaExpression(PatternNode<TResult> body = null, ParameterListPattern<TResult> parameterList = null, Func<TResult, ParenthesizedLambdaExpressionSyntax, TResult> action = null)
        {
            return new ParenthesizedLambdaExpressionPattern<TResult>(body, parameterList, action);
        }
        public InitializerExpressionPattern<TResult> InitializerExpression(SyntaxKind kind = default(SyntaxKind), IEnumerable<ExpressionPattern<TResult>> expressions = null, Func<TResult, InitializerExpressionSyntax, TResult> action = null)
        {
            return new InitializerExpressionPattern<TResult>(kind, NodeList(expressions), action);
        }

        public InitializerExpressionPattern<TResult> InitializerExpression(SyntaxKind kind, params ExpressionPattern<TResult>[] expressions)
        {
            return new InitializerExpressionPattern<TResult>(kind, NodeList(expressions), null);
        }
        public ObjectCreationExpressionPattern<TResult> ObjectCreationExpression(TypePattern<TResult> type = null, ArgumentListPattern<TResult> argumentList = null, InitializerExpressionPattern<TResult> initializer = null, Func<TResult, ObjectCreationExpressionSyntax, TResult> action = null)
        {
            return new ObjectCreationExpressionPattern<TResult>(type, argumentList, initializer, action);
        }
        public AnonymousObjectMemberDeclaratorPattern<TResult> AnonymousObjectMemberDeclarator(NameEqualsPattern<TResult> nameEquals = null, ExpressionPattern<TResult> expression = null, Func<TResult, AnonymousObjectMemberDeclaratorSyntax, TResult> action = null)
        {
            return new AnonymousObjectMemberDeclaratorPattern<TResult>(nameEquals, expression, action);
        }
        public AnonymousObjectCreationExpressionPattern<TResult> AnonymousObjectCreationExpression(IEnumerable<AnonymousObjectMemberDeclaratorPattern<TResult>> initializers = null, Func<TResult, AnonymousObjectCreationExpressionSyntax, TResult> action = null)
        {
            return new AnonymousObjectCreationExpressionPattern<TResult>(NodeList(initializers), action);
        }

        public AnonymousObjectCreationExpressionPattern<TResult> AnonymousObjectCreationExpression(params AnonymousObjectMemberDeclaratorPattern<TResult>[] initializers)
        {
            return new AnonymousObjectCreationExpressionPattern<TResult>(NodeList(initializers), null);
        }
        public ArrayCreationExpressionPattern<TResult> ArrayCreationExpression(ArrayTypePattern<TResult> type = null, InitializerExpressionPattern<TResult> initializer = null, Func<TResult, ArrayCreationExpressionSyntax, TResult> action = null)
        {
            return new ArrayCreationExpressionPattern<TResult>(type, initializer, action);
        }
        public ImplicitArrayCreationExpressionPattern<TResult> ImplicitArrayCreationExpression(InitializerExpressionPattern<TResult> initializer = null, Func<TResult, ImplicitArrayCreationExpressionSyntax, TResult> action = null)
        {
            return new ImplicitArrayCreationExpressionPattern<TResult>(initializer, action);
        }
        public StackAllocArrayCreationExpressionPattern<TResult> StackAllocArrayCreationExpression(TypePattern<TResult> type = null, Func<TResult, StackAllocArrayCreationExpressionSyntax, TResult> action = null)
        {
            return new StackAllocArrayCreationExpressionPattern<TResult>(type, action);
        }
        public QueryExpressionPattern<TResult> QueryExpression(FromClausePattern<TResult> fromClause = null, QueryBodyPattern<TResult> body = null, Func<TResult, QueryExpressionSyntax, TResult> action = null)
        {
            return new QueryExpressionPattern<TResult>(fromClause, body, action);
        }
        public QueryBodyPattern<TResult> QueryBody(IEnumerable<QueryClausePattern<TResult>> clauses = null, SelectOrGroupClausePattern<TResult> selectOrGroup = null, QueryContinuationPattern<TResult> continuation = null, Func<TResult, QueryBodySyntax, TResult> action = null)
        {
            return new QueryBodyPattern<TResult>(NodeList(clauses), selectOrGroup, continuation, action);
        }
        public FromClausePattern<TResult> FromClause(TypePattern<TResult> type = null, string identifier = null, ExpressionPattern<TResult> expression = null, Func<TResult, FromClauseSyntax, TResult> action = null)
        {
            return new FromClausePattern<TResult>(type, identifier, expression, action);
        }
        public LetClausePattern<TResult> LetClause(string identifier = null, ExpressionPattern<TResult> expression = null, Func<TResult, LetClauseSyntax, TResult> action = null)
        {
            return new LetClausePattern<TResult>(identifier, expression, action);
        }
        public JoinClausePattern<TResult> JoinClause(TypePattern<TResult> type = null, string identifier = null, ExpressionPattern<TResult> inExpression = null, ExpressionPattern<TResult> leftExpression = null, ExpressionPattern<TResult> rightExpression = null, JoinIntoClausePattern<TResult> into = null, Func<TResult, JoinClauseSyntax, TResult> action = null)
        {
            return new JoinClausePattern<TResult>(type, identifier, inExpression, leftExpression, rightExpression, into, action);
        }
        public JoinIntoClausePattern<TResult> JoinIntoClause(string identifier = null, Func<TResult, JoinIntoClauseSyntax, TResult> action = null)
        {
            return new JoinIntoClausePattern<TResult>(identifier, action);
        }
        public WhereClausePattern<TResult> WhereClause(ExpressionPattern<TResult> condition = null, Func<TResult, WhereClauseSyntax, TResult> action = null)
        {
            return new WhereClausePattern<TResult>(condition, action);
        }
        public OrderByClausePattern<TResult> OrderByClause(IEnumerable<OrderingPattern<TResult>> orderings = null, Func<TResult, OrderByClauseSyntax, TResult> action = null)
        {
            return new OrderByClausePattern<TResult>(NodeList(orderings), action);
        }

        public OrderByClausePattern<TResult> OrderByClause(params OrderingPattern<TResult>[] orderings)
        {
            return new OrderByClausePattern<TResult>(NodeList(orderings), null);
        }
        public OrderingPattern<TResult> Ordering(SyntaxKind kind = default(SyntaxKind), ExpressionPattern<TResult> expression = null, Func<TResult, OrderingSyntax, TResult> action = null)
        {
            return new OrderingPattern<TResult>(kind, expression, action);
        }
        public SelectClausePattern<TResult> SelectClause(ExpressionPattern<TResult> expression = null, Func<TResult, SelectClauseSyntax, TResult> action = null)
        {
            return new SelectClausePattern<TResult>(expression, action);
        }
        public GroupClausePattern<TResult> GroupClause(ExpressionPattern<TResult> groupExpression = null, ExpressionPattern<TResult> byExpression = null, Func<TResult, GroupClauseSyntax, TResult> action = null)
        {
            return new GroupClausePattern<TResult>(groupExpression, byExpression, action);
        }
        public QueryContinuationPattern<TResult> QueryContinuation(string identifier = null, QueryBodyPattern<TResult> body = null, Func<TResult, QueryContinuationSyntax, TResult> action = null)
        {
            return new QueryContinuationPattern<TResult>(identifier, body, action);
        }
        public OmittedArraySizeExpressionPattern<TResult> OmittedArraySizeExpression(Func<TResult, OmittedArraySizeExpressionSyntax, TResult> action = null)
        {
            return new OmittedArraySizeExpressionPattern<TResult>(action);
        }
        public InterpolatedStringExpressionPattern<TResult> InterpolatedStringExpression(IEnumerable<InterpolatedStringContentPattern<TResult>> contents = null, Func<TResult, InterpolatedStringExpressionSyntax, TResult> action = null)
        {
            return new InterpolatedStringExpressionPattern<TResult>(NodeList(contents), action);
        }

        public InterpolatedStringExpressionPattern<TResult> InterpolatedStringExpression(params InterpolatedStringContentPattern<TResult>[] contents)
        {
            return new InterpolatedStringExpressionPattern<TResult>(NodeList(contents), null);
        }
        public IsPatternExpressionPattern<TResult> IsPatternExpression(ExpressionPattern<TResult> expression = null, PatternPattern<TResult> pattern = null, Func<TResult, IsPatternExpressionSyntax, TResult> action = null)
        {
            return new IsPatternExpressionPattern<TResult>(expression, pattern, action);
        }
        public ThrowExpressionPattern<TResult> ThrowExpression(ExpressionPattern<TResult> expression = null, Func<TResult, ThrowExpressionSyntax, TResult> action = null)
        {
            return new ThrowExpressionPattern<TResult>(expression, action);
        }
        public WhenClausePattern<TResult> WhenClause(ExpressionPattern<TResult> condition = null, Func<TResult, WhenClauseSyntax, TResult> action = null)
        {
            return new WhenClausePattern<TResult>(condition, action);
        }
        public DeclarationPatternPattern<TResult> DeclarationPattern(TypePattern<TResult> type = null, VariableDesignationPattern<TResult> designation = null, Func<TResult, DeclarationPatternSyntax, TResult> action = null)
        {
            return new DeclarationPatternPattern<TResult>(type, designation, action);
        }
        public ConstantPatternPattern<TResult> ConstantPattern(ExpressionPattern<TResult> expression = null, Func<TResult, ConstantPatternSyntax, TResult> action = null)
        {
            return new ConstantPatternPattern<TResult>(expression, action);
        }
        public InterpolatedStringTextPattern<TResult> InterpolatedStringText(Func<TResult, InterpolatedStringTextSyntax, TResult> action = null)
        {
            return new InterpolatedStringTextPattern<TResult>(action);
        }
        public InterpolationPattern<TResult> Interpolation(ExpressionPattern<TResult> expression = null, InterpolationAlignmentClausePattern<TResult> alignmentClause = null, InterpolationFormatClausePattern<TResult> formatClause = null, Func<TResult, InterpolationSyntax, TResult> action = null)
        {
            return new InterpolationPattern<TResult>(expression, alignmentClause, formatClause, action);
        }
        public InterpolationAlignmentClausePattern<TResult> InterpolationAlignmentClause(ExpressionPattern<TResult> value = null, Func<TResult, InterpolationAlignmentClauseSyntax, TResult> action = null)
        {
            return new InterpolationAlignmentClausePattern<TResult>(value, action);
        }
        public InterpolationFormatClausePattern<TResult> InterpolationFormatClause(Func<TResult, InterpolationFormatClauseSyntax, TResult> action = null)
        {
            return new InterpolationFormatClausePattern<TResult>(action);
        }
        public GlobalStatementPattern<TResult> GlobalStatement(StatementPattern<TResult> statement = null, Func<TResult, GlobalStatementSyntax, TResult> action = null)
        {
            return new GlobalStatementPattern<TResult>(statement, action);
        }
        public BlockPattern<TResult> Block(IEnumerable<StatementPattern<TResult>> statements = null, Func<TResult, BlockSyntax, TResult> action = null)
        {
            return new BlockPattern<TResult>(NodeList(statements), action);
        }

        public BlockPattern<TResult> Block(params StatementPattern<TResult>[] statements)
        {
            return new BlockPattern<TResult>(NodeList(statements), null);
        }
        public LocalFunctionStatementPattern<TResult> LocalFunctionStatement(IEnumerable<string> modifiers = null, TypePattern<TResult> returnType = null, string identifier = null, TypeParameterListPattern<TResult> typeParameterList = null, ParameterListPattern<TResult> parameterList = null, IEnumerable<TypeParameterConstraintClausePattern<TResult>> constraintClauses = null, BlockPattern<TResult> body = null, ArrowExpressionClausePattern<TResult> expressionBody = null, Func<TResult, LocalFunctionStatementSyntax, TResult> action = null)
        {
            return new LocalFunctionStatementPattern<TResult>(TokenList(modifiers), returnType, identifier, typeParameterList, parameterList, NodeList(constraintClauses), body, expressionBody, action);
        }
        public LocalDeclarationStatementPattern<TResult> LocalDeclarationStatement(IEnumerable<string> modifiers = null, VariableDeclarationPattern<TResult> declaration = null, Func<TResult, LocalDeclarationStatementSyntax, TResult> action = null)
        {
            return new LocalDeclarationStatementPattern<TResult>(TokenList(modifiers), declaration, action);
        }
        public VariableDeclarationPattern<TResult> VariableDeclaration(TypePattern<TResult> type = null, IEnumerable<VariableDeclaratorPattern<TResult>> variables = null, Func<TResult, VariableDeclarationSyntax, TResult> action = null)
        {
            return new VariableDeclarationPattern<TResult>(type, NodeList(variables), action);
        }

        public VariableDeclarationPattern<TResult> VariableDeclaration(params VariableDeclaratorPattern<TResult>[] variables)
        {
            return new VariableDeclarationPattern<TResult>(null, NodeList(variables), null);
        }
        public VariableDeclaratorPattern<TResult> VariableDeclarator(string identifier = null, BracketedArgumentListPattern<TResult> argumentList = null, EqualsValueClausePattern<TResult> initializer = null, Func<TResult, VariableDeclaratorSyntax, TResult> action = null)
        {
            return new VariableDeclaratorPattern<TResult>(identifier, argumentList, initializer, action);
        }
        public EqualsValueClausePattern<TResult> EqualsValueClause(ExpressionPattern<TResult> value = null, Func<TResult, EqualsValueClauseSyntax, TResult> action = null)
        {
            return new EqualsValueClausePattern<TResult>(value, action);
        }
        public SingleVariableDesignationPattern<TResult> SingleVariableDesignation(string identifier = null, Func<TResult, SingleVariableDesignationSyntax, TResult> action = null)
        {
            return new SingleVariableDesignationPattern<TResult>(identifier, action);
        }
        public DiscardDesignationPattern<TResult> DiscardDesignation(Func<TResult, DiscardDesignationSyntax, TResult> action = null)
        {
            return new DiscardDesignationPattern<TResult>(action);
        }
        public ParenthesizedVariableDesignationPattern<TResult> ParenthesizedVariableDesignation(IEnumerable<VariableDesignationPattern<TResult>> variables = null, Func<TResult, ParenthesizedVariableDesignationSyntax, TResult> action = null)
        {
            return new ParenthesizedVariableDesignationPattern<TResult>(NodeList(variables), action);
        }

        public ParenthesizedVariableDesignationPattern<TResult> ParenthesizedVariableDesignation(params VariableDesignationPattern<TResult>[] variables)
        {
            return new ParenthesizedVariableDesignationPattern<TResult>(NodeList(variables), null);
        }
        public ExpressionStatementPattern<TResult> ExpressionStatement(ExpressionPattern<TResult> expression = null, Func<TResult, ExpressionStatementSyntax, TResult> action = null)
        {
            return new ExpressionStatementPattern<TResult>(expression, action);
        }
        public EmptyStatementPattern<TResult> EmptyStatement(Func<TResult, EmptyStatementSyntax, TResult> action = null)
        {
            return new EmptyStatementPattern<TResult>(action);
        }
        public LabeledStatementPattern<TResult> LabeledStatement(string identifier = null, StatementPattern<TResult> statement = null, Func<TResult, LabeledStatementSyntax, TResult> action = null)
        {
            return new LabeledStatementPattern<TResult>(identifier, statement, action);
        }
        public GotoStatementPattern<TResult> GotoStatement(SyntaxKind kind = default(SyntaxKind), ExpressionPattern<TResult> expression = null, Func<TResult, GotoStatementSyntax, TResult> action = null)
        {
            return new GotoStatementPattern<TResult>(kind, expression, action);
        }
        public BreakStatementPattern<TResult> BreakStatement(Func<TResult, BreakStatementSyntax, TResult> action = null)
        {
            return new BreakStatementPattern<TResult>(action);
        }
        public ContinueStatementPattern<TResult> ContinueStatement(Func<TResult, ContinueStatementSyntax, TResult> action = null)
        {
            return new ContinueStatementPattern<TResult>(action);
        }
        public ReturnStatementPattern<TResult> ReturnStatement(ExpressionPattern<TResult> expression = null, Func<TResult, ReturnStatementSyntax, TResult> action = null)
        {
            return new ReturnStatementPattern<TResult>(expression, action);
        }
        public ThrowStatementPattern<TResult> ThrowStatement(ExpressionPattern<TResult> expression = null, Func<TResult, ThrowStatementSyntax, TResult> action = null)
        {
            return new ThrowStatementPattern<TResult>(expression, action);
        }
        public YieldStatementPattern<TResult> YieldStatement(SyntaxKind kind = default(SyntaxKind), ExpressionPattern<TResult> expression = null, Func<TResult, YieldStatementSyntax, TResult> action = null)
        {
            return new YieldStatementPattern<TResult>(kind, expression, action);
        }
        public WhileStatementPattern<TResult> WhileStatement(ExpressionPattern<TResult> condition = null, StatementPattern<TResult> statement = null, Func<TResult, WhileStatementSyntax, TResult> action = null)
        {
            return new WhileStatementPattern<TResult>(condition, statement, action);
        }
        public DoStatementPattern<TResult> DoStatement(StatementPattern<TResult> statement = null, ExpressionPattern<TResult> condition = null, Func<TResult, DoStatementSyntax, TResult> action = null)
        {
            return new DoStatementPattern<TResult>(statement, condition, action);
        }
        public ForStatementPattern<TResult> ForStatement(VariableDeclarationPattern<TResult> declaration = null, IEnumerable<ExpressionPattern<TResult>> initializers = null, ExpressionPattern<TResult> condition = null, IEnumerable<ExpressionPattern<TResult>> incrementors = null, StatementPattern<TResult> statement = null, Func<TResult, ForStatementSyntax, TResult> action = null)
        {
            return new ForStatementPattern<TResult>(declaration, NodeList(initializers), condition, NodeList(incrementors), statement, action);
        }
        public ForEachStatementPattern<TResult> ForEachStatement(ExpressionPattern<TResult> expression = null, StatementPattern<TResult> statement = null, TypePattern<TResult> type = null, string identifier = null, Func<TResult, ForEachStatementSyntax, TResult> action = null)
        {
            return new ForEachStatementPattern<TResult>(expression, statement, type, identifier, action);
        }
        public ForEachVariableStatementPattern<TResult> ForEachVariableStatement(ExpressionPattern<TResult> expression = null, StatementPattern<TResult> statement = null, ExpressionPattern<TResult> variable = null, Func<TResult, ForEachVariableStatementSyntax, TResult> action = null)
        {
            return new ForEachVariableStatementPattern<TResult>(expression, statement, variable, action);
        }
        public UsingStatementPattern<TResult> UsingStatement(VariableDeclarationPattern<TResult> declaration = null, ExpressionPattern<TResult> expression = null, StatementPattern<TResult> statement = null, Func<TResult, UsingStatementSyntax, TResult> action = null)
        {
            return new UsingStatementPattern<TResult>(declaration, expression, statement, action);
        }
        public FixedStatementPattern<TResult> FixedStatement(VariableDeclarationPattern<TResult> declaration = null, StatementPattern<TResult> statement = null, Func<TResult, FixedStatementSyntax, TResult> action = null)
        {
            return new FixedStatementPattern<TResult>(declaration, statement, action);
        }
        public CheckedStatementPattern<TResult> CheckedStatement(SyntaxKind kind = default(SyntaxKind), BlockPattern<TResult> block = null, Func<TResult, CheckedStatementSyntax, TResult> action = null)
        {
            return new CheckedStatementPattern<TResult>(kind, block, action);
        }
        public UnsafeStatementPattern<TResult> UnsafeStatement(BlockPattern<TResult> block = null, Func<TResult, UnsafeStatementSyntax, TResult> action = null)
        {
            return new UnsafeStatementPattern<TResult>(block, action);
        }
        public LockStatementPattern<TResult> LockStatement(ExpressionPattern<TResult> expression = null, StatementPattern<TResult> statement = null, Func<TResult, LockStatementSyntax, TResult> action = null)
        {
            return new LockStatementPattern<TResult>(expression, statement, action);
        }
        public IfStatementPattern<TResult> IfStatement(ExpressionPattern<TResult> condition = null, StatementPattern<TResult> statement = null, ElseClausePattern<TResult> @else = null, Func<TResult, IfStatementSyntax, TResult> action = null)
        {
            return new IfStatementPattern<TResult>(condition, statement, @else, action);
        }
        public ElseClausePattern<TResult> ElseClause(StatementPattern<TResult> statement = null, Func<TResult, ElseClauseSyntax, TResult> action = null)
        {
            return new ElseClausePattern<TResult>(statement, action);
        }
        public SwitchStatementPattern<TResult> SwitchStatement(ExpressionPattern<TResult> expression = null, IEnumerable<SwitchSectionPattern<TResult>> sections = null, Func<TResult, SwitchStatementSyntax, TResult> action = null)
        {
            return new SwitchStatementPattern<TResult>(expression, NodeList(sections), action);
        }

        public SwitchStatementPattern<TResult> SwitchStatement(params SwitchSectionPattern<TResult>[] sections)
        {
            return new SwitchStatementPattern<TResult>(null, NodeList(sections), null);
        }
        public SwitchSectionPattern<TResult> SwitchSection(IEnumerable<SwitchLabelPattern<TResult>> labels = null, IEnumerable<StatementPattern<TResult>> statements = null, Func<TResult, SwitchSectionSyntax, TResult> action = null)
        {
            return new SwitchSectionPattern<TResult>(NodeList(labels), NodeList(statements), action);
        }
        public CasePatternSwitchLabelPattern<TResult> CasePatternSwitchLabel(PatternPattern<TResult> pattern = null, WhenClausePattern<TResult> whenClause = null, Func<TResult, CasePatternSwitchLabelSyntax, TResult> action = null)
        {
            return new CasePatternSwitchLabelPattern<TResult>(pattern, whenClause, action);
        }
        public CaseSwitchLabelPattern<TResult> CaseSwitchLabel(ExpressionPattern<TResult> value = null, Func<TResult, CaseSwitchLabelSyntax, TResult> action = null)
        {
            return new CaseSwitchLabelPattern<TResult>(value, action);
        }
        public DefaultSwitchLabelPattern<TResult> DefaultSwitchLabel(Func<TResult, DefaultSwitchLabelSyntax, TResult> action = null)
        {
            return new DefaultSwitchLabelPattern<TResult>(action);
        }
        public TryStatementPattern<TResult> TryStatement(BlockPattern<TResult> block = null, IEnumerable<CatchClausePattern<TResult>> catches = null, FinallyClausePattern<TResult> @finally = null, Func<TResult, TryStatementSyntax, TResult> action = null)
        {
            return new TryStatementPattern<TResult>(block, NodeList(catches), @finally, action);
        }
        public CatchClausePattern<TResult> CatchClause(CatchDeclarationPattern<TResult> declaration = null, CatchFilterClausePattern<TResult> filter = null, BlockPattern<TResult> block = null, Func<TResult, CatchClauseSyntax, TResult> action = null)
        {
            return new CatchClausePattern<TResult>(declaration, filter, block, action);
        }
        public CatchDeclarationPattern<TResult> CatchDeclaration(TypePattern<TResult> type = null, string identifier = null, Func<TResult, CatchDeclarationSyntax, TResult> action = null)
        {
            return new CatchDeclarationPattern<TResult>(type, identifier, action);
        }
        public CatchFilterClausePattern<TResult> CatchFilterClause(ExpressionPattern<TResult> filterExpression = null, Func<TResult, CatchFilterClauseSyntax, TResult> action = null)
        {
            return new CatchFilterClausePattern<TResult>(filterExpression, action);
        }
        public FinallyClausePattern<TResult> FinallyClause(BlockPattern<TResult> block = null, Func<TResult, FinallyClauseSyntax, TResult> action = null)
        {
            return new FinallyClausePattern<TResult>(block, action);
        }
        public CompilationUnitPattern<TResult> CompilationUnit(IEnumerable<ExternAliasDirectivePattern<TResult>> externs = null, IEnumerable<UsingDirectivePattern<TResult>> usings = null, IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<MemberDeclarationPattern<TResult>> members = null, Func<TResult, CompilationUnitSyntax, TResult> action = null)
        {
            return new CompilationUnitPattern<TResult>(NodeList(externs), NodeList(usings), NodeList(attributeLists), NodeList(members), action);
        }
        public ExternAliasDirectivePattern<TResult> ExternAliasDirective(string identifier = null, Func<TResult, ExternAliasDirectiveSyntax, TResult> action = null)
        {
            return new ExternAliasDirectivePattern<TResult>(identifier, action);
        }
        public UsingDirectivePattern<TResult> UsingDirective(NameEqualsPattern<TResult> alias = null, NamePattern<TResult> name = null, Func<TResult, UsingDirectiveSyntax, TResult> action = null)
        {
            return new UsingDirectivePattern<TResult>(alias, name, action);
        }
        public NamespaceDeclarationPattern<TResult> NamespaceDeclaration(NamePattern<TResult> name = null, IEnumerable<ExternAliasDirectivePattern<TResult>> externs = null, IEnumerable<UsingDirectivePattern<TResult>> usings = null, IEnumerable<MemberDeclarationPattern<TResult>> members = null, Func<TResult, NamespaceDeclarationSyntax, TResult> action = null)
        {
            return new NamespaceDeclarationPattern<TResult>(name, NodeList(externs), NodeList(usings), NodeList(members), action);
        }
        public AttributeListPattern<TResult> AttributeList(AttributeTargetSpecifierPattern<TResult> target = null, IEnumerable<AttributePattern<TResult>> attributes = null, Func<TResult, AttributeListSyntax, TResult> action = null)
        {
            return new AttributeListPattern<TResult>(target, NodeList(attributes), action);
        }

        public AttributeListPattern<TResult> AttributeList(params AttributePattern<TResult>[] attributes)
        {
            return new AttributeListPattern<TResult>(null, NodeList(attributes), null);
        }
        public AttributeTargetSpecifierPattern<TResult> AttributeTargetSpecifier(string identifier = null, Func<TResult, AttributeTargetSpecifierSyntax, TResult> action = null)
        {
            return new AttributeTargetSpecifierPattern<TResult>(identifier, action);
        }
        public AttributePattern<TResult> Attribute(NamePattern<TResult> name = null, AttributeArgumentListPattern<TResult> argumentList = null, Func<TResult, AttributeSyntax, TResult> action = null)
        {
            return new AttributePattern<TResult>(name, argumentList, action);
        }
        public AttributeArgumentListPattern<TResult> AttributeArgumentList(IEnumerable<AttributeArgumentPattern<TResult>> arguments = null, Func<TResult, AttributeArgumentListSyntax, TResult> action = null)
        {
            return new AttributeArgumentListPattern<TResult>(NodeList(arguments), action);
        }

        public AttributeArgumentListPattern<TResult> AttributeArgumentList(params AttributeArgumentPattern<TResult>[] arguments)
        {
            return new AttributeArgumentListPattern<TResult>(NodeList(arguments), null);
        }
        public AttributeArgumentPattern<TResult> AttributeArgument(NameEqualsPattern<TResult> nameEquals = null, NameColonPattern<TResult> nameColon = null, ExpressionPattern<TResult> expression = null, Func<TResult, AttributeArgumentSyntax, TResult> action = null)
        {
            return new AttributeArgumentPattern<TResult>(nameEquals, nameColon, expression, action);
        }
        public NameEqualsPattern<TResult> NameEquals(IdentifierNamePattern<TResult> name = null, Func<TResult, NameEqualsSyntax, TResult> action = null)
        {
            return new NameEqualsPattern<TResult>(name, action);
        }
        public TypeParameterListPattern<TResult> TypeParameterList(IEnumerable<TypeParameterPattern<TResult>> parameters = null, Func<TResult, TypeParameterListSyntax, TResult> action = null)
        {
            return new TypeParameterListPattern<TResult>(NodeList(parameters), action);
        }

        public TypeParameterListPattern<TResult> TypeParameterList(params TypeParameterPattern<TResult>[] parameters)
        {
            return new TypeParameterListPattern<TResult>(NodeList(parameters), null);
        }
        public TypeParameterPattern<TResult> TypeParameter(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, string identifier = null, Func<TResult, TypeParameterSyntax, TResult> action = null)
        {
            return new TypeParameterPattern<TResult>(NodeList(attributeLists), identifier, action);
        }
        public ClassDeclarationPattern<TResult> ClassDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern<TResult> baseList = null, TypeParameterListPattern<TResult> typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern<TResult>> constraintClauses = null, IEnumerable<MemberDeclarationPattern<TResult>> members = null, Func<TResult, ClassDeclarationSyntax, TResult> action = null)
        {
            return new ClassDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, typeParameterList, NodeList(constraintClauses), NodeList(members), action);
        }

        public ClassDeclarationPattern<TResult> ClassDeclaration(params MemberDeclarationPattern<TResult>[] members)
        {
            return new ClassDeclarationPattern<TResult>(null, null, null, null, null, null, NodeList(members), null);
        }
        public StructDeclarationPattern<TResult> StructDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern<TResult> baseList = null, TypeParameterListPattern<TResult> typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern<TResult>> constraintClauses = null, IEnumerable<MemberDeclarationPattern<TResult>> members = null, Func<TResult, StructDeclarationSyntax, TResult> action = null)
        {
            return new StructDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, typeParameterList, NodeList(constraintClauses), NodeList(members), action);
        }

        public StructDeclarationPattern<TResult> StructDeclaration(params MemberDeclarationPattern<TResult>[] members)
        {
            return new StructDeclarationPattern<TResult>(null, null, null, null, null, null, NodeList(members), null);
        }
        public InterfaceDeclarationPattern<TResult> InterfaceDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern<TResult> baseList = null, TypeParameterListPattern<TResult> typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern<TResult>> constraintClauses = null, IEnumerable<MemberDeclarationPattern<TResult>> members = null, Func<TResult, InterfaceDeclarationSyntax, TResult> action = null)
        {
            return new InterfaceDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, typeParameterList, NodeList(constraintClauses), NodeList(members), action);
        }

        public InterfaceDeclarationPattern<TResult> InterfaceDeclaration(params MemberDeclarationPattern<TResult>[] members)
        {
            return new InterfaceDeclarationPattern<TResult>(null, null, null, null, null, null, NodeList(members), null);
        }
        public EnumDeclarationPattern<TResult> EnumDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern<TResult> baseList = null, IEnumerable<EnumMemberDeclarationPattern<TResult>> members = null, Func<TResult, EnumDeclarationSyntax, TResult> action = null)
        {
            return new EnumDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, NodeList(members), action);
        }

        public EnumDeclarationPattern<TResult> EnumDeclaration(params EnumMemberDeclarationPattern<TResult>[] members)
        {
            return new EnumDeclarationPattern<TResult>(null, null, null, null, NodeList(members), null);
        }
        public DelegateDeclarationPattern<TResult> DelegateDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern<TResult> returnType = null, string identifier = null, TypeParameterListPattern<TResult> typeParameterList = null, ParameterListPattern<TResult> parameterList = null, IEnumerable<TypeParameterConstraintClausePattern<TResult>> constraintClauses = null, Func<TResult, DelegateDeclarationSyntax, TResult> action = null)
        {
            return new DelegateDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), returnType, identifier, typeParameterList, parameterList, NodeList(constraintClauses), action);
        }
        public EnumMemberDeclarationPattern<TResult> EnumMemberDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, string identifier = null, EqualsValueClausePattern<TResult> equalsValue = null, Func<TResult, EnumMemberDeclarationSyntax, TResult> action = null)
        {
            return new EnumMemberDeclarationPattern<TResult>(NodeList(attributeLists), identifier, equalsValue, action);
        }
        public BaseListPattern<TResult> BaseList(IEnumerable<BaseTypePattern<TResult>> types = null, Func<TResult, BaseListSyntax, TResult> action = null)
        {
            return new BaseListPattern<TResult>(NodeList(types), action);
        }

        public BaseListPattern<TResult> BaseList(params BaseTypePattern<TResult>[] types)
        {
            return new BaseListPattern<TResult>(NodeList(types), null);
        }
        public SimpleBaseTypePattern<TResult> SimpleBaseType(TypePattern<TResult> type = null, Func<TResult, SimpleBaseTypeSyntax, TResult> action = null)
        {
            return new SimpleBaseTypePattern<TResult>(type, action);
        }
        public TypeParameterConstraintClausePattern<TResult> TypeParameterConstraintClause(IdentifierNamePattern<TResult> name = null, IEnumerable<TypeParameterConstraintPattern<TResult>> constraints = null, Func<TResult, TypeParameterConstraintClauseSyntax, TResult> action = null)
        {
            return new TypeParameterConstraintClausePattern<TResult>(name, NodeList(constraints), action);
        }
        public ConstructorConstraintPattern<TResult> ConstructorConstraint(Func<TResult, ConstructorConstraintSyntax, TResult> action = null)
        {
            return new ConstructorConstraintPattern<TResult>(action);
        }
        public ClassOrStructConstraintPattern<TResult> ClassOrStructConstraint(SyntaxKind kind = default(SyntaxKind), Func<TResult, ClassOrStructConstraintSyntax, TResult> action = null)
        {
            return new ClassOrStructConstraintPattern<TResult>(kind, action);
        }
        public TypeConstraintPattern<TResult> TypeConstraint(TypePattern<TResult> type = null, Func<TResult, TypeConstraintSyntax, TResult> action = null)
        {
            return new TypeConstraintPattern<TResult>(type, action);
        }
        public FieldDeclarationPattern<TResult> FieldDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, VariableDeclarationPattern<TResult> declaration = null, Func<TResult, FieldDeclarationSyntax, TResult> action = null)
        {
            return new FieldDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), declaration, action);
        }
        public EventFieldDeclarationPattern<TResult> EventFieldDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, VariableDeclarationPattern<TResult> declaration = null, Func<TResult, EventFieldDeclarationSyntax, TResult> action = null)
        {
            return new EventFieldDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), declaration, action);
        }
        public ExplicitInterfaceSpecifierPattern<TResult> ExplicitInterfaceSpecifier(NamePattern<TResult> name = null, Func<TResult, ExplicitInterfaceSpecifierSyntax, TResult> action = null)
        {
            return new ExplicitInterfaceSpecifierPattern<TResult>(name, action);
        }
        public MethodDeclarationPattern<TResult> MethodDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern<TResult> parameterList = null, BlockPattern<TResult> body = null, ArrowExpressionClausePattern<TResult> expressionBody = null, TypePattern<TResult> returnType = null, ExplicitInterfaceSpecifierPattern<TResult> explicitInterfaceSpecifier = null, string identifier = null, TypeParameterListPattern<TResult> typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern<TResult>> constraintClauses = null, Func<TResult, MethodDeclarationSyntax, TResult> action = null)
        {
            return new MethodDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), parameterList, body, expressionBody, returnType, explicitInterfaceSpecifier, identifier, typeParameterList, NodeList(constraintClauses), action);
        }
        public OperatorDeclarationPattern<TResult> OperatorDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern<TResult> parameterList = null, BlockPattern<TResult> body = null, ArrowExpressionClausePattern<TResult> expressionBody = null, TypePattern<TResult> returnType = null, Func<TResult, OperatorDeclarationSyntax, TResult> action = null)
        {
            return new OperatorDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), parameterList, body, expressionBody, returnType, action);
        }
        public ConversionOperatorDeclarationPattern<TResult> ConversionOperatorDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern<TResult> parameterList = null, BlockPattern<TResult> body = null, ArrowExpressionClausePattern<TResult> expressionBody = null, TypePattern<TResult> type = null, Func<TResult, ConversionOperatorDeclarationSyntax, TResult> action = null)
        {
            return new ConversionOperatorDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), parameterList, body, expressionBody, type, action);
        }
        public ConstructorDeclarationPattern<TResult> ConstructorDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern<TResult> parameterList = null, BlockPattern<TResult> body = null, ArrowExpressionClausePattern<TResult> expressionBody = null, string identifier = null, ConstructorInitializerPattern<TResult> initializer = null, Func<TResult, ConstructorDeclarationSyntax, TResult> action = null)
        {
            return new ConstructorDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), parameterList, body, expressionBody, identifier, initializer, action);
        }
        public ConstructorInitializerPattern<TResult> ConstructorInitializer(SyntaxKind kind = default(SyntaxKind), ArgumentListPattern<TResult> argumentList = null, Func<TResult, ConstructorInitializerSyntax, TResult> action = null)
        {
            return new ConstructorInitializerPattern<TResult>(kind, argumentList, action);
        }
        public DestructorDeclarationPattern<TResult> DestructorDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern<TResult> parameterList = null, BlockPattern<TResult> body = null, ArrowExpressionClausePattern<TResult> expressionBody = null, string identifier = null, Func<TResult, DestructorDeclarationSyntax, TResult> action = null)
        {
            return new DestructorDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), parameterList, body, expressionBody, identifier, action);
        }
        public PropertyDeclarationPattern<TResult> PropertyDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern<TResult> type = null, ExplicitInterfaceSpecifierPattern<TResult> explicitInterfaceSpecifier = null, AccessorListPattern<TResult> accessorList = null, string identifier = null, ArrowExpressionClausePattern<TResult> expressionBody = null, EqualsValueClausePattern<TResult> initializer = null, Func<TResult, PropertyDeclarationSyntax, TResult> action = null)
        {
            return new PropertyDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), type, explicitInterfaceSpecifier, accessorList, identifier, expressionBody, initializer, action);
        }
        public ArrowExpressionClausePattern<TResult> ArrowExpressionClause(ExpressionPattern<TResult> expression = null, Func<TResult, ArrowExpressionClauseSyntax, TResult> action = null)
        {
            return new ArrowExpressionClausePattern<TResult>(expression, action);
        }
        public EventDeclarationPattern<TResult> EventDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern<TResult> type = null, ExplicitInterfaceSpecifierPattern<TResult> explicitInterfaceSpecifier = null, AccessorListPattern<TResult> accessorList = null, string identifier = null, Func<TResult, EventDeclarationSyntax, TResult> action = null)
        {
            return new EventDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), type, explicitInterfaceSpecifier, accessorList, identifier, action);
        }
        public IndexerDeclarationPattern<TResult> IndexerDeclaration(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern<TResult> type = null, ExplicitInterfaceSpecifierPattern<TResult> explicitInterfaceSpecifier = null, AccessorListPattern<TResult> accessorList = null, BracketedParameterListPattern<TResult> parameterList = null, ArrowExpressionClausePattern<TResult> expressionBody = null, Func<TResult, IndexerDeclarationSyntax, TResult> action = null)
        {
            return new IndexerDeclarationPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), type, explicitInterfaceSpecifier, accessorList, parameterList, expressionBody, action);
        }
        public AccessorListPattern<TResult> AccessorList(IEnumerable<AccessorDeclarationPattern<TResult>> accessors = null, Func<TResult, AccessorListSyntax, TResult> action = null)
        {
            return new AccessorListPattern<TResult>(NodeList(accessors), action);
        }

        public AccessorListPattern<TResult> AccessorList(params AccessorDeclarationPattern<TResult>[] accessors)
        {
            return new AccessorListPattern<TResult>(NodeList(accessors), null);
        }
        public AccessorDeclarationPattern<TResult> AccessorDeclaration(SyntaxKind kind = default(SyntaxKind), IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, BlockPattern<TResult> body = null, ArrowExpressionClausePattern<TResult> expressionBody = null, Func<TResult, AccessorDeclarationSyntax, TResult> action = null)
        {
            return new AccessorDeclarationPattern<TResult>(kind, NodeList(attributeLists), TokenList(modifiers), body, expressionBody, action);
        }
        public ParameterListPattern<TResult> ParameterList(IEnumerable<ParameterPattern<TResult>> parameters = null, Func<TResult, ParameterListSyntax, TResult> action = null)
        {
            return new ParameterListPattern<TResult>(NodeList(parameters), action);
        }

        public ParameterListPattern<TResult> ParameterList(params ParameterPattern<TResult>[] parameters)
        {
            return new ParameterListPattern<TResult>(NodeList(parameters), null);
        }
        public BracketedParameterListPattern<TResult> BracketedParameterList(IEnumerable<ParameterPattern<TResult>> parameters = null, Func<TResult, BracketedParameterListSyntax, TResult> action = null)
        {
            return new BracketedParameterListPattern<TResult>(NodeList(parameters), action);
        }

        public BracketedParameterListPattern<TResult> BracketedParameterList(params ParameterPattern<TResult>[] parameters)
        {
            return new BracketedParameterListPattern<TResult>(NodeList(parameters), null);
        }
        public ParameterPattern<TResult> Parameter(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern<TResult> type = null, string identifier = null, EqualsValueClausePattern<TResult> @default = null, Func<TResult, ParameterSyntax, TResult> action = null)
        {
            return new ParameterPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), type, identifier, @default, action);
        }
        public IncompleteMemberPattern<TResult> IncompleteMember(IEnumerable<AttributeListPattern<TResult>> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern<TResult> type = null, Func<TResult, IncompleteMemberSyntax, TResult> action = null)
        {
            return new IncompleteMemberPattern<TResult>(NodeList(attributeLists), TokenList(modifiers), type, action);
        }
    }
    public abstract partial class NamePattern : TypePattern
    {

        internal NamePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NameSyntax typed))
                return false;


            return true;
        }
    }

    public abstract partial class SimpleNamePattern : NamePattern
    {
        private readonly string _identifier;

        internal SimpleNamePattern(string identifier)
        {
            _identifier = identifier;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SimpleNameSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }
    }

    public partial class IdentifierNamePattern : SimpleNamePattern
    {
        private readonly Action<IdentifierNameSyntax> _action;

        internal IdentifierNamePattern(string identifier, Action<IdentifierNameSyntax> action)
            : base(identifier)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IdentifierNameSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (IdentifierNameSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class QualifiedNamePattern : NamePattern
    {
        private readonly NamePattern _left;
        private readonly SimpleNamePattern _right;
        private readonly Action<QualifiedNameSyntax> _action;

        internal QualifiedNamePattern(NamePattern left, SimpleNamePattern right, Action<QualifiedNameSyntax> action)
        {
            _left = left;
            _right = right;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QualifiedNameSyntax typed))
                return false;

            if (_left != null && !_left.Test(typed.Left, semanticModel))
                return false;
            if (_right != null && !_right.Test(typed.Right, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QualifiedNameSyntax)node;

            if (_left != null)
                _left.RunCallback(typed.Left, semanticModel);
            if (_right != null)
                _right.RunCallback(typed.Right, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class GenericNamePattern : SimpleNamePattern
    {
        private readonly TypeArgumentListPattern _typeArgumentList;
        private readonly Action<GenericNameSyntax> _action;

        internal GenericNamePattern(string identifier, TypeArgumentListPattern typeArgumentList, Action<GenericNameSyntax> action)
            : base(identifier)
        {
            _typeArgumentList = typeArgumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GenericNameSyntax typed))
                return false;

            if (_typeArgumentList != null && !_typeArgumentList.Test(typed.TypeArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (GenericNameSyntax)node;

            if (_typeArgumentList != null)
                _typeArgumentList.RunCallback(typed.TypeArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class TypeArgumentListPattern : PatternNode
    {
        private readonly NodeListPattern<TypePattern> _arguments;
        private readonly Action<TypeArgumentListSyntax> _action;

        internal TypeArgumentListPattern(NodeListPattern<TypePattern> arguments, Action<TypeArgumentListSyntax> action)
        {
            _arguments = arguments;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeArgumentListSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeArgumentListSyntax)node;

            if (_arguments != null)
                _arguments.RunCallback(typed.Arguments, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AliasQualifiedNamePattern : NamePattern
    {
        private readonly IdentifierNamePattern _alias;
        private readonly SimpleNamePattern _name;
        private readonly Action<AliasQualifiedNameSyntax> _action;

        internal AliasQualifiedNamePattern(IdentifierNamePattern alias, SimpleNamePattern name, Action<AliasQualifiedNameSyntax> action)
        {
            _alias = alias;
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AliasQualifiedNameSyntax typed))
                return false;

            if (_alias != null && !_alias.Test(typed.Alias, semanticModel))
                return false;
            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AliasQualifiedNameSyntax)node;

            if (_alias != null)
                _alias.RunCallback(typed.Alias, semanticModel);
            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class TypePattern : ExpressionPattern
    {

        internal TypePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeSyntax typed))
                return false;


            return true;
        }
    }

    public partial class PredefinedTypePattern : TypePattern
    {
        private readonly string _keyword;
        private readonly Action<PredefinedTypeSyntax> _action;

        internal PredefinedTypePattern(string keyword, Action<PredefinedTypeSyntax> action)
        {
            _keyword = keyword;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PredefinedTypeSyntax typed))
                return false;

            if (_keyword != null && _keyword != typed.Keyword.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PredefinedTypeSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class ArrayTypePattern : TypePattern
    {
        private readonly TypePattern _elementType;
        private readonly NodeListPattern<ArrayRankSpecifierPattern> _rankSpecifiers;
        private readonly Action<ArrayTypeSyntax> _action;

        internal ArrayTypePattern(TypePattern elementType, NodeListPattern<ArrayRankSpecifierPattern> rankSpecifiers, Action<ArrayTypeSyntax> action)
        {
            _elementType = elementType;
            _rankSpecifiers = rankSpecifiers;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrayTypeSyntax typed))
                return false;

            if (_elementType != null && !_elementType.Test(typed.ElementType, semanticModel))
                return false;
            if (_rankSpecifiers != null && !_rankSpecifiers.Test(typed.RankSpecifiers, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrayTypeSyntax)node;

            if (_elementType != null)
                _elementType.RunCallback(typed.ElementType, semanticModel);
            if (_rankSpecifiers != null)
                _rankSpecifiers.RunCallback(typed.RankSpecifiers, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ArrayRankSpecifierPattern : PatternNode
    {
        private readonly NodeListPattern<ExpressionPattern> _sizes;
        private readonly Action<ArrayRankSpecifierSyntax> _action;

        internal ArrayRankSpecifierPattern(NodeListPattern<ExpressionPattern> sizes, Action<ArrayRankSpecifierSyntax> action)
        {
            _sizes = sizes;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrayRankSpecifierSyntax typed))
                return false;

            if (_sizes != null && !_sizes.Test(typed.Sizes, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrayRankSpecifierSyntax)node;

            if (_sizes != null)
                _sizes.RunCallback(typed.Sizes, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class PointerTypePattern : TypePattern
    {
        private readonly TypePattern _elementType;
        private readonly Action<PointerTypeSyntax> _action;

        internal PointerTypePattern(TypePattern elementType, Action<PointerTypeSyntax> action)
        {
            _elementType = elementType;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PointerTypeSyntax typed))
                return false;

            if (_elementType != null && !_elementType.Test(typed.ElementType, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PointerTypeSyntax)node;

            if (_elementType != null)
                _elementType.RunCallback(typed.ElementType, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class NullableTypePattern : TypePattern
    {
        private readonly TypePattern _elementType;
        private readonly Action<NullableTypeSyntax> _action;

        internal NullableTypePattern(TypePattern elementType, Action<NullableTypeSyntax> action)
        {
            _elementType = elementType;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NullableTypeSyntax typed))
                return false;

            if (_elementType != null && !_elementType.Test(typed.ElementType, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NullableTypeSyntax)node;

            if (_elementType != null)
                _elementType.RunCallback(typed.ElementType, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class TupleTypePattern : TypePattern
    {
        private readonly NodeListPattern<TupleElementPattern> _elements;
        private readonly Action<TupleTypeSyntax> _action;

        internal TupleTypePattern(NodeListPattern<TupleElementPattern> elements, Action<TupleTypeSyntax> action)
        {
            _elements = elements;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TupleTypeSyntax typed))
                return false;

            if (_elements != null && !_elements.Test(typed.Elements, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TupleTypeSyntax)node;

            if (_elements != null)
                _elements.RunCallback(typed.Elements, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class TupleElementPattern : PatternNode
    {
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly Action<TupleElementSyntax> _action;

        internal TupleElementPattern(TypePattern type, string identifier, Action<TupleElementSyntax> action)
        {
            _type = type;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TupleElementSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TupleElementSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class OmittedTypeArgumentPattern : TypePattern
    {
        private readonly Action<OmittedTypeArgumentSyntax> _action;

        internal OmittedTypeArgumentPattern(Action<OmittedTypeArgumentSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OmittedTypeArgumentSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OmittedTypeArgumentSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class RefTypePattern : TypePattern
    {
        private readonly TypePattern _type;
        private readonly Action<RefTypeSyntax> _action;

        internal RefTypePattern(TypePattern type, Action<RefTypeSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefTypeSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefTypeSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class ExpressionPattern : PatternNode
    {

        internal ExpressionPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExpressionSyntax typed))
                return false;


            return true;
        }
    }

    public partial class ParenthesizedExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ParenthesizedExpressionSyntax> _action;

        internal ParenthesizedExpressionPattern(ExpressionPattern expression, Action<ParenthesizedExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParenthesizedExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParenthesizedExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class TupleExpressionPattern : ExpressionPattern
    {
        private readonly NodeListPattern<ArgumentPattern> _arguments;
        private readonly Action<TupleExpressionSyntax> _action;

        internal TupleExpressionPattern(NodeListPattern<ArgumentPattern> arguments, Action<TupleExpressionSyntax> action)
        {
            _arguments = arguments;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TupleExpressionSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TupleExpressionSyntax)node;

            if (_arguments != null)
                _arguments.RunCallback(typed.Arguments, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class PrefixUnaryExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _operand;
        private readonly Action<PrefixUnaryExpressionSyntax> _action;

        internal PrefixUnaryExpressionPattern(SyntaxKind kind, ExpressionPattern operand, Action<PrefixUnaryExpressionSyntax> action)
        {
            _kind = kind;
            _operand = operand;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PrefixUnaryExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_operand != null && !_operand.Test(typed.Operand, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PrefixUnaryExpressionSyntax)node;

            if (_operand != null)
                _operand.RunCallback(typed.Operand, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AwaitExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<AwaitExpressionSyntax> _action;

        internal AwaitExpressionPattern(ExpressionPattern expression, Action<AwaitExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AwaitExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AwaitExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class PostfixUnaryExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _operand;
        private readonly Action<PostfixUnaryExpressionSyntax> _action;

        internal PostfixUnaryExpressionPattern(SyntaxKind kind, ExpressionPattern operand, Action<PostfixUnaryExpressionSyntax> action)
        {
            _kind = kind;
            _operand = operand;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PostfixUnaryExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_operand != null && !_operand.Test(typed.Operand, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PostfixUnaryExpressionSyntax)node;

            if (_operand != null)
                _operand.RunCallback(typed.Operand, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class MemberAccessExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _expression;
        private readonly SimpleNamePattern _name;
        private readonly Action<MemberAccessExpressionSyntax> _action;

        internal MemberAccessExpressionPattern(SyntaxKind kind, ExpressionPattern expression, SimpleNamePattern name, Action<MemberAccessExpressionSyntax> action)
        {
            _kind = kind;
            _expression = expression;
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MemberAccessExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (MemberAccessExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ConditionalAccessExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly ExpressionPattern _whenNotNull;
        private readonly Action<ConditionalAccessExpressionSyntax> _action;

        internal ConditionalAccessExpressionPattern(ExpressionPattern expression, ExpressionPattern whenNotNull, Action<ConditionalAccessExpressionSyntax> action)
        {
            _expression = expression;
            _whenNotNull = whenNotNull;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConditionalAccessExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_whenNotNull != null && !_whenNotNull.Test(typed.WhenNotNull, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConditionalAccessExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_whenNotNull != null)
                _whenNotNull.RunCallback(typed.WhenNotNull, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class MemberBindingExpressionPattern : ExpressionPattern
    {
        private readonly SimpleNamePattern _name;
        private readonly Action<MemberBindingExpressionSyntax> _action;

        internal MemberBindingExpressionPattern(SimpleNamePattern name, Action<MemberBindingExpressionSyntax> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MemberBindingExpressionSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (MemberBindingExpressionSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ElementBindingExpressionPattern : ExpressionPattern
    {
        private readonly BracketedArgumentListPattern _argumentList;
        private readonly Action<ElementBindingExpressionSyntax> _action;

        internal ElementBindingExpressionPattern(BracketedArgumentListPattern argumentList, Action<ElementBindingExpressionSyntax> action)
        {
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ElementBindingExpressionSyntax typed))
                return false;

            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ElementBindingExpressionSyntax)node;

            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ImplicitElementAccessPattern : ExpressionPattern
    {
        private readonly BracketedArgumentListPattern _argumentList;
        private readonly Action<ImplicitElementAccessSyntax> _action;

        internal ImplicitElementAccessPattern(BracketedArgumentListPattern argumentList, Action<ImplicitElementAccessSyntax> action)
        {
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ImplicitElementAccessSyntax typed))
                return false;

            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ImplicitElementAccessSyntax)node;

            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class BinaryExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _left;
        private readonly ExpressionPattern _right;
        private readonly Action<BinaryExpressionSyntax> _action;

        internal BinaryExpressionPattern(SyntaxKind kind, ExpressionPattern left, ExpressionPattern right, Action<BinaryExpressionSyntax> action)
        {
            _kind = kind;
            _left = left;
            _right = right;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BinaryExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_left != null && !_left.Test(typed.Left, semanticModel))
                return false;
            if (_right != null && !_right.Test(typed.Right, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BinaryExpressionSyntax)node;

            if (_left != null)
                _left.RunCallback(typed.Left, semanticModel);
            if (_right != null)
                _right.RunCallback(typed.Right, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AssignmentExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _left;
        private readonly ExpressionPattern _right;
        private readonly Action<AssignmentExpressionSyntax> _action;

        internal AssignmentExpressionPattern(SyntaxKind kind, ExpressionPattern left, ExpressionPattern right, Action<AssignmentExpressionSyntax> action)
        {
            _kind = kind;
            _left = left;
            _right = right;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AssignmentExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_left != null && !_left.Test(typed.Left, semanticModel))
                return false;
            if (_right != null && !_right.Test(typed.Right, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AssignmentExpressionSyntax)node;

            if (_left != null)
                _left.RunCallback(typed.Left, semanticModel);
            if (_right != null)
                _right.RunCallback(typed.Right, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ConditionalExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _condition;
        private readonly ExpressionPattern _whenTrue;
        private readonly ExpressionPattern _whenFalse;
        private readonly Action<ConditionalExpressionSyntax> _action;

        internal ConditionalExpressionPattern(ExpressionPattern condition, ExpressionPattern whenTrue, ExpressionPattern whenFalse, Action<ConditionalExpressionSyntax> action)
        {
            _condition = condition;
            _whenTrue = whenTrue;
            _whenFalse = whenFalse;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConditionalExpressionSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_whenTrue != null && !_whenTrue.Test(typed.WhenTrue, semanticModel))
                return false;
            if (_whenFalse != null && !_whenFalse.Test(typed.WhenFalse, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConditionalExpressionSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);
            if (_whenTrue != null)
                _whenTrue.RunCallback(typed.WhenTrue, semanticModel);
            if (_whenFalse != null)
                _whenFalse.RunCallback(typed.WhenFalse, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class InstanceExpressionPattern : ExpressionPattern
    {

        internal InstanceExpressionPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InstanceExpressionSyntax typed))
                return false;


            return true;
        }
    }

    public partial class ThisExpressionPattern : InstanceExpressionPattern
    {
        private readonly Action<ThisExpressionSyntax> _action;

        internal ThisExpressionPattern(Action<ThisExpressionSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ThisExpressionSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ThisExpressionSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class BaseExpressionPattern : InstanceExpressionPattern
    {
        private readonly Action<BaseExpressionSyntax> _action;

        internal BaseExpressionPattern(Action<BaseExpressionSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseExpressionSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseExpressionSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class LiteralExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly Action<LiteralExpressionSyntax> _action;

        internal LiteralExpressionPattern(SyntaxKind kind, Action<LiteralExpressionSyntax> action)
        {
            _kind = kind;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LiteralExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LiteralExpressionSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class MakeRefExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<MakeRefExpressionSyntax> _action;

        internal MakeRefExpressionPattern(ExpressionPattern expression, Action<MakeRefExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MakeRefExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (MakeRefExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class RefTypeExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<RefTypeExpressionSyntax> _action;

        internal RefTypeExpressionPattern(ExpressionPattern expression, Action<RefTypeExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefTypeExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefTypeExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class RefValueExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly TypePattern _type;
        private readonly Action<RefValueExpressionSyntax> _action;

        internal RefValueExpressionPattern(ExpressionPattern expression, TypePattern type, Action<RefValueExpressionSyntax> action)
        {
            _expression = expression;
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefValueExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefValueExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class CheckedExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _expression;
        private readonly Action<CheckedExpressionSyntax> _action;

        internal CheckedExpressionPattern(SyntaxKind kind, ExpressionPattern expression, Action<CheckedExpressionSyntax> action)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CheckedExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CheckedExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class DefaultExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly Action<DefaultExpressionSyntax> _action;

        internal DefaultExpressionPattern(TypePattern type, Action<DefaultExpressionSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DefaultExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DefaultExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class TypeOfExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly Action<TypeOfExpressionSyntax> _action;

        internal TypeOfExpressionPattern(TypePattern type, Action<TypeOfExpressionSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeOfExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeOfExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class SizeOfExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly Action<SizeOfExpressionSyntax> _action;

        internal SizeOfExpressionPattern(TypePattern type, Action<SizeOfExpressionSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SizeOfExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SizeOfExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class InvocationExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly ArgumentListPattern _argumentList;
        private readonly Action<InvocationExpressionSyntax> _action;

        internal InvocationExpressionPattern(ExpressionPattern expression, ArgumentListPattern argumentList, Action<InvocationExpressionSyntax> action)
        {
            _expression = expression;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InvocationExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InvocationExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ElementAccessExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly BracketedArgumentListPattern _argumentList;
        private readonly Action<ElementAccessExpressionSyntax> _action;

        internal ElementAccessExpressionPattern(ExpressionPattern expression, BracketedArgumentListPattern argumentList, Action<ElementAccessExpressionSyntax> action)
        {
            _expression = expression;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ElementAccessExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ElementAccessExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class BaseArgumentListPattern : PatternNode
    {
        private readonly NodeListPattern<ArgumentPattern> _arguments;

        internal BaseArgumentListPattern(NodeListPattern<ArgumentPattern> arguments)
        {
            _arguments = arguments;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseArgumentListSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseArgumentListSyntax)node;

            if (_arguments != null)
                _arguments.RunCallback(typed.Arguments, semanticModel);
        }
    }

    public partial class ArgumentListPattern : BaseArgumentListPattern
    {
        private readonly Action<ArgumentListSyntax> _action;

        internal ArgumentListPattern(NodeListPattern<ArgumentPattern> arguments, Action<ArgumentListSyntax> action)
            : base(arguments)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArgumentListSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ArgumentListSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class BracketedArgumentListPattern : BaseArgumentListPattern
    {
        private readonly Action<BracketedArgumentListSyntax> _action;

        internal BracketedArgumentListPattern(NodeListPattern<ArgumentPattern> arguments, Action<BracketedArgumentListSyntax> action)
            : base(arguments)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BracketedArgumentListSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (BracketedArgumentListSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class ArgumentPattern : PatternNode
    {
        private readonly NameColonPattern _nameColon;
        private readonly ExpressionPattern _expression;
        private readonly Action<ArgumentSyntax> _action;

        internal ArgumentPattern(NameColonPattern nameColon, ExpressionPattern expression, Action<ArgumentSyntax> action)
        {
            _nameColon = nameColon;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArgumentSyntax typed))
                return false;

            if (_nameColon != null && !_nameColon.Test(typed.NameColon, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArgumentSyntax)node;

            if (_nameColon != null)
                _nameColon.RunCallback(typed.NameColon, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class NameColonPattern : PatternNode
    {
        private readonly IdentifierNamePattern _name;
        private readonly Action<NameColonSyntax> _action;

        internal NameColonPattern(IdentifierNamePattern name, Action<NameColonSyntax> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NameColonSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NameColonSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class DeclarationExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly VariableDesignationPattern _designation;
        private readonly Action<DeclarationExpressionSyntax> _action;

        internal DeclarationExpressionPattern(TypePattern type, VariableDesignationPattern designation, Action<DeclarationExpressionSyntax> action)
        {
            _type = type;
            _designation = designation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DeclarationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_designation != null && !_designation.Test(typed.Designation, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DeclarationExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_designation != null)
                _designation.RunCallback(typed.Designation, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class CastExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly ExpressionPattern _expression;
        private readonly Action<CastExpressionSyntax> _action;

        internal CastExpressionPattern(TypePattern type, ExpressionPattern expression, Action<CastExpressionSyntax> action)
        {
            _type = type;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CastExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CastExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class AnonymousFunctionExpressionPattern : ExpressionPattern
    {
        private readonly PatternNode _body;

        internal AnonymousFunctionExpressionPattern(PatternNode body)
        {
            _body = body;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousFunctionExpressionSyntax typed))
                return false;

            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AnonymousFunctionExpressionSyntax)node;

            if (_body != null)
                _body.RunCallback(typed.Body, semanticModel);
        }
    }

    public partial class AnonymousMethodExpressionPattern : AnonymousFunctionExpressionPattern
    {
        private readonly ParameterListPattern _parameterList;
        private readonly Action<AnonymousMethodExpressionSyntax> _action;

        internal AnonymousMethodExpressionPattern(PatternNode body, ParameterListPattern parameterList, Action<AnonymousMethodExpressionSyntax> action)
            : base(body)
        {
            _parameterList = parameterList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousMethodExpressionSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (AnonymousMethodExpressionSyntax)node;

            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class LambdaExpressionPattern : AnonymousFunctionExpressionPattern
    {

        internal LambdaExpressionPattern(PatternNode body)
            : base(body)
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LambdaExpressionSyntax typed))
                return false;


            return true;
        }
    }

    public partial class SimpleLambdaExpressionPattern : LambdaExpressionPattern
    {
        private readonly ParameterPattern _parameter;
        private readonly Action<SimpleLambdaExpressionSyntax> _action;

        internal SimpleLambdaExpressionPattern(PatternNode body, ParameterPattern parameter, Action<SimpleLambdaExpressionSyntax> action)
            : base(body)
        {
            _parameter = parameter;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SimpleLambdaExpressionSyntax typed))
                return false;

            if (_parameter != null && !_parameter.Test(typed.Parameter, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SimpleLambdaExpressionSyntax)node;

            if (_parameter != null)
                _parameter.RunCallback(typed.Parameter, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class RefExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<RefExpressionSyntax> _action;

        internal RefExpressionPattern(ExpressionPattern expression, Action<RefExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ParenthesizedLambdaExpressionPattern : LambdaExpressionPattern
    {
        private readonly ParameterListPattern _parameterList;
        private readonly Action<ParenthesizedLambdaExpressionSyntax> _action;

        internal ParenthesizedLambdaExpressionPattern(PatternNode body, ParameterListPattern parameterList, Action<ParenthesizedLambdaExpressionSyntax> action)
            : base(body)
        {
            _parameterList = parameterList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParenthesizedLambdaExpressionSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParenthesizedLambdaExpressionSyntax)node;

            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class InitializerExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly NodeListPattern<ExpressionPattern> _expressions;
        private readonly Action<InitializerExpressionSyntax> _action;

        internal InitializerExpressionPattern(SyntaxKind kind, NodeListPattern<ExpressionPattern> expressions, Action<InitializerExpressionSyntax> action)
        {
            _kind = kind;
            _expressions = expressions;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InitializerExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expressions != null && !_expressions.Test(typed.Expressions, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InitializerExpressionSyntax)node;

            if (_expressions != null)
                _expressions.RunCallback(typed.Expressions, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ObjectCreationExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly ArgumentListPattern _argumentList;
        private readonly InitializerExpressionPattern _initializer;
        private readonly Action<ObjectCreationExpressionSyntax> _action;

        internal ObjectCreationExpressionPattern(TypePattern type, ArgumentListPattern argumentList, InitializerExpressionPattern initializer, Action<ObjectCreationExpressionSyntax> action)
        {
            _type = type;
            _argumentList = argumentList;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ObjectCreationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ObjectCreationExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);
            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AnonymousObjectMemberDeclaratorPattern : PatternNode
    {
        private readonly NameEqualsPattern _nameEquals;
        private readonly ExpressionPattern _expression;
        private readonly Action<AnonymousObjectMemberDeclaratorSyntax> _action;

        internal AnonymousObjectMemberDeclaratorPattern(NameEqualsPattern nameEquals, ExpressionPattern expression, Action<AnonymousObjectMemberDeclaratorSyntax> action)
        {
            _nameEquals = nameEquals;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousObjectMemberDeclaratorSyntax typed))
                return false;

            if (_nameEquals != null && !_nameEquals.Test(typed.NameEquals, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AnonymousObjectMemberDeclaratorSyntax)node;

            if (_nameEquals != null)
                _nameEquals.RunCallback(typed.NameEquals, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AnonymousObjectCreationExpressionPattern : ExpressionPattern
    {
        private readonly NodeListPattern<AnonymousObjectMemberDeclaratorPattern> _initializers;
        private readonly Action<AnonymousObjectCreationExpressionSyntax> _action;

        internal AnonymousObjectCreationExpressionPattern(NodeListPattern<AnonymousObjectMemberDeclaratorPattern> initializers, Action<AnonymousObjectCreationExpressionSyntax> action)
        {
            _initializers = initializers;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousObjectCreationExpressionSyntax typed))
                return false;

            if (_initializers != null && !_initializers.Test(typed.Initializers, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AnonymousObjectCreationExpressionSyntax)node;

            if (_initializers != null)
                _initializers.RunCallback(typed.Initializers, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ArrayCreationExpressionPattern : ExpressionPattern
    {
        private readonly ArrayTypePattern _type;
        private readonly InitializerExpressionPattern _initializer;
        private readonly Action<ArrayCreationExpressionSyntax> _action;

        internal ArrayCreationExpressionPattern(ArrayTypePattern type, InitializerExpressionPattern initializer, Action<ArrayCreationExpressionSyntax> action)
        {
            _type = type;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrayCreationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrayCreationExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ImplicitArrayCreationExpressionPattern : ExpressionPattern
    {
        private readonly InitializerExpressionPattern _initializer;
        private readonly Action<ImplicitArrayCreationExpressionSyntax> _action;

        internal ImplicitArrayCreationExpressionPattern(InitializerExpressionPattern initializer, Action<ImplicitArrayCreationExpressionSyntax> action)
        {
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ImplicitArrayCreationExpressionSyntax typed))
                return false;

            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ImplicitArrayCreationExpressionSyntax)node;

            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class StackAllocArrayCreationExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly Action<StackAllocArrayCreationExpressionSyntax> _action;

        internal StackAllocArrayCreationExpressionPattern(TypePattern type, Action<StackAllocArrayCreationExpressionSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is StackAllocArrayCreationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (StackAllocArrayCreationExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class QueryClausePattern : PatternNode
    {

        internal QueryClausePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryClauseSyntax typed))
                return false;


            return true;
        }
    }

    public abstract partial class SelectOrGroupClausePattern : PatternNode
    {

        internal SelectOrGroupClausePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SelectOrGroupClauseSyntax typed))
                return false;


            return true;
        }
    }

    public partial class QueryExpressionPattern : ExpressionPattern
    {
        private readonly FromClausePattern _fromClause;
        private readonly QueryBodyPattern _body;
        private readonly Action<QueryExpressionSyntax> _action;

        internal QueryExpressionPattern(FromClausePattern fromClause, QueryBodyPattern body, Action<QueryExpressionSyntax> action)
        {
            _fromClause = fromClause;
            _body = body;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryExpressionSyntax typed))
                return false;

            if (_fromClause != null && !_fromClause.Test(typed.FromClause, semanticModel))
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QueryExpressionSyntax)node;

            if (_fromClause != null)
                _fromClause.RunCallback(typed.FromClause, semanticModel);
            if (_body != null)
                _body.RunCallback(typed.Body, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class QueryBodyPattern : PatternNode
    {
        private readonly NodeListPattern<QueryClausePattern> _clauses;
        private readonly SelectOrGroupClausePattern _selectOrGroup;
        private readonly QueryContinuationPattern _continuation;
        private readonly Action<QueryBodySyntax> _action;

        internal QueryBodyPattern(NodeListPattern<QueryClausePattern> clauses, SelectOrGroupClausePattern selectOrGroup, QueryContinuationPattern continuation, Action<QueryBodySyntax> action)
        {
            _clauses = clauses;
            _selectOrGroup = selectOrGroup;
            _continuation = continuation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryBodySyntax typed))
                return false;

            if (_clauses != null && !_clauses.Test(typed.Clauses, semanticModel))
                return false;
            if (_selectOrGroup != null && !_selectOrGroup.Test(typed.SelectOrGroup, semanticModel))
                return false;
            if (_continuation != null && !_continuation.Test(typed.Continuation, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QueryBodySyntax)node;

            if (_clauses != null)
                _clauses.RunCallback(typed.Clauses, semanticModel);
            if (_selectOrGroup != null)
                _selectOrGroup.RunCallback(typed.SelectOrGroup, semanticModel);
            if (_continuation != null)
                _continuation.RunCallback(typed.Continuation, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class FromClausePattern : QueryClausePattern
    {
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly ExpressionPattern _expression;
        private readonly Action<FromClauseSyntax> _action;

        internal FromClausePattern(TypePattern type, string identifier, ExpressionPattern expression, Action<FromClauseSyntax> action)
        {
            _type = type;
            _identifier = identifier;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FromClauseSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FromClauseSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class LetClausePattern : QueryClausePattern
    {
        private readonly string _identifier;
        private readonly ExpressionPattern _expression;
        private readonly Action<LetClauseSyntax> _action;

        internal LetClausePattern(string identifier, ExpressionPattern expression, Action<LetClauseSyntax> action)
        {
            _identifier = identifier;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LetClauseSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LetClauseSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class JoinClausePattern : QueryClausePattern
    {
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly ExpressionPattern _inExpression;
        private readonly ExpressionPattern _leftExpression;
        private readonly ExpressionPattern _rightExpression;
        private readonly JoinIntoClausePattern _into;
        private readonly Action<JoinClauseSyntax> _action;

        internal JoinClausePattern(TypePattern type, string identifier, ExpressionPattern inExpression, ExpressionPattern leftExpression, ExpressionPattern rightExpression, JoinIntoClausePattern into, Action<JoinClauseSyntax> action)
        {
            _type = type;
            _identifier = identifier;
            _inExpression = inExpression;
            _leftExpression = leftExpression;
            _rightExpression = rightExpression;
            _into = into;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is JoinClauseSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_inExpression != null && !_inExpression.Test(typed.InExpression, semanticModel))
                return false;
            if (_leftExpression != null && !_leftExpression.Test(typed.LeftExpression, semanticModel))
                return false;
            if (_rightExpression != null && !_rightExpression.Test(typed.RightExpression, semanticModel))
                return false;
            if (_into != null && !_into.Test(typed.Into, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (JoinClauseSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_inExpression != null)
                _inExpression.RunCallback(typed.InExpression, semanticModel);
            if (_leftExpression != null)
                _leftExpression.RunCallback(typed.LeftExpression, semanticModel);
            if (_rightExpression != null)
                _rightExpression.RunCallback(typed.RightExpression, semanticModel);
            if (_into != null)
                _into.RunCallback(typed.Into, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class JoinIntoClausePattern : PatternNode
    {
        private readonly string _identifier;
        private readonly Action<JoinIntoClauseSyntax> _action;

        internal JoinIntoClausePattern(string identifier, Action<JoinIntoClauseSyntax> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is JoinIntoClauseSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (JoinIntoClauseSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class WhereClausePattern : QueryClausePattern
    {
        private readonly ExpressionPattern _condition;
        private readonly Action<WhereClauseSyntax> _action;

        internal WhereClausePattern(ExpressionPattern condition, Action<WhereClauseSyntax> action)
        {
            _condition = condition;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is WhereClauseSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (WhereClauseSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class OrderByClausePattern : QueryClausePattern
    {
        private readonly NodeListPattern<OrderingPattern> _orderings;
        private readonly Action<OrderByClauseSyntax> _action;

        internal OrderByClausePattern(NodeListPattern<OrderingPattern> orderings, Action<OrderByClauseSyntax> action)
        {
            _orderings = orderings;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OrderByClauseSyntax typed))
                return false;

            if (_orderings != null && !_orderings.Test(typed.Orderings, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OrderByClauseSyntax)node;

            if (_orderings != null)
                _orderings.RunCallback(typed.Orderings, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class OrderingPattern : PatternNode
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _expression;
        private readonly Action<OrderingSyntax> _action;

        internal OrderingPattern(SyntaxKind kind, ExpressionPattern expression, Action<OrderingSyntax> action)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OrderingSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OrderingSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class SelectClausePattern : SelectOrGroupClausePattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<SelectClauseSyntax> _action;

        internal SelectClausePattern(ExpressionPattern expression, Action<SelectClauseSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SelectClauseSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SelectClauseSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class GroupClausePattern : SelectOrGroupClausePattern
    {
        private readonly ExpressionPattern _groupExpression;
        private readonly ExpressionPattern _byExpression;
        private readonly Action<GroupClauseSyntax> _action;

        internal GroupClausePattern(ExpressionPattern groupExpression, ExpressionPattern byExpression, Action<GroupClauseSyntax> action)
        {
            _groupExpression = groupExpression;
            _byExpression = byExpression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GroupClauseSyntax typed))
                return false;

            if (_groupExpression != null && !_groupExpression.Test(typed.GroupExpression, semanticModel))
                return false;
            if (_byExpression != null && !_byExpression.Test(typed.ByExpression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (GroupClauseSyntax)node;

            if (_groupExpression != null)
                _groupExpression.RunCallback(typed.GroupExpression, semanticModel);
            if (_byExpression != null)
                _byExpression.RunCallback(typed.ByExpression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class QueryContinuationPattern : PatternNode
    {
        private readonly string _identifier;
        private readonly QueryBodyPattern _body;
        private readonly Action<QueryContinuationSyntax> _action;

        internal QueryContinuationPattern(string identifier, QueryBodyPattern body, Action<QueryContinuationSyntax> action)
        {
            _identifier = identifier;
            _body = body;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryContinuationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QueryContinuationSyntax)node;

            if (_body != null)
                _body.RunCallback(typed.Body, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class OmittedArraySizeExpressionPattern : ExpressionPattern
    {
        private readonly Action<OmittedArraySizeExpressionSyntax> _action;

        internal OmittedArraySizeExpressionPattern(Action<OmittedArraySizeExpressionSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OmittedArraySizeExpressionSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OmittedArraySizeExpressionSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class InterpolatedStringExpressionPattern : ExpressionPattern
    {
        private readonly NodeListPattern<InterpolatedStringContentPattern> _contents;
        private readonly Action<InterpolatedStringExpressionSyntax> _action;

        internal InterpolatedStringExpressionPattern(NodeListPattern<InterpolatedStringContentPattern> contents, Action<InterpolatedStringExpressionSyntax> action)
        {
            _contents = contents;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolatedStringExpressionSyntax typed))
                return false;

            if (_contents != null && !_contents.Test(typed.Contents, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolatedStringExpressionSyntax)node;

            if (_contents != null)
                _contents.RunCallback(typed.Contents, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class IsPatternExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly PatternPattern _pattern;
        private readonly Action<IsPatternExpressionSyntax> _action;

        internal IsPatternExpressionPattern(ExpressionPattern expression, PatternPattern pattern, Action<IsPatternExpressionSyntax> action)
        {
            _expression = expression;
            _pattern = pattern;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IsPatternExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_pattern != null && !_pattern.Test(typed.Pattern, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (IsPatternExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_pattern != null)
                _pattern.RunCallback(typed.Pattern, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ThrowExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ThrowExpressionSyntax> _action;

        internal ThrowExpressionPattern(ExpressionPattern expression, Action<ThrowExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ThrowExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ThrowExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class WhenClausePattern : PatternNode
    {
        private readonly ExpressionPattern _condition;
        private readonly Action<WhenClauseSyntax> _action;

        internal WhenClausePattern(ExpressionPattern condition, Action<WhenClauseSyntax> action)
        {
            _condition = condition;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is WhenClauseSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (WhenClauseSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class PatternPattern : PatternNode
    {

        internal PatternPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PatternSyntax typed))
                return false;


            return true;
        }
    }

    public partial class DeclarationPatternPattern : PatternPattern
    {
        private readonly TypePattern _type;
        private readonly VariableDesignationPattern _designation;
        private readonly Action<DeclarationPatternSyntax> _action;

        internal DeclarationPatternPattern(TypePattern type, VariableDesignationPattern designation, Action<DeclarationPatternSyntax> action)
        {
            _type = type;
            _designation = designation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DeclarationPatternSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_designation != null && !_designation.Test(typed.Designation, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DeclarationPatternSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_designation != null)
                _designation.RunCallback(typed.Designation, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ConstantPatternPattern : PatternPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ConstantPatternSyntax> _action;

        internal ConstantPatternPattern(ExpressionPattern expression, Action<ConstantPatternSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstantPatternSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConstantPatternSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class InterpolatedStringContentPattern : PatternNode
    {

        internal InterpolatedStringContentPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolatedStringContentSyntax typed))
                return false;


            return true;
        }
    }

    public partial class InterpolatedStringTextPattern : InterpolatedStringContentPattern
    {
        private readonly Action<InterpolatedStringTextSyntax> _action;

        internal InterpolatedStringTextPattern(Action<InterpolatedStringTextSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolatedStringTextSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolatedStringTextSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class InterpolationPattern : InterpolatedStringContentPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly InterpolationAlignmentClausePattern _alignmentClause;
        private readonly InterpolationFormatClausePattern _formatClause;
        private readonly Action<InterpolationSyntax> _action;

        internal InterpolationPattern(ExpressionPattern expression, InterpolationAlignmentClausePattern alignmentClause, InterpolationFormatClausePattern formatClause, Action<InterpolationSyntax> action)
        {
            _expression = expression;
            _alignmentClause = alignmentClause;
            _formatClause = formatClause;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolationSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_alignmentClause != null && !_alignmentClause.Test(typed.AlignmentClause, semanticModel))
                return false;
            if (_formatClause != null && !_formatClause.Test(typed.FormatClause, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolationSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_alignmentClause != null)
                _alignmentClause.RunCallback(typed.AlignmentClause, semanticModel);
            if (_formatClause != null)
                _formatClause.RunCallback(typed.FormatClause, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class InterpolationAlignmentClausePattern : PatternNode
    {
        private readonly ExpressionPattern _value;
        private readonly Action<InterpolationAlignmentClauseSyntax> _action;

        internal InterpolationAlignmentClausePattern(ExpressionPattern value, Action<InterpolationAlignmentClauseSyntax> action)
        {
            _value = value;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolationAlignmentClauseSyntax typed))
                return false;

            if (_value != null && !_value.Test(typed.Value, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolationAlignmentClauseSyntax)node;

            if (_value != null)
                _value.RunCallback(typed.Value, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class InterpolationFormatClausePattern : PatternNode
    {
        private readonly Action<InterpolationFormatClauseSyntax> _action;

        internal InterpolationFormatClausePattern(Action<InterpolationFormatClauseSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolationFormatClauseSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolationFormatClauseSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class GlobalStatementPattern : MemberDeclarationPattern
    {
        private readonly StatementPattern _statement;
        private readonly Action<GlobalStatementSyntax> _action;

        internal GlobalStatementPattern(StatementPattern statement, Action<GlobalStatementSyntax> action)
        {
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GlobalStatementSyntax typed))
                return false;

            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (GlobalStatementSyntax)node;

            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class StatementPattern : PatternNode
    {

        internal StatementPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is StatementSyntax typed))
                return false;


            return true;
        }
    }

    public partial class BlockPattern : StatementPattern
    {
        private readonly NodeListPattern<StatementPattern> _statements;
        private readonly Action<BlockSyntax> _action;

        internal BlockPattern(NodeListPattern<StatementPattern> statements, Action<BlockSyntax> action)
        {
            _statements = statements;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BlockSyntax typed))
                return false;

            if (_statements != null && !_statements.Test(typed.Statements, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BlockSyntax)node;

            if (_statements != null)
                _statements.RunCallback(typed.Statements, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class LocalFunctionStatementPattern : StatementPattern
    {
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern _returnType;
        private readonly string _identifier;
        private readonly TypeParameterListPattern _typeParameterList;
        private readonly ParameterListPattern _parameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern> _constraintClauses;
        private readonly BlockPattern _body;
        private readonly ArrowExpressionClausePattern _expressionBody;
        private readonly Action<LocalFunctionStatementSyntax> _action;

        internal LocalFunctionStatementPattern(TokenListPattern modifiers, TypePattern returnType, string identifier, TypeParameterListPattern typeParameterList, ParameterListPattern parameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, BlockPattern body, ArrowExpressionClausePattern expressionBody, Action<LocalFunctionStatementSyntax> action)
        {
            _modifiers = modifiers;
            _returnType = returnType;
            _identifier = identifier;
            _typeParameterList = typeParameterList;
            _parameterList = parameterList;
            _constraintClauses = constraintClauses;
            _body = body;
            _expressionBody = expressionBody;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LocalFunctionStatementSyntax typed))
                return false;

            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;
            if (_expressionBody != null && !_expressionBody.Test(typed.ExpressionBody, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LocalFunctionStatementSyntax)node;

            if (_returnType != null)
                _returnType.RunCallback(typed.ReturnType, semanticModel);
            if (_typeParameterList != null)
                _typeParameterList.RunCallback(typed.TypeParameterList, semanticModel);
            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);
            if (_constraintClauses != null)
                _constraintClauses.RunCallback(typed.ConstraintClauses, semanticModel);
            if (_body != null)
                _body.RunCallback(typed.Body, semanticModel);
            if (_expressionBody != null)
                _expressionBody.RunCallback(typed.ExpressionBody, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class LocalDeclarationStatementPattern : StatementPattern
    {
        private readonly TokenListPattern _modifiers;
        private readonly VariableDeclarationPattern _declaration;
        private readonly Action<LocalDeclarationStatementSyntax> _action;

        internal LocalDeclarationStatementPattern(TokenListPattern modifiers, VariableDeclarationPattern declaration, Action<LocalDeclarationStatementSyntax> action)
        {
            _modifiers = modifiers;
            _declaration = declaration;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LocalDeclarationStatementSyntax typed))
                return false;

            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LocalDeclarationStatementSyntax)node;

            if (_declaration != null)
                _declaration.RunCallback(typed.Declaration, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class VariableDeclarationPattern : PatternNode
    {
        private readonly TypePattern _type;
        private readonly NodeListPattern<VariableDeclaratorPattern> _variables;
        private readonly Action<VariableDeclarationSyntax> _action;

        internal VariableDeclarationPattern(TypePattern type, NodeListPattern<VariableDeclaratorPattern> variables, Action<VariableDeclarationSyntax> action)
        {
            _type = type;
            _variables = variables;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is VariableDeclarationSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_variables != null && !_variables.Test(typed.Variables, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (VariableDeclarationSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_variables != null)
                _variables.RunCallback(typed.Variables, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class VariableDeclaratorPattern : PatternNode
    {
        private readonly string _identifier;
        private readonly BracketedArgumentListPattern _argumentList;
        private readonly EqualsValueClausePattern _initializer;
        private readonly Action<VariableDeclaratorSyntax> _action;

        internal VariableDeclaratorPattern(string identifier, BracketedArgumentListPattern argumentList, EqualsValueClausePattern initializer, Action<VariableDeclaratorSyntax> action)
        {
            _identifier = identifier;
            _argumentList = argumentList;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is VariableDeclaratorSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (VariableDeclaratorSyntax)node;

            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);
            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class EqualsValueClausePattern : PatternNode
    {
        private readonly ExpressionPattern _value;
        private readonly Action<EqualsValueClauseSyntax> _action;

        internal EqualsValueClausePattern(ExpressionPattern value, Action<EqualsValueClauseSyntax> action)
        {
            _value = value;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EqualsValueClauseSyntax typed))
                return false;

            if (_value != null && !_value.Test(typed.Value, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (EqualsValueClauseSyntax)node;

            if (_value != null)
                _value.RunCallback(typed.Value, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class VariableDesignationPattern : PatternNode
    {

        internal VariableDesignationPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is VariableDesignationSyntax typed))
                return false;


            return true;
        }
    }

    public partial class SingleVariableDesignationPattern : VariableDesignationPattern
    {
        private readonly string _identifier;
        private readonly Action<SingleVariableDesignationSyntax> _action;

        internal SingleVariableDesignationPattern(string identifier, Action<SingleVariableDesignationSyntax> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SingleVariableDesignationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SingleVariableDesignationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class DiscardDesignationPattern : VariableDesignationPattern
    {
        private readonly Action<DiscardDesignationSyntax> _action;

        internal DiscardDesignationPattern(Action<DiscardDesignationSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DiscardDesignationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DiscardDesignationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class ParenthesizedVariableDesignationPattern : VariableDesignationPattern
    {
        private readonly NodeListPattern<VariableDesignationPattern> _variables;
        private readonly Action<ParenthesizedVariableDesignationSyntax> _action;

        internal ParenthesizedVariableDesignationPattern(NodeListPattern<VariableDesignationPattern> variables, Action<ParenthesizedVariableDesignationSyntax> action)
        {
            _variables = variables;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParenthesizedVariableDesignationSyntax typed))
                return false;

            if (_variables != null && !_variables.Test(typed.Variables, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParenthesizedVariableDesignationSyntax)node;

            if (_variables != null)
                _variables.RunCallback(typed.Variables, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ExpressionStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ExpressionStatementSyntax> _action;

        internal ExpressionStatementPattern(ExpressionPattern expression, Action<ExpressionStatementSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExpressionStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ExpressionStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class EmptyStatementPattern : StatementPattern
    {
        private readonly Action<EmptyStatementSyntax> _action;

        internal EmptyStatementPattern(Action<EmptyStatementSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EmptyStatementSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (EmptyStatementSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class LabeledStatementPattern : StatementPattern
    {
        private readonly string _identifier;
        private readonly StatementPattern _statement;
        private readonly Action<LabeledStatementSyntax> _action;

        internal LabeledStatementPattern(string identifier, StatementPattern statement, Action<LabeledStatementSyntax> action)
        {
            _identifier = identifier;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LabeledStatementSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LabeledStatementSyntax)node;

            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class GotoStatementPattern : StatementPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _expression;
        private readonly Action<GotoStatementSyntax> _action;

        internal GotoStatementPattern(SyntaxKind kind, ExpressionPattern expression, Action<GotoStatementSyntax> action)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GotoStatementSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (GotoStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class BreakStatementPattern : StatementPattern
    {
        private readonly Action<BreakStatementSyntax> _action;

        internal BreakStatementPattern(Action<BreakStatementSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BreakStatementSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BreakStatementSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class ContinueStatementPattern : StatementPattern
    {
        private readonly Action<ContinueStatementSyntax> _action;

        internal ContinueStatementPattern(Action<ContinueStatementSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ContinueStatementSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ContinueStatementSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class ReturnStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ReturnStatementSyntax> _action;

        internal ReturnStatementPattern(ExpressionPattern expression, Action<ReturnStatementSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ReturnStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ReturnStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ThrowStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ThrowStatementSyntax> _action;

        internal ThrowStatementPattern(ExpressionPattern expression, Action<ThrowStatementSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ThrowStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ThrowStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class YieldStatementPattern : StatementPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _expression;
        private readonly Action<YieldStatementSyntax> _action;

        internal YieldStatementPattern(SyntaxKind kind, ExpressionPattern expression, Action<YieldStatementSyntax> action)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is YieldStatementSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (YieldStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class WhileStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _condition;
        private readonly StatementPattern _statement;
        private readonly Action<WhileStatementSyntax> _action;

        internal WhileStatementPattern(ExpressionPattern condition, StatementPattern statement, Action<WhileStatementSyntax> action)
        {
            _condition = condition;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is WhileStatementSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (WhileStatementSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class DoStatementPattern : StatementPattern
    {
        private readonly StatementPattern _statement;
        private readonly ExpressionPattern _condition;
        private readonly Action<DoStatementSyntax> _action;

        internal DoStatementPattern(StatementPattern statement, ExpressionPattern condition, Action<DoStatementSyntax> action)
        {
            _statement = statement;
            _condition = condition;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DoStatementSyntax typed))
                return false;

            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;
            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DoStatementSyntax)node;

            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);
            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ForStatementPattern : StatementPattern
    {
        private readonly VariableDeclarationPattern _declaration;
        private readonly NodeListPattern<ExpressionPattern> _initializers;
        private readonly ExpressionPattern _condition;
        private readonly NodeListPattern<ExpressionPattern> _incrementors;
        private readonly StatementPattern _statement;
        private readonly Action<ForStatementSyntax> _action;

        internal ForStatementPattern(VariableDeclarationPattern declaration, NodeListPattern<ExpressionPattern> initializers, ExpressionPattern condition, NodeListPattern<ExpressionPattern> incrementors, StatementPattern statement, Action<ForStatementSyntax> action)
        {
            _declaration = declaration;
            _initializers = initializers;
            _condition = condition;
            _incrementors = incrementors;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ForStatementSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;
            if (_initializers != null && !_initializers.Test(typed.Initializers, semanticModel))
                return false;
            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_incrementors != null && !_incrementors.Test(typed.Incrementors, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ForStatementSyntax)node;

            if (_declaration != null)
                _declaration.RunCallback(typed.Declaration, semanticModel);
            if (_initializers != null)
                _initializers.RunCallback(typed.Initializers, semanticModel);
            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);
            if (_incrementors != null)
                _incrementors.RunCallback(typed.Incrementors, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class CommonForEachStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly StatementPattern _statement;

        internal CommonForEachStatementPattern(ExpressionPattern expression, StatementPattern statement)
        {
            _expression = expression;
            _statement = statement;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CommonForEachStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CommonForEachStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);
        }
    }

    public partial class ForEachStatementPattern : CommonForEachStatementPattern
    {
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly Action<ForEachStatementSyntax> _action;

        internal ForEachStatementPattern(ExpressionPattern expression, StatementPattern statement, TypePattern type, string identifier, Action<ForEachStatementSyntax> action)
            : base(expression, statement)
        {
            _type = type;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ForEachStatementSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ForEachStatementSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ForEachVariableStatementPattern : CommonForEachStatementPattern
    {
        private readonly ExpressionPattern _variable;
        private readonly Action<ForEachVariableStatementSyntax> _action;

        internal ForEachVariableStatementPattern(ExpressionPattern expression, StatementPattern statement, ExpressionPattern variable, Action<ForEachVariableStatementSyntax> action)
            : base(expression, statement)
        {
            _variable = variable;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ForEachVariableStatementSyntax typed))
                return false;

            if (_variable != null && !_variable.Test(typed.Variable, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ForEachVariableStatementSyntax)node;

            if (_variable != null)
                _variable.RunCallback(typed.Variable, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class UsingStatementPattern : StatementPattern
    {
        private readonly VariableDeclarationPattern _declaration;
        private readonly ExpressionPattern _expression;
        private readonly StatementPattern _statement;
        private readonly Action<UsingStatementSyntax> _action;

        internal UsingStatementPattern(VariableDeclarationPattern declaration, ExpressionPattern expression, StatementPattern statement, Action<UsingStatementSyntax> action)
        {
            _declaration = declaration;
            _expression = expression;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is UsingStatementSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (UsingStatementSyntax)node;

            if (_declaration != null)
                _declaration.RunCallback(typed.Declaration, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class FixedStatementPattern : StatementPattern
    {
        private readonly VariableDeclarationPattern _declaration;
        private readonly StatementPattern _statement;
        private readonly Action<FixedStatementSyntax> _action;

        internal FixedStatementPattern(VariableDeclarationPattern declaration, StatementPattern statement, Action<FixedStatementSyntax> action)
        {
            _declaration = declaration;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FixedStatementSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FixedStatementSyntax)node;

            if (_declaration != null)
                _declaration.RunCallback(typed.Declaration, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class CheckedStatementPattern : StatementPattern
    {
        private readonly SyntaxKind _kind;
        private readonly BlockPattern _block;
        private readonly Action<CheckedStatementSyntax> _action;

        internal CheckedStatementPattern(SyntaxKind kind, BlockPattern block, Action<CheckedStatementSyntax> action)
        {
            _kind = kind;
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CheckedStatementSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CheckedStatementSyntax)node;

            if (_block != null)
                _block.RunCallback(typed.Block, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class UnsafeStatementPattern : StatementPattern
    {
        private readonly BlockPattern _block;
        private readonly Action<UnsafeStatementSyntax> _action;

        internal UnsafeStatementPattern(BlockPattern block, Action<UnsafeStatementSyntax> action)
        {
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is UnsafeStatementSyntax typed))
                return false;

            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (UnsafeStatementSyntax)node;

            if (_block != null)
                _block.RunCallback(typed.Block, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class LockStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly StatementPattern _statement;
        private readonly Action<LockStatementSyntax> _action;

        internal LockStatementPattern(ExpressionPattern expression, StatementPattern statement, Action<LockStatementSyntax> action)
        {
            _expression = expression;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LockStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LockStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class IfStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _condition;
        private readonly StatementPattern _statement;
        private readonly ElseClausePattern _else;
        private readonly Action<IfStatementSyntax> _action;

        internal IfStatementPattern(ExpressionPattern condition, StatementPattern statement, ElseClausePattern @else, Action<IfStatementSyntax> action)
        {
            _condition = condition;
            _statement = statement;
            _else = @else;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IfStatementSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;
            if (_else != null && !_else.Test(typed.Else, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (IfStatementSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);
            if (_else != null)
                _else.RunCallback(typed.Else, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ElseClausePattern : PatternNode
    {
        private readonly StatementPattern _statement;
        private readonly Action<ElseClauseSyntax> _action;

        internal ElseClausePattern(StatementPattern statement, Action<ElseClauseSyntax> action)
        {
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ElseClauseSyntax typed))
                return false;

            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ElseClauseSyntax)node;

            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class SwitchStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly NodeListPattern<SwitchSectionPattern> _sections;
        private readonly Action<SwitchStatementSyntax> _action;

        internal SwitchStatementPattern(ExpressionPattern expression, NodeListPattern<SwitchSectionPattern> sections, Action<SwitchStatementSyntax> action)
        {
            _expression = expression;
            _sections = sections;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_sections != null && !_sections.Test(typed.Sections, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SwitchStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_sections != null)
                _sections.RunCallback(typed.Sections, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class SwitchSectionPattern : PatternNode
    {
        private readonly NodeListPattern<SwitchLabelPattern> _labels;
        private readonly NodeListPattern<StatementPattern> _statements;
        private readonly Action<SwitchSectionSyntax> _action;

        internal SwitchSectionPattern(NodeListPattern<SwitchLabelPattern> labels, NodeListPattern<StatementPattern> statements, Action<SwitchSectionSyntax> action)
        {
            _labels = labels;
            _statements = statements;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchSectionSyntax typed))
                return false;

            if (_labels != null && !_labels.Test(typed.Labels, semanticModel))
                return false;
            if (_statements != null && !_statements.Test(typed.Statements, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SwitchSectionSyntax)node;

            if (_labels != null)
                _labels.RunCallback(typed.Labels, semanticModel);
            if (_statements != null)
                _statements.RunCallback(typed.Statements, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class SwitchLabelPattern : PatternNode
    {

        internal SwitchLabelPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchLabelSyntax typed))
                return false;


            return true;
        }
    }

    public partial class CasePatternSwitchLabelPattern : SwitchLabelPattern
    {
        private readonly PatternPattern _pattern;
        private readonly WhenClausePattern _whenClause;
        private readonly Action<CasePatternSwitchLabelSyntax> _action;

        internal CasePatternSwitchLabelPattern(PatternPattern pattern, WhenClausePattern whenClause, Action<CasePatternSwitchLabelSyntax> action)
        {
            _pattern = pattern;
            _whenClause = whenClause;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CasePatternSwitchLabelSyntax typed))
                return false;

            if (_pattern != null && !_pattern.Test(typed.Pattern, semanticModel))
                return false;
            if (_whenClause != null && !_whenClause.Test(typed.WhenClause, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CasePatternSwitchLabelSyntax)node;

            if (_pattern != null)
                _pattern.RunCallback(typed.Pattern, semanticModel);
            if (_whenClause != null)
                _whenClause.RunCallback(typed.WhenClause, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class CaseSwitchLabelPattern : SwitchLabelPattern
    {
        private readonly ExpressionPattern _value;
        private readonly Action<CaseSwitchLabelSyntax> _action;

        internal CaseSwitchLabelPattern(ExpressionPattern value, Action<CaseSwitchLabelSyntax> action)
        {
            _value = value;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CaseSwitchLabelSyntax typed))
                return false;

            if (_value != null && !_value.Test(typed.Value, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CaseSwitchLabelSyntax)node;

            if (_value != null)
                _value.RunCallback(typed.Value, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class DefaultSwitchLabelPattern : SwitchLabelPattern
    {
        private readonly Action<DefaultSwitchLabelSyntax> _action;

        internal DefaultSwitchLabelPattern(Action<DefaultSwitchLabelSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DefaultSwitchLabelSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DefaultSwitchLabelSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class TryStatementPattern : StatementPattern
    {
        private readonly BlockPattern _block;
        private readonly NodeListPattern<CatchClausePattern> _catches;
        private readonly FinallyClausePattern _finally;
        private readonly Action<TryStatementSyntax> _action;

        internal TryStatementPattern(BlockPattern block, NodeListPattern<CatchClausePattern> catches, FinallyClausePattern @finally, Action<TryStatementSyntax> action)
        {
            _block = block;
            _catches = catches;
            _finally = @finally;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TryStatementSyntax typed))
                return false;

            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;
            if (_catches != null && !_catches.Test(typed.Catches, semanticModel))
                return false;
            if (_finally != null && !_finally.Test(typed.Finally, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TryStatementSyntax)node;

            if (_block != null)
                _block.RunCallback(typed.Block, semanticModel);
            if (_catches != null)
                _catches.RunCallback(typed.Catches, semanticModel);
            if (_finally != null)
                _finally.RunCallback(typed.Finally, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class CatchClausePattern : PatternNode
    {
        private readonly CatchDeclarationPattern _declaration;
        private readonly CatchFilterClausePattern _filter;
        private readonly BlockPattern _block;
        private readonly Action<CatchClauseSyntax> _action;

        internal CatchClausePattern(CatchDeclarationPattern declaration, CatchFilterClausePattern filter, BlockPattern block, Action<CatchClauseSyntax> action)
        {
            _declaration = declaration;
            _filter = filter;
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CatchClauseSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;
            if (_filter != null && !_filter.Test(typed.Filter, semanticModel))
                return false;
            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CatchClauseSyntax)node;

            if (_declaration != null)
                _declaration.RunCallback(typed.Declaration, semanticModel);
            if (_filter != null)
                _filter.RunCallback(typed.Filter, semanticModel);
            if (_block != null)
                _block.RunCallback(typed.Block, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class CatchDeclarationPattern : PatternNode
    {
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly Action<CatchDeclarationSyntax> _action;

        internal CatchDeclarationPattern(TypePattern type, string identifier, Action<CatchDeclarationSyntax> action)
        {
            _type = type;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CatchDeclarationSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CatchDeclarationSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class CatchFilterClausePattern : PatternNode
    {
        private readonly ExpressionPattern _filterExpression;
        private readonly Action<CatchFilterClauseSyntax> _action;

        internal CatchFilterClausePattern(ExpressionPattern filterExpression, Action<CatchFilterClauseSyntax> action)
        {
            _filterExpression = filterExpression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CatchFilterClauseSyntax typed))
                return false;

            if (_filterExpression != null && !_filterExpression.Test(typed.FilterExpression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CatchFilterClauseSyntax)node;

            if (_filterExpression != null)
                _filterExpression.RunCallback(typed.FilterExpression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class FinallyClausePattern : PatternNode
    {
        private readonly BlockPattern _block;
        private readonly Action<FinallyClauseSyntax> _action;

        internal FinallyClausePattern(BlockPattern block, Action<FinallyClauseSyntax> action)
        {
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FinallyClauseSyntax typed))
                return false;

            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FinallyClauseSyntax)node;

            if (_block != null)
                _block.RunCallback(typed.Block, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class CompilationUnitPattern : PatternNode
    {
        private readonly NodeListPattern<ExternAliasDirectivePattern> _externs;
        private readonly NodeListPattern<UsingDirectivePattern> _usings;
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly NodeListPattern<MemberDeclarationPattern> _members;
        private readonly Action<CompilationUnitSyntax> _action;

        internal CompilationUnitPattern(NodeListPattern<ExternAliasDirectivePattern> externs, NodeListPattern<UsingDirectivePattern> usings, NodeListPattern<AttributeListPattern> attributeLists, NodeListPattern<MemberDeclarationPattern> members, Action<CompilationUnitSyntax> action)
        {
            _externs = externs;
            _usings = usings;
            _attributeLists = attributeLists;
            _members = members;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CompilationUnitSyntax typed))
                return false;

            if (_externs != null && !_externs.Test(typed.Externs, semanticModel))
                return false;
            if (_usings != null && !_usings.Test(typed.Usings, semanticModel))
                return false;
            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CompilationUnitSyntax)node;

            if (_externs != null)
                _externs.RunCallback(typed.Externs, semanticModel);
            if (_usings != null)
                _usings.RunCallback(typed.Usings, semanticModel);
            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_members != null)
                _members.RunCallback(typed.Members, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ExternAliasDirectivePattern : PatternNode
    {
        private readonly string _identifier;
        private readonly Action<ExternAliasDirectiveSyntax> _action;

        internal ExternAliasDirectivePattern(string identifier, Action<ExternAliasDirectiveSyntax> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExternAliasDirectiveSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ExternAliasDirectiveSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class UsingDirectivePattern : PatternNode
    {
        private readonly NameEqualsPattern _alias;
        private readonly NamePattern _name;
        private readonly Action<UsingDirectiveSyntax> _action;

        internal UsingDirectivePattern(NameEqualsPattern alias, NamePattern name, Action<UsingDirectiveSyntax> action)
        {
            _alias = alias;
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is UsingDirectiveSyntax typed))
                return false;

            if (_alias != null && !_alias.Test(typed.Alias, semanticModel))
                return false;
            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (UsingDirectiveSyntax)node;

            if (_alias != null)
                _alias.RunCallback(typed.Alias, semanticModel);
            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class MemberDeclarationPattern : PatternNode
    {

        internal MemberDeclarationPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MemberDeclarationSyntax typed))
                return false;


            return true;
        }
    }

    public partial class NamespaceDeclarationPattern : MemberDeclarationPattern
    {
        private readonly NamePattern _name;
        private readonly NodeListPattern<ExternAliasDirectivePattern> _externs;
        private readonly NodeListPattern<UsingDirectivePattern> _usings;
        private readonly NodeListPattern<MemberDeclarationPattern> _members;
        private readonly Action<NamespaceDeclarationSyntax> _action;

        internal NamespaceDeclarationPattern(NamePattern name, NodeListPattern<ExternAliasDirectivePattern> externs, NodeListPattern<UsingDirectivePattern> usings, NodeListPattern<MemberDeclarationPattern> members, Action<NamespaceDeclarationSyntax> action)
        {
            _name = name;
            _externs = externs;
            _usings = usings;
            _members = members;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NamespaceDeclarationSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;
            if (_externs != null && !_externs.Test(typed.Externs, semanticModel))
                return false;
            if (_usings != null && !_usings.Test(typed.Usings, semanticModel))
                return false;
            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NamespaceDeclarationSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);
            if (_externs != null)
                _externs.RunCallback(typed.Externs, semanticModel);
            if (_usings != null)
                _usings.RunCallback(typed.Usings, semanticModel);
            if (_members != null)
                _members.RunCallback(typed.Members, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AttributeListPattern : PatternNode
    {
        private readonly AttributeTargetSpecifierPattern _target;
        private readonly NodeListPattern<AttributePattern> _attributes;
        private readonly Action<AttributeListSyntax> _action;

        internal AttributeListPattern(AttributeTargetSpecifierPattern target, NodeListPattern<AttributePattern> attributes, Action<AttributeListSyntax> action)
        {
            _target = target;
            _attributes = attributes;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeListSyntax typed))
                return false;

            if (_target != null && !_target.Test(typed.Target, semanticModel))
                return false;
            if (_attributes != null && !_attributes.Test(typed.Attributes, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeListSyntax)node;

            if (_target != null)
                _target.RunCallback(typed.Target, semanticModel);
            if (_attributes != null)
                _attributes.RunCallback(typed.Attributes, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AttributeTargetSpecifierPattern : PatternNode
    {
        private readonly string _identifier;
        private readonly Action<AttributeTargetSpecifierSyntax> _action;

        internal AttributeTargetSpecifierPattern(string identifier, Action<AttributeTargetSpecifierSyntax> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeTargetSpecifierSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeTargetSpecifierSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class AttributePattern : PatternNode
    {
        private readonly NamePattern _name;
        private readonly AttributeArgumentListPattern _argumentList;
        private readonly Action<AttributeSyntax> _action;

        internal AttributePattern(NamePattern name, AttributeArgumentListPattern argumentList, Action<AttributeSyntax> action)
        {
            _name = name;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);
            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AttributeArgumentListPattern : PatternNode
    {
        private readonly NodeListPattern<AttributeArgumentPattern> _arguments;
        private readonly Action<AttributeArgumentListSyntax> _action;

        internal AttributeArgumentListPattern(NodeListPattern<AttributeArgumentPattern> arguments, Action<AttributeArgumentListSyntax> action)
        {
            _arguments = arguments;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeArgumentListSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeArgumentListSyntax)node;

            if (_arguments != null)
                _arguments.RunCallback(typed.Arguments, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AttributeArgumentPattern : PatternNode
    {
        private readonly NameEqualsPattern _nameEquals;
        private readonly NameColonPattern _nameColon;
        private readonly ExpressionPattern _expression;
        private readonly Action<AttributeArgumentSyntax> _action;

        internal AttributeArgumentPattern(NameEqualsPattern nameEquals, NameColonPattern nameColon, ExpressionPattern expression, Action<AttributeArgumentSyntax> action)
        {
            _nameEquals = nameEquals;
            _nameColon = nameColon;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeArgumentSyntax typed))
                return false;

            if (_nameEquals != null && !_nameEquals.Test(typed.NameEquals, semanticModel))
                return false;
            if (_nameColon != null && !_nameColon.Test(typed.NameColon, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeArgumentSyntax)node;

            if (_nameEquals != null)
                _nameEquals.RunCallback(typed.NameEquals, semanticModel);
            if (_nameColon != null)
                _nameColon.RunCallback(typed.NameColon, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class NameEqualsPattern : PatternNode
    {
        private readonly IdentifierNamePattern _name;
        private readonly Action<NameEqualsSyntax> _action;

        internal NameEqualsPattern(IdentifierNamePattern name, Action<NameEqualsSyntax> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NameEqualsSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NameEqualsSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class TypeParameterListPattern : PatternNode
    {
        private readonly NodeListPattern<TypeParameterPattern> _parameters;
        private readonly Action<TypeParameterListSyntax> _action;

        internal TypeParameterListPattern(NodeListPattern<TypeParameterPattern> parameters, Action<TypeParameterListSyntax> action)
        {
            _parameters = parameters;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterListSyntax typed))
                return false;

            if (_parameters != null && !_parameters.Test(typed.Parameters, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeParameterListSyntax)node;

            if (_parameters != null)
                _parameters.RunCallback(typed.Parameters, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class TypeParameterPattern : PatternNode
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly string _identifier;
        private readonly Action<TypeParameterSyntax> _action;

        internal TypeParameterPattern(NodeListPattern<AttributeListPattern> attributeLists, string identifier, Action<TypeParameterSyntax> action)
        {
            _attributeLists = attributeLists;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeParameterSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class BaseTypeDeclarationPattern : MemberDeclarationPattern
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly string _identifier;
        private readonly BaseListPattern _baseList;

        internal BaseTypeDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _identifier = identifier;
            _baseList = baseList;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseTypeDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_baseList != null && !_baseList.Test(typed.BaseList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseTypeDeclarationSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_baseList != null)
                _baseList.RunCallback(typed.BaseList, semanticModel);
        }
    }

    public abstract partial class TypeDeclarationPattern : BaseTypeDeclarationPattern
    {
        private readonly TypeParameterListPattern _typeParameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern> _constraintClauses;
        private readonly NodeListPattern<MemberDeclarationPattern> _members;

        internal TypeDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, NodeListPattern<MemberDeclarationPattern> members)
            : base(attributeLists, modifiers, identifier, baseList)
        {
            _typeParameterList = typeParameterList;
            _constraintClauses = constraintClauses;
            _members = members;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeDeclarationSyntax typed))
                return false;

            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;
            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (TypeDeclarationSyntax)node;

            if (_typeParameterList != null)
                _typeParameterList.RunCallback(typed.TypeParameterList, semanticModel);
            if (_constraintClauses != null)
                _constraintClauses.RunCallback(typed.ConstraintClauses, semanticModel);
            if (_members != null)
                _members.RunCallback(typed.Members, semanticModel);
        }
    }

    public partial class ClassDeclarationPattern : TypeDeclarationPattern
    {
        private readonly Action<ClassDeclarationSyntax> _action;

        internal ClassDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, NodeListPattern<MemberDeclarationPattern> members, Action<ClassDeclarationSyntax> action)
            : base(attributeLists, modifiers, identifier, baseList, typeParameterList, constraintClauses, members)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ClassDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ClassDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class StructDeclarationPattern : TypeDeclarationPattern
    {
        private readonly Action<StructDeclarationSyntax> _action;

        internal StructDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, NodeListPattern<MemberDeclarationPattern> members, Action<StructDeclarationSyntax> action)
            : base(attributeLists, modifiers, identifier, baseList, typeParameterList, constraintClauses, members)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is StructDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (StructDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class InterfaceDeclarationPattern : TypeDeclarationPattern
    {
        private readonly Action<InterfaceDeclarationSyntax> _action;

        internal InterfaceDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, NodeListPattern<MemberDeclarationPattern> members, Action<InterfaceDeclarationSyntax> action)
            : base(attributeLists, modifiers, identifier, baseList, typeParameterList, constraintClauses, members)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterfaceDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (InterfaceDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class EnumDeclarationPattern : BaseTypeDeclarationPattern
    {
        private readonly NodeListPattern<EnumMemberDeclarationPattern> _members;
        private readonly Action<EnumDeclarationSyntax> _action;

        internal EnumDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, NodeListPattern<EnumMemberDeclarationPattern> members, Action<EnumDeclarationSyntax> action)
            : base(attributeLists, modifiers, identifier, baseList)
        {
            _members = members;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EnumDeclarationSyntax typed))
                return false;

            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (EnumDeclarationSyntax)node;

            if (_members != null)
                _members.RunCallback(typed.Members, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class DelegateDeclarationPattern : MemberDeclarationPattern
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern _returnType;
        private readonly string _identifier;
        private readonly TypeParameterListPattern _typeParameterList;
        private readonly ParameterListPattern _parameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern> _constraintClauses;
        private readonly Action<DelegateDeclarationSyntax> _action;

        internal DelegateDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern returnType, string identifier, TypeParameterListPattern typeParameterList, ParameterListPattern parameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, Action<DelegateDeclarationSyntax> action)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _returnType = returnType;
            _identifier = identifier;
            _typeParameterList = typeParameterList;
            _parameterList = parameterList;
            _constraintClauses = constraintClauses;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DelegateDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DelegateDeclarationSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_returnType != null)
                _returnType.RunCallback(typed.ReturnType, semanticModel);
            if (_typeParameterList != null)
                _typeParameterList.RunCallback(typed.TypeParameterList, semanticModel);
            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);
            if (_constraintClauses != null)
                _constraintClauses.RunCallback(typed.ConstraintClauses, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class EnumMemberDeclarationPattern : MemberDeclarationPattern
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly string _identifier;
        private readonly EqualsValueClausePattern _equalsValue;
        private readonly Action<EnumMemberDeclarationSyntax> _action;

        internal EnumMemberDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, string identifier, EqualsValueClausePattern equalsValue, Action<EnumMemberDeclarationSyntax> action)
        {
            _attributeLists = attributeLists;
            _identifier = identifier;
            _equalsValue = equalsValue;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EnumMemberDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_equalsValue != null && !_equalsValue.Test(typed.EqualsValue, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (EnumMemberDeclarationSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_equalsValue != null)
                _equalsValue.RunCallback(typed.EqualsValue, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class BaseListPattern : PatternNode
    {
        private readonly NodeListPattern<BaseTypePattern> _types;
        private readonly Action<BaseListSyntax> _action;

        internal BaseListPattern(NodeListPattern<BaseTypePattern> types, Action<BaseListSyntax> action)
        {
            _types = types;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseListSyntax typed))
                return false;

            if (_types != null && !_types.Test(typed.Types, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseListSyntax)node;

            if (_types != null)
                _types.RunCallback(typed.Types, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class BaseTypePattern : PatternNode
    {
        private readonly TypePattern _type;

        internal BaseTypePattern(TypePattern type)
        {
            _type = type;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseTypeSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseTypeSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
        }
    }

    public partial class SimpleBaseTypePattern : BaseTypePattern
    {
        private readonly Action<SimpleBaseTypeSyntax> _action;

        internal SimpleBaseTypePattern(TypePattern type, Action<SimpleBaseTypeSyntax> action)
            : base(type)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SimpleBaseTypeSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (SimpleBaseTypeSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class TypeParameterConstraintClausePattern : PatternNode
    {
        private readonly IdentifierNamePattern _name;
        private readonly NodeListPattern<TypeParameterConstraintPattern> _constraints;
        private readonly Action<TypeParameterConstraintClauseSyntax> _action;

        internal TypeParameterConstraintClausePattern(IdentifierNamePattern name, NodeListPattern<TypeParameterConstraintPattern> constraints, Action<TypeParameterConstraintClauseSyntax> action)
        {
            _name = name;
            _constraints = constraints;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterConstraintClauseSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;
            if (_constraints != null && !_constraints.Test(typed.Constraints, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeParameterConstraintClauseSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);
            if (_constraints != null)
                _constraints.RunCallback(typed.Constraints, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class TypeParameterConstraintPattern : PatternNode
    {

        internal TypeParameterConstraintPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterConstraintSyntax typed))
                return false;


            return true;
        }
    }

    public partial class ConstructorConstraintPattern : TypeParameterConstraintPattern
    {
        private readonly Action<ConstructorConstraintSyntax> _action;

        internal ConstructorConstraintPattern(Action<ConstructorConstraintSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstructorConstraintSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConstructorConstraintSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class ClassOrStructConstraintPattern : TypeParameterConstraintPattern
    {
        private readonly SyntaxKind _kind;
        private readonly Action<ClassOrStructConstraintSyntax> _action;

        internal ClassOrStructConstraintPattern(SyntaxKind kind, Action<ClassOrStructConstraintSyntax> action)
        {
            _kind = kind;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ClassOrStructConstraintSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ClassOrStructConstraintSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class TypeConstraintPattern : TypeParameterConstraintPattern
    {
        private readonly TypePattern _type;
        private readonly Action<TypeConstraintSyntax> _action;

        internal TypeConstraintPattern(TypePattern type, Action<TypeConstraintSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeConstraintSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeConstraintSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class BaseFieldDeclarationPattern : MemberDeclarationPattern
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly VariableDeclarationPattern _declaration;

        internal BaseFieldDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, VariableDeclarationPattern declaration)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _declaration = declaration;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseFieldDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseFieldDeclarationSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_declaration != null)
                _declaration.RunCallback(typed.Declaration, semanticModel);
        }
    }

    public partial class FieldDeclarationPattern : BaseFieldDeclarationPattern
    {
        private readonly Action<FieldDeclarationSyntax> _action;

        internal FieldDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, VariableDeclarationPattern declaration, Action<FieldDeclarationSyntax> action)
            : base(attributeLists, modifiers, declaration)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FieldDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (FieldDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class EventFieldDeclarationPattern : BaseFieldDeclarationPattern
    {
        private readonly Action<EventFieldDeclarationSyntax> _action;

        internal EventFieldDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, VariableDeclarationPattern declaration, Action<EventFieldDeclarationSyntax> action)
            : base(attributeLists, modifiers, declaration)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EventFieldDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (EventFieldDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class ExplicitInterfaceSpecifierPattern : PatternNode
    {
        private readonly NamePattern _name;
        private readonly Action<ExplicitInterfaceSpecifierSyntax> _action;

        internal ExplicitInterfaceSpecifierPattern(NamePattern name, Action<ExplicitInterfaceSpecifierSyntax> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExplicitInterfaceSpecifierSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ExplicitInterfaceSpecifierSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class BaseMethodDeclarationPattern : MemberDeclarationPattern
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly ParameterListPattern _parameterList;
        private readonly BlockPattern _body;
        private readonly ArrowExpressionClausePattern _expressionBody;

        internal BaseMethodDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, BlockPattern body, ArrowExpressionClausePattern expressionBody)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _parameterList = parameterList;
            _body = body;
            _expressionBody = expressionBody;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseMethodDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;
            if (_expressionBody != null && !_expressionBody.Test(typed.ExpressionBody, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseMethodDeclarationSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);
            if (_body != null)
                _body.RunCallback(typed.Body, semanticModel);
            if (_expressionBody != null)
                _expressionBody.RunCallback(typed.ExpressionBody, semanticModel);
        }
    }

    public partial class MethodDeclarationPattern : BaseMethodDeclarationPattern
    {
        private readonly TypePattern _returnType;
        private readonly ExplicitInterfaceSpecifierPattern _explicitInterfaceSpecifier;
        private readonly string _identifier;
        private readonly TypeParameterListPattern _typeParameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern> _constraintClauses;
        private readonly Action<MethodDeclarationSyntax> _action;

        internal MethodDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, BlockPattern body, ArrowExpressionClausePattern expressionBody, TypePattern returnType, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier, string identifier, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, Action<MethodDeclarationSyntax> action)
            : base(attributeLists, modifiers, parameterList, body, expressionBody)
        {
            _returnType = returnType;
            _explicitInterfaceSpecifier = explicitInterfaceSpecifier;
            _identifier = identifier;
            _typeParameterList = typeParameterList;
            _constraintClauses = constraintClauses;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MethodDeclarationSyntax typed))
                return false;

            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;
            if (_explicitInterfaceSpecifier != null && !_explicitInterfaceSpecifier.Test(typed.ExplicitInterfaceSpecifier, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (MethodDeclarationSyntax)node;

            if (_returnType != null)
                _returnType.RunCallback(typed.ReturnType, semanticModel);
            if (_explicitInterfaceSpecifier != null)
                _explicitInterfaceSpecifier.RunCallback(typed.ExplicitInterfaceSpecifier, semanticModel);
            if (_typeParameterList != null)
                _typeParameterList.RunCallback(typed.TypeParameterList, semanticModel);
            if (_constraintClauses != null)
                _constraintClauses.RunCallback(typed.ConstraintClauses, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class OperatorDeclarationPattern : BaseMethodDeclarationPattern
    {
        private readonly TypePattern _returnType;
        private readonly Action<OperatorDeclarationSyntax> _action;

        internal OperatorDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, BlockPattern body, ArrowExpressionClausePattern expressionBody, TypePattern returnType, Action<OperatorDeclarationSyntax> action)
            : base(attributeLists, modifiers, parameterList, body, expressionBody)
        {
            _returnType = returnType;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OperatorDeclarationSyntax typed))
                return false;

            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (OperatorDeclarationSyntax)node;

            if (_returnType != null)
                _returnType.RunCallback(typed.ReturnType, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ConversionOperatorDeclarationPattern : BaseMethodDeclarationPattern
    {
        private readonly TypePattern _type;
        private readonly Action<ConversionOperatorDeclarationSyntax> _action;

        internal ConversionOperatorDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, BlockPattern body, ArrowExpressionClausePattern expressionBody, TypePattern type, Action<ConversionOperatorDeclarationSyntax> action)
            : base(attributeLists, modifiers, parameterList, body, expressionBody)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConversionOperatorDeclarationSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ConversionOperatorDeclarationSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ConstructorDeclarationPattern : BaseMethodDeclarationPattern
    {
        private readonly string _identifier;
        private readonly ConstructorInitializerPattern _initializer;
        private readonly Action<ConstructorDeclarationSyntax> _action;

        internal ConstructorDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, BlockPattern body, ArrowExpressionClausePattern expressionBody, string identifier, ConstructorInitializerPattern initializer, Action<ConstructorDeclarationSyntax> action)
            : base(attributeLists, modifiers, parameterList, body, expressionBody)
        {
            _identifier = identifier;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstructorDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ConstructorDeclarationSyntax)node;

            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ConstructorInitializerPattern : PatternNode
    {
        private readonly SyntaxKind _kind;
        private readonly ArgumentListPattern _argumentList;
        private readonly Action<ConstructorInitializerSyntax> _action;

        internal ConstructorInitializerPattern(SyntaxKind kind, ArgumentListPattern argumentList, Action<ConstructorInitializerSyntax> action)
        {
            _kind = kind;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstructorInitializerSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConstructorInitializerSyntax)node;

            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class DestructorDeclarationPattern : BaseMethodDeclarationPattern
    {
        private readonly string _identifier;
        private readonly Action<DestructorDeclarationSyntax> _action;

        internal DestructorDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, BlockPattern body, ArrowExpressionClausePattern expressionBody, string identifier, Action<DestructorDeclarationSyntax> action)
            : base(attributeLists, modifiers, parameterList, body, expressionBody)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DestructorDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (DestructorDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class BasePropertyDeclarationPattern : MemberDeclarationPattern
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern _type;
        private readonly ExplicitInterfaceSpecifierPattern _explicitInterfaceSpecifier;
        private readonly AccessorListPattern _accessorList;

        internal BasePropertyDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier, AccessorListPattern accessorList)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _type = type;
            _explicitInterfaceSpecifier = explicitInterfaceSpecifier;
            _accessorList = accessorList;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BasePropertyDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_explicitInterfaceSpecifier != null && !_explicitInterfaceSpecifier.Test(typed.ExplicitInterfaceSpecifier, semanticModel))
                return false;
            if (_accessorList != null && !_accessorList.Test(typed.AccessorList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BasePropertyDeclarationSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_explicitInterfaceSpecifier != null)
                _explicitInterfaceSpecifier.RunCallback(typed.ExplicitInterfaceSpecifier, semanticModel);
            if (_accessorList != null)
                _accessorList.RunCallback(typed.AccessorList, semanticModel);
        }
    }

    public partial class PropertyDeclarationPattern : BasePropertyDeclarationPattern
    {
        private readonly string _identifier;
        private readonly ArrowExpressionClausePattern _expressionBody;
        private readonly EqualsValueClausePattern _initializer;
        private readonly Action<PropertyDeclarationSyntax> _action;

        internal PropertyDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier, AccessorListPattern accessorList, string identifier, ArrowExpressionClausePattern expressionBody, EqualsValueClausePattern initializer, Action<PropertyDeclarationSyntax> action)
            : base(attributeLists, modifiers, type, explicitInterfaceSpecifier, accessorList)
        {
            _identifier = identifier;
            _expressionBody = expressionBody;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PropertyDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_expressionBody != null && !_expressionBody.Test(typed.ExpressionBody, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (PropertyDeclarationSyntax)node;

            if (_expressionBody != null)
                _expressionBody.RunCallback(typed.ExpressionBody, semanticModel);
            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class ArrowExpressionClausePattern : PatternNode
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ArrowExpressionClauseSyntax> _action;

        internal ArrowExpressionClausePattern(ExpressionPattern expression, Action<ArrowExpressionClauseSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrowExpressionClauseSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrowExpressionClauseSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class EventDeclarationPattern : BasePropertyDeclarationPattern
    {
        private readonly string _identifier;
        private readonly Action<EventDeclarationSyntax> _action;

        internal EventDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier, AccessorListPattern accessorList, string identifier, Action<EventDeclarationSyntax> action)
            : base(attributeLists, modifiers, type, explicitInterfaceSpecifier, accessorList)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EventDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (EventDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class IndexerDeclarationPattern : BasePropertyDeclarationPattern
    {
        private readonly BracketedParameterListPattern _parameterList;
        private readonly ArrowExpressionClausePattern _expressionBody;
        private readonly Action<IndexerDeclarationSyntax> _action;

        internal IndexerDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier, AccessorListPattern accessorList, BracketedParameterListPattern parameterList, ArrowExpressionClausePattern expressionBody, Action<IndexerDeclarationSyntax> action)
            : base(attributeLists, modifiers, type, explicitInterfaceSpecifier, accessorList)
        {
            _parameterList = parameterList;
            _expressionBody = expressionBody;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IndexerDeclarationSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;
            if (_expressionBody != null && !_expressionBody.Test(typed.ExpressionBody, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (IndexerDeclarationSyntax)node;

            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);
            if (_expressionBody != null)
                _expressionBody.RunCallback(typed.ExpressionBody, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AccessorListPattern : PatternNode
    {
        private readonly NodeListPattern<AccessorDeclarationPattern> _accessors;
        private readonly Action<AccessorListSyntax> _action;

        internal AccessorListPattern(NodeListPattern<AccessorDeclarationPattern> accessors, Action<AccessorListSyntax> action)
        {
            _accessors = accessors;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AccessorListSyntax typed))
                return false;

            if (_accessors != null && !_accessors.Test(typed.Accessors, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AccessorListSyntax)node;

            if (_accessors != null)
                _accessors.RunCallback(typed.Accessors, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class AccessorDeclarationPattern : PatternNode
    {
        private readonly SyntaxKind _kind;
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly BlockPattern _body;
        private readonly ArrowExpressionClausePattern _expressionBody;
        private readonly Action<AccessorDeclarationSyntax> _action;

        internal AccessorDeclarationPattern(SyntaxKind kind, NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, BlockPattern body, ArrowExpressionClausePattern expressionBody, Action<AccessorDeclarationSyntax> action)
        {
            _kind = kind;
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _body = body;
            _expressionBody = expressionBody;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AccessorDeclarationSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;
            if (_expressionBody != null && !_expressionBody.Test(typed.ExpressionBody, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AccessorDeclarationSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_body != null)
                _body.RunCallback(typed.Body, semanticModel);
            if (_expressionBody != null)
                _expressionBody.RunCallback(typed.ExpressionBody, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public abstract partial class BaseParameterListPattern : PatternNode
    {
        private readonly NodeListPattern<ParameterPattern> _parameters;

        internal BaseParameterListPattern(NodeListPattern<ParameterPattern> parameters)
        {
            _parameters = parameters;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseParameterListSyntax typed))
                return false;

            if (_parameters != null && !_parameters.Test(typed.Parameters, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseParameterListSyntax)node;

            if (_parameters != null)
                _parameters.RunCallback(typed.Parameters, semanticModel);
        }
    }

    public partial class ParameterListPattern : BaseParameterListPattern
    {
        private readonly Action<ParameterListSyntax> _action;

        internal ParameterListPattern(NodeListPattern<ParameterPattern> parameters, Action<ParameterListSyntax> action)
            : base(parameters)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParameterListSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ParameterListSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class BracketedParameterListPattern : BaseParameterListPattern
    {
        private readonly Action<BracketedParameterListSyntax> _action;

        internal BracketedParameterListPattern(NodeListPattern<ParameterPattern> parameters, Action<BracketedParameterListSyntax> action)
            : base(parameters)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BracketedParameterListSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (BracketedParameterListSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    public partial class ParameterPattern : PatternNode
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly EqualsValueClausePattern _default;
        private readonly Action<ParameterSyntax> _action;

        internal ParameterPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, string identifier, EqualsValueClausePattern @default, Action<ParameterSyntax> action)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _type = type;
            _identifier = identifier;
            _default = @default;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParameterSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_default != null && !_default.Test(typed.Default, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParameterSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_default != null)
                _default.RunCallback(typed.Default, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    public partial class IncompleteMemberPattern : MemberDeclarationPattern
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern _type;
        private readonly Action<IncompleteMemberSyntax> _action;

        internal IncompleteMemberPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, Action<IncompleteMemberSyntax> action)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IncompleteMemberSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (IncompleteMemberSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    partial class Pattern
    {
        public static IdentifierNamePattern IdentifierName(string identifier = null, Action<IdentifierNameSyntax> action = null)
        {
            return new IdentifierNamePattern(identifier, action);
        }
        public static QualifiedNamePattern QualifiedName(NamePattern left = null, SimpleNamePattern right = null, Action<QualifiedNameSyntax> action = null)
        {
            return new QualifiedNamePattern(left, right, action);
        }
        public static GenericNamePattern GenericName(string identifier = null, TypeArgumentListPattern typeArgumentList = null, Action<GenericNameSyntax> action = null)
        {
            return new GenericNamePattern(identifier, typeArgumentList, action);
        }
        public static TypeArgumentListPattern TypeArgumentList(IEnumerable<TypePattern> arguments = null, Action<TypeArgumentListSyntax> action = null)
        {
            return new TypeArgumentListPattern(NodeList(arguments), action);
        }

        public static TypeArgumentListPattern TypeArgumentList(params TypePattern[] arguments)
        {
            return new TypeArgumentListPattern(NodeList(arguments), null);
        }
        public static AliasQualifiedNamePattern AliasQualifiedName(IdentifierNamePattern alias = null, SimpleNamePattern name = null, Action<AliasQualifiedNameSyntax> action = null)
        {
            return new AliasQualifiedNamePattern(alias, name, action);
        }
        public static PredefinedTypePattern PredefinedType(string keyword = null, Action<PredefinedTypeSyntax> action = null)
        {
            return new PredefinedTypePattern(keyword, action);
        }
        public static ArrayTypePattern ArrayType(TypePattern elementType = null, IEnumerable<ArrayRankSpecifierPattern> rankSpecifiers = null, Action<ArrayTypeSyntax> action = null)
        {
            return new ArrayTypePattern(elementType, NodeList(rankSpecifiers), action);
        }
        public static ArrayRankSpecifierPattern ArrayRankSpecifier(IEnumerable<ExpressionPattern> sizes = null, Action<ArrayRankSpecifierSyntax> action = null)
        {
            return new ArrayRankSpecifierPattern(NodeList(sizes), action);
        }

        public static ArrayRankSpecifierPattern ArrayRankSpecifier(params ExpressionPattern[] sizes)
        {
            return new ArrayRankSpecifierPattern(NodeList(sizes), null);
        }
        public static PointerTypePattern PointerType(TypePattern elementType = null, Action<PointerTypeSyntax> action = null)
        {
            return new PointerTypePattern(elementType, action);
        }
        public static NullableTypePattern NullableType(TypePattern elementType = null, Action<NullableTypeSyntax> action = null)
        {
            return new NullableTypePattern(elementType, action);
        }
        public static TupleTypePattern TupleType(IEnumerable<TupleElementPattern> elements = null, Action<TupleTypeSyntax> action = null)
        {
            return new TupleTypePattern(NodeList(elements), action);
        }

        public static TupleTypePattern TupleType(params TupleElementPattern[] elements)
        {
            return new TupleTypePattern(NodeList(elements), null);
        }
        public static TupleElementPattern TupleElement(TypePattern type = null, string identifier = null, Action<TupleElementSyntax> action = null)
        {
            return new TupleElementPattern(type, identifier, action);
        }
        public static OmittedTypeArgumentPattern OmittedTypeArgument(Action<OmittedTypeArgumentSyntax> action = null)
        {
            return new OmittedTypeArgumentPattern(action);
        }
        public static RefTypePattern RefType(TypePattern type = null, Action<RefTypeSyntax> action = null)
        {
            return new RefTypePattern(type, action);
        }
        public static ParenthesizedExpressionPattern ParenthesizedExpression(ExpressionPattern expression = null, Action<ParenthesizedExpressionSyntax> action = null)
        {
            return new ParenthesizedExpressionPattern(expression, action);
        }
        public static TupleExpressionPattern TupleExpression(IEnumerable<ArgumentPattern> arguments = null, Action<TupleExpressionSyntax> action = null)
        {
            return new TupleExpressionPattern(NodeList(arguments), action);
        }

        public static TupleExpressionPattern TupleExpression(params ArgumentPattern[] arguments)
        {
            return new TupleExpressionPattern(NodeList(arguments), null);
        }
        public static PrefixUnaryExpressionPattern PrefixUnaryExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern operand = null, Action<PrefixUnaryExpressionSyntax> action = null)
        {
            return new PrefixUnaryExpressionPattern(kind, operand, action);
        }
        public static AwaitExpressionPattern AwaitExpression(ExpressionPattern expression = null, Action<AwaitExpressionSyntax> action = null)
        {
            return new AwaitExpressionPattern(expression, action);
        }
        public static PostfixUnaryExpressionPattern PostfixUnaryExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern operand = null, Action<PostfixUnaryExpressionSyntax> action = null)
        {
            return new PostfixUnaryExpressionPattern(kind, operand, action);
        }
        public static MemberAccessExpressionPattern MemberAccessExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern expression = null, SimpleNamePattern name = null, Action<MemberAccessExpressionSyntax> action = null)
        {
            return new MemberAccessExpressionPattern(kind, expression, name, action);
        }
        public static ConditionalAccessExpressionPattern ConditionalAccessExpression(ExpressionPattern expression = null, ExpressionPattern whenNotNull = null, Action<ConditionalAccessExpressionSyntax> action = null)
        {
            return new ConditionalAccessExpressionPattern(expression, whenNotNull, action);
        }
        public static MemberBindingExpressionPattern MemberBindingExpression(SimpleNamePattern name = null, Action<MemberBindingExpressionSyntax> action = null)
        {
            return new MemberBindingExpressionPattern(name, action);
        }
        public static ElementBindingExpressionPattern ElementBindingExpression(BracketedArgumentListPattern argumentList = null, Action<ElementBindingExpressionSyntax> action = null)
        {
            return new ElementBindingExpressionPattern(argumentList, action);
        }
        public static ImplicitElementAccessPattern ImplicitElementAccess(BracketedArgumentListPattern argumentList = null, Action<ImplicitElementAccessSyntax> action = null)
        {
            return new ImplicitElementAccessPattern(argumentList, action);
        }
        public static BinaryExpressionPattern BinaryExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern left = null, ExpressionPattern right = null, Action<BinaryExpressionSyntax> action = null)
        {
            return new BinaryExpressionPattern(kind, left, right, action);
        }
        public static AssignmentExpressionPattern AssignmentExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern left = null, ExpressionPattern right = null, Action<AssignmentExpressionSyntax> action = null)
        {
            return new AssignmentExpressionPattern(kind, left, right, action);
        }
        public static ConditionalExpressionPattern ConditionalExpression(ExpressionPattern condition = null, ExpressionPattern whenTrue = null, ExpressionPattern whenFalse = null, Action<ConditionalExpressionSyntax> action = null)
        {
            return new ConditionalExpressionPattern(condition, whenTrue, whenFalse, action);
        }
        public static ThisExpressionPattern ThisExpression(Action<ThisExpressionSyntax> action = null)
        {
            return new ThisExpressionPattern(action);
        }
        public static BaseExpressionPattern BaseExpression(Action<BaseExpressionSyntax> action = null)
        {
            return new BaseExpressionPattern(action);
        }
        public static LiteralExpressionPattern LiteralExpression(SyntaxKind kind = default(SyntaxKind), Action<LiteralExpressionSyntax> action = null)
        {
            return new LiteralExpressionPattern(kind, action);
        }
        public static MakeRefExpressionPattern MakeRefExpression(ExpressionPattern expression = null, Action<MakeRefExpressionSyntax> action = null)
        {
            return new MakeRefExpressionPattern(expression, action);
        }
        public static RefTypeExpressionPattern RefTypeExpression(ExpressionPattern expression = null, Action<RefTypeExpressionSyntax> action = null)
        {
            return new RefTypeExpressionPattern(expression, action);
        }
        public static RefValueExpressionPattern RefValueExpression(ExpressionPattern expression = null, TypePattern type = null, Action<RefValueExpressionSyntax> action = null)
        {
            return new RefValueExpressionPattern(expression, type, action);
        }
        public static CheckedExpressionPattern CheckedExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern expression = null, Action<CheckedExpressionSyntax> action = null)
        {
            return new CheckedExpressionPattern(kind, expression, action);
        }
        public static DefaultExpressionPattern DefaultExpression(TypePattern type = null, Action<DefaultExpressionSyntax> action = null)
        {
            return new DefaultExpressionPattern(type, action);
        }
        public static TypeOfExpressionPattern TypeOfExpression(TypePattern type = null, Action<TypeOfExpressionSyntax> action = null)
        {
            return new TypeOfExpressionPattern(type, action);
        }
        public static SizeOfExpressionPattern SizeOfExpression(TypePattern type = null, Action<SizeOfExpressionSyntax> action = null)
        {
            return new SizeOfExpressionPattern(type, action);
        }
        public static InvocationExpressionPattern InvocationExpression(ExpressionPattern expression = null, ArgumentListPattern argumentList = null, Action<InvocationExpressionSyntax> action = null)
        {
            return new InvocationExpressionPattern(expression, argumentList, action);
        }
        public static ElementAccessExpressionPattern ElementAccessExpression(ExpressionPattern expression = null, BracketedArgumentListPattern argumentList = null, Action<ElementAccessExpressionSyntax> action = null)
        {
            return new ElementAccessExpressionPattern(expression, argumentList, action);
        }
        public static ArgumentListPattern ArgumentList(IEnumerable<ArgumentPattern> arguments = null, Action<ArgumentListSyntax> action = null)
        {
            return new ArgumentListPattern(NodeList(arguments), action);
        }

        public static ArgumentListPattern ArgumentList(params ArgumentPattern[] arguments)
        {
            return new ArgumentListPattern(NodeList(arguments), null);
        }
        public static BracketedArgumentListPattern BracketedArgumentList(IEnumerable<ArgumentPattern> arguments = null, Action<BracketedArgumentListSyntax> action = null)
        {
            return new BracketedArgumentListPattern(NodeList(arguments), action);
        }

        public static BracketedArgumentListPattern BracketedArgumentList(params ArgumentPattern[] arguments)
        {
            return new BracketedArgumentListPattern(NodeList(arguments), null);
        }
        public static ArgumentPattern Argument(NameColonPattern nameColon = null, ExpressionPattern expression = null, Action<ArgumentSyntax> action = null)
        {
            return new ArgumentPattern(nameColon, expression, action);
        }
        public static NameColonPattern NameColon(IdentifierNamePattern name = null, Action<NameColonSyntax> action = null)
        {
            return new NameColonPattern(name, action);
        }
        public static DeclarationExpressionPattern DeclarationExpression(TypePattern type = null, VariableDesignationPattern designation = null, Action<DeclarationExpressionSyntax> action = null)
        {
            return new DeclarationExpressionPattern(type, designation, action);
        }
        public static CastExpressionPattern CastExpression(TypePattern type = null, ExpressionPattern expression = null, Action<CastExpressionSyntax> action = null)
        {
            return new CastExpressionPattern(type, expression, action);
        }
        public static AnonymousMethodExpressionPattern AnonymousMethodExpression(PatternNode body = null, ParameterListPattern parameterList = null, Action<AnonymousMethodExpressionSyntax> action = null)
        {
            return new AnonymousMethodExpressionPattern(body, parameterList, action);
        }
        public static SimpleLambdaExpressionPattern SimpleLambdaExpression(PatternNode body = null, ParameterPattern parameter = null, Action<SimpleLambdaExpressionSyntax> action = null)
        {
            return new SimpleLambdaExpressionPattern(body, parameter, action);
        }
        public static RefExpressionPattern RefExpression(ExpressionPattern expression = null, Action<RefExpressionSyntax> action = null)
        {
            return new RefExpressionPattern(expression, action);
        }
        public static ParenthesizedLambdaExpressionPattern ParenthesizedLambdaExpression(PatternNode body = null, ParameterListPattern parameterList = null, Action<ParenthesizedLambdaExpressionSyntax> action = null)
        {
            return new ParenthesizedLambdaExpressionPattern(body, parameterList, action);
        }
        public static InitializerExpressionPattern InitializerExpression(SyntaxKind kind = default(SyntaxKind), IEnumerable<ExpressionPattern> expressions = null, Action<InitializerExpressionSyntax> action = null)
        {
            return new InitializerExpressionPattern(kind, NodeList(expressions), action);
        }

        public static InitializerExpressionPattern InitializerExpression(SyntaxKind kind, params ExpressionPattern[] expressions)
        {
            return new InitializerExpressionPattern(kind, NodeList(expressions), null);
        }
        public static ObjectCreationExpressionPattern ObjectCreationExpression(TypePattern type = null, ArgumentListPattern argumentList = null, InitializerExpressionPattern initializer = null, Action<ObjectCreationExpressionSyntax> action = null)
        {
            return new ObjectCreationExpressionPattern(type, argumentList, initializer, action);
        }
        public static AnonymousObjectMemberDeclaratorPattern AnonymousObjectMemberDeclarator(NameEqualsPattern nameEquals = null, ExpressionPattern expression = null, Action<AnonymousObjectMemberDeclaratorSyntax> action = null)
        {
            return new AnonymousObjectMemberDeclaratorPattern(nameEquals, expression, action);
        }
        public static AnonymousObjectCreationExpressionPattern AnonymousObjectCreationExpression(IEnumerable<AnonymousObjectMemberDeclaratorPattern> initializers = null, Action<AnonymousObjectCreationExpressionSyntax> action = null)
        {
            return new AnonymousObjectCreationExpressionPattern(NodeList(initializers), action);
        }

        public static AnonymousObjectCreationExpressionPattern AnonymousObjectCreationExpression(params AnonymousObjectMemberDeclaratorPattern[] initializers)
        {
            return new AnonymousObjectCreationExpressionPattern(NodeList(initializers), null);
        }
        public static ArrayCreationExpressionPattern ArrayCreationExpression(ArrayTypePattern type = null, InitializerExpressionPattern initializer = null, Action<ArrayCreationExpressionSyntax> action = null)
        {
            return new ArrayCreationExpressionPattern(type, initializer, action);
        }
        public static ImplicitArrayCreationExpressionPattern ImplicitArrayCreationExpression(InitializerExpressionPattern initializer = null, Action<ImplicitArrayCreationExpressionSyntax> action = null)
        {
            return new ImplicitArrayCreationExpressionPattern(initializer, action);
        }
        public static StackAllocArrayCreationExpressionPattern StackAllocArrayCreationExpression(TypePattern type = null, Action<StackAllocArrayCreationExpressionSyntax> action = null)
        {
            return new StackAllocArrayCreationExpressionPattern(type, action);
        }
        public static QueryExpressionPattern QueryExpression(FromClausePattern fromClause = null, QueryBodyPattern body = null, Action<QueryExpressionSyntax> action = null)
        {
            return new QueryExpressionPattern(fromClause, body, action);
        }
        public static QueryBodyPattern QueryBody(IEnumerable<QueryClausePattern> clauses = null, SelectOrGroupClausePattern selectOrGroup = null, QueryContinuationPattern continuation = null, Action<QueryBodySyntax> action = null)
        {
            return new QueryBodyPattern(NodeList(clauses), selectOrGroup, continuation, action);
        }
        public static FromClausePattern FromClause(TypePattern type = null, string identifier = null, ExpressionPattern expression = null, Action<FromClauseSyntax> action = null)
        {
            return new FromClausePattern(type, identifier, expression, action);
        }
        public static LetClausePattern LetClause(string identifier = null, ExpressionPattern expression = null, Action<LetClauseSyntax> action = null)
        {
            return new LetClausePattern(identifier, expression, action);
        }
        public static JoinClausePattern JoinClause(TypePattern type = null, string identifier = null, ExpressionPattern inExpression = null, ExpressionPattern leftExpression = null, ExpressionPattern rightExpression = null, JoinIntoClausePattern into = null, Action<JoinClauseSyntax> action = null)
        {
            return new JoinClausePattern(type, identifier, inExpression, leftExpression, rightExpression, into, action);
        }
        public static JoinIntoClausePattern JoinIntoClause(string identifier = null, Action<JoinIntoClauseSyntax> action = null)
        {
            return new JoinIntoClausePattern(identifier, action);
        }
        public static WhereClausePattern WhereClause(ExpressionPattern condition = null, Action<WhereClauseSyntax> action = null)
        {
            return new WhereClausePattern(condition, action);
        }
        public static OrderByClausePattern OrderByClause(IEnumerable<OrderingPattern> orderings = null, Action<OrderByClauseSyntax> action = null)
        {
            return new OrderByClausePattern(NodeList(orderings), action);
        }

        public static OrderByClausePattern OrderByClause(params OrderingPattern[] orderings)
        {
            return new OrderByClausePattern(NodeList(orderings), null);
        }
        public static OrderingPattern Ordering(SyntaxKind kind = default(SyntaxKind), ExpressionPattern expression = null, Action<OrderingSyntax> action = null)
        {
            return new OrderingPattern(kind, expression, action);
        }
        public static SelectClausePattern SelectClause(ExpressionPattern expression = null, Action<SelectClauseSyntax> action = null)
        {
            return new SelectClausePattern(expression, action);
        }
        public static GroupClausePattern GroupClause(ExpressionPattern groupExpression = null, ExpressionPattern byExpression = null, Action<GroupClauseSyntax> action = null)
        {
            return new GroupClausePattern(groupExpression, byExpression, action);
        }
        public static QueryContinuationPattern QueryContinuation(string identifier = null, QueryBodyPattern body = null, Action<QueryContinuationSyntax> action = null)
        {
            return new QueryContinuationPattern(identifier, body, action);
        }
        public static OmittedArraySizeExpressionPattern OmittedArraySizeExpression(Action<OmittedArraySizeExpressionSyntax> action = null)
        {
            return new OmittedArraySizeExpressionPattern(action);
        }
        public static InterpolatedStringExpressionPattern InterpolatedStringExpression(IEnumerable<InterpolatedStringContentPattern> contents = null, Action<InterpolatedStringExpressionSyntax> action = null)
        {
            return new InterpolatedStringExpressionPattern(NodeList(contents), action);
        }

        public static InterpolatedStringExpressionPattern InterpolatedStringExpression(params InterpolatedStringContentPattern[] contents)
        {
            return new InterpolatedStringExpressionPattern(NodeList(contents), null);
        }
        public static IsPatternExpressionPattern IsPatternExpression(ExpressionPattern expression = null, PatternPattern pattern = null, Action<IsPatternExpressionSyntax> action = null)
        {
            return new IsPatternExpressionPattern(expression, pattern, action);
        }
        public static ThrowExpressionPattern ThrowExpression(ExpressionPattern expression = null, Action<ThrowExpressionSyntax> action = null)
        {
            return new ThrowExpressionPattern(expression, action);
        }
        public static WhenClausePattern WhenClause(ExpressionPattern condition = null, Action<WhenClauseSyntax> action = null)
        {
            return new WhenClausePattern(condition, action);
        }
        public static DeclarationPatternPattern DeclarationPattern(TypePattern type = null, VariableDesignationPattern designation = null, Action<DeclarationPatternSyntax> action = null)
        {
            return new DeclarationPatternPattern(type, designation, action);
        }
        public static ConstantPatternPattern ConstantPattern(ExpressionPattern expression = null, Action<ConstantPatternSyntax> action = null)
        {
            return new ConstantPatternPattern(expression, action);
        }
        public static InterpolatedStringTextPattern InterpolatedStringText(Action<InterpolatedStringTextSyntax> action = null)
        {
            return new InterpolatedStringTextPattern(action);
        }
        public static InterpolationPattern Interpolation(ExpressionPattern expression = null, InterpolationAlignmentClausePattern alignmentClause = null, InterpolationFormatClausePattern formatClause = null, Action<InterpolationSyntax> action = null)
        {
            return new InterpolationPattern(expression, alignmentClause, formatClause, action);
        }
        public static InterpolationAlignmentClausePattern InterpolationAlignmentClause(ExpressionPattern value = null, Action<InterpolationAlignmentClauseSyntax> action = null)
        {
            return new InterpolationAlignmentClausePattern(value, action);
        }
        public static InterpolationFormatClausePattern InterpolationFormatClause(Action<InterpolationFormatClauseSyntax> action = null)
        {
            return new InterpolationFormatClausePattern(action);
        }
        public static GlobalStatementPattern GlobalStatement(StatementPattern statement = null, Action<GlobalStatementSyntax> action = null)
        {
            return new GlobalStatementPattern(statement, action);
        }
        public static BlockPattern Block(IEnumerable<StatementPattern> statements = null, Action<BlockSyntax> action = null)
        {
            return new BlockPattern(NodeList(statements), action);
        }

        public static BlockPattern Block(params StatementPattern[] statements)
        {
            return new BlockPattern(NodeList(statements), null);
        }
        public static LocalFunctionStatementPattern LocalFunctionStatement(IEnumerable<string> modifiers = null, TypePattern returnType = null, string identifier = null, TypeParameterListPattern typeParameterList = null, ParameterListPattern parameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, BlockPattern body = null, ArrowExpressionClausePattern expressionBody = null, Action<LocalFunctionStatementSyntax> action = null)
        {
            return new LocalFunctionStatementPattern(TokenList(modifiers), returnType, identifier, typeParameterList, parameterList, NodeList(constraintClauses), body, expressionBody, action);
        }
        public static LocalDeclarationStatementPattern LocalDeclarationStatement(IEnumerable<string> modifiers = null, VariableDeclarationPattern declaration = null, Action<LocalDeclarationStatementSyntax> action = null)
        {
            return new LocalDeclarationStatementPattern(TokenList(modifiers), declaration, action);
        }
        public static VariableDeclarationPattern VariableDeclaration(TypePattern type = null, IEnumerable<VariableDeclaratorPattern> variables = null, Action<VariableDeclarationSyntax> action = null)
        {
            return new VariableDeclarationPattern(type, NodeList(variables), action);
        }

        public static VariableDeclarationPattern VariableDeclaration(params VariableDeclaratorPattern[] variables)
        {
            return new VariableDeclarationPattern(null, NodeList(variables), null);
        }
        public static VariableDeclaratorPattern VariableDeclarator(string identifier = null, BracketedArgumentListPattern argumentList = null, EqualsValueClausePattern initializer = null, Action<VariableDeclaratorSyntax> action = null)
        {
            return new VariableDeclaratorPattern(identifier, argumentList, initializer, action);
        }
        public static EqualsValueClausePattern EqualsValueClause(ExpressionPattern value = null, Action<EqualsValueClauseSyntax> action = null)
        {
            return new EqualsValueClausePattern(value, action);
        }
        public static SingleVariableDesignationPattern SingleVariableDesignation(string identifier = null, Action<SingleVariableDesignationSyntax> action = null)
        {
            return new SingleVariableDesignationPattern(identifier, action);
        }
        public static DiscardDesignationPattern DiscardDesignation(Action<DiscardDesignationSyntax> action = null)
        {
            return new DiscardDesignationPattern(action);
        }
        public static ParenthesizedVariableDesignationPattern ParenthesizedVariableDesignation(IEnumerable<VariableDesignationPattern> variables = null, Action<ParenthesizedVariableDesignationSyntax> action = null)
        {
            return new ParenthesizedVariableDesignationPattern(NodeList(variables), action);
        }

        public static ParenthesizedVariableDesignationPattern ParenthesizedVariableDesignation(params VariableDesignationPattern[] variables)
        {
            return new ParenthesizedVariableDesignationPattern(NodeList(variables), null);
        }
        public static ExpressionStatementPattern ExpressionStatement(ExpressionPattern expression = null, Action<ExpressionStatementSyntax> action = null)
        {
            return new ExpressionStatementPattern(expression, action);
        }
        public static EmptyStatementPattern EmptyStatement(Action<EmptyStatementSyntax> action = null)
        {
            return new EmptyStatementPattern(action);
        }
        public static LabeledStatementPattern LabeledStatement(string identifier = null, StatementPattern statement = null, Action<LabeledStatementSyntax> action = null)
        {
            return new LabeledStatementPattern(identifier, statement, action);
        }
        public static GotoStatementPattern GotoStatement(SyntaxKind kind = default(SyntaxKind), ExpressionPattern expression = null, Action<GotoStatementSyntax> action = null)
        {
            return new GotoStatementPattern(kind, expression, action);
        }
        public static BreakStatementPattern BreakStatement(Action<BreakStatementSyntax> action = null)
        {
            return new BreakStatementPattern(action);
        }
        public static ContinueStatementPattern ContinueStatement(Action<ContinueStatementSyntax> action = null)
        {
            return new ContinueStatementPattern(action);
        }
        public static ReturnStatementPattern ReturnStatement(ExpressionPattern expression = null, Action<ReturnStatementSyntax> action = null)
        {
            return new ReturnStatementPattern(expression, action);
        }
        public static ThrowStatementPattern ThrowStatement(ExpressionPattern expression = null, Action<ThrowStatementSyntax> action = null)
        {
            return new ThrowStatementPattern(expression, action);
        }
        public static YieldStatementPattern YieldStatement(SyntaxKind kind = default(SyntaxKind), ExpressionPattern expression = null, Action<YieldStatementSyntax> action = null)
        {
            return new YieldStatementPattern(kind, expression, action);
        }
        public static WhileStatementPattern WhileStatement(ExpressionPattern condition = null, StatementPattern statement = null, Action<WhileStatementSyntax> action = null)
        {
            return new WhileStatementPattern(condition, statement, action);
        }
        public static DoStatementPattern DoStatement(StatementPattern statement = null, ExpressionPattern condition = null, Action<DoStatementSyntax> action = null)
        {
            return new DoStatementPattern(statement, condition, action);
        }
        public static ForStatementPattern ForStatement(VariableDeclarationPattern declaration = null, IEnumerable<ExpressionPattern> initializers = null, ExpressionPattern condition = null, IEnumerable<ExpressionPattern> incrementors = null, StatementPattern statement = null, Action<ForStatementSyntax> action = null)
        {
            return new ForStatementPattern(declaration, NodeList(initializers), condition, NodeList(incrementors), statement, action);
        }
        public static ForEachStatementPattern ForEachStatement(ExpressionPattern expression = null, StatementPattern statement = null, TypePattern type = null, string identifier = null, Action<ForEachStatementSyntax> action = null)
        {
            return new ForEachStatementPattern(expression, statement, type, identifier, action);
        }
        public static ForEachVariableStatementPattern ForEachVariableStatement(ExpressionPattern expression = null, StatementPattern statement = null, ExpressionPattern variable = null, Action<ForEachVariableStatementSyntax> action = null)
        {
            return new ForEachVariableStatementPattern(expression, statement, variable, action);
        }
        public static UsingStatementPattern UsingStatement(VariableDeclarationPattern declaration = null, ExpressionPattern expression = null, StatementPattern statement = null, Action<UsingStatementSyntax> action = null)
        {
            return new UsingStatementPattern(declaration, expression, statement, action);
        }
        public static FixedStatementPattern FixedStatement(VariableDeclarationPattern declaration = null, StatementPattern statement = null, Action<FixedStatementSyntax> action = null)
        {
            return new FixedStatementPattern(declaration, statement, action);
        }
        public static CheckedStatementPattern CheckedStatement(SyntaxKind kind = default(SyntaxKind), BlockPattern block = null, Action<CheckedStatementSyntax> action = null)
        {
            return new CheckedStatementPattern(kind, block, action);
        }
        public static UnsafeStatementPattern UnsafeStatement(BlockPattern block = null, Action<UnsafeStatementSyntax> action = null)
        {
            return new UnsafeStatementPattern(block, action);
        }
        public static LockStatementPattern LockStatement(ExpressionPattern expression = null, StatementPattern statement = null, Action<LockStatementSyntax> action = null)
        {
            return new LockStatementPattern(expression, statement, action);
        }
        public static IfStatementPattern IfStatement(ExpressionPattern condition = null, StatementPattern statement = null, ElseClausePattern @else = null, Action<IfStatementSyntax> action = null)
        {
            return new IfStatementPattern(condition, statement, @else, action);
        }
        public static ElseClausePattern ElseClause(StatementPattern statement = null, Action<ElseClauseSyntax> action = null)
        {
            return new ElseClausePattern(statement, action);
        }
        public static SwitchStatementPattern SwitchStatement(ExpressionPattern expression = null, IEnumerable<SwitchSectionPattern> sections = null, Action<SwitchStatementSyntax> action = null)
        {
            return new SwitchStatementPattern(expression, NodeList(sections), action);
        }

        public static SwitchStatementPattern SwitchStatement(params SwitchSectionPattern[] sections)
        {
            return new SwitchStatementPattern(null, NodeList(sections), null);
        }
        public static SwitchSectionPattern SwitchSection(IEnumerable<SwitchLabelPattern> labels = null, IEnumerable<StatementPattern> statements = null, Action<SwitchSectionSyntax> action = null)
        {
            return new SwitchSectionPattern(NodeList(labels), NodeList(statements), action);
        }
        public static CasePatternSwitchLabelPattern CasePatternSwitchLabel(PatternPattern pattern = null, WhenClausePattern whenClause = null, Action<CasePatternSwitchLabelSyntax> action = null)
        {
            return new CasePatternSwitchLabelPattern(pattern, whenClause, action);
        }
        public static CaseSwitchLabelPattern CaseSwitchLabel(ExpressionPattern value = null, Action<CaseSwitchLabelSyntax> action = null)
        {
            return new CaseSwitchLabelPattern(value, action);
        }
        public static DefaultSwitchLabelPattern DefaultSwitchLabel(Action<DefaultSwitchLabelSyntax> action = null)
        {
            return new DefaultSwitchLabelPattern(action);
        }
        public static TryStatementPattern TryStatement(BlockPattern block = null, IEnumerable<CatchClausePattern> catches = null, FinallyClausePattern @finally = null, Action<TryStatementSyntax> action = null)
        {
            return new TryStatementPattern(block, NodeList(catches), @finally, action);
        }
        public static CatchClausePattern CatchClause(CatchDeclarationPattern declaration = null, CatchFilterClausePattern filter = null, BlockPattern block = null, Action<CatchClauseSyntax> action = null)
        {
            return new CatchClausePattern(declaration, filter, block, action);
        }
        public static CatchDeclarationPattern CatchDeclaration(TypePattern type = null, string identifier = null, Action<CatchDeclarationSyntax> action = null)
        {
            return new CatchDeclarationPattern(type, identifier, action);
        }
        public static CatchFilterClausePattern CatchFilterClause(ExpressionPattern filterExpression = null, Action<CatchFilterClauseSyntax> action = null)
        {
            return new CatchFilterClausePattern(filterExpression, action);
        }
        public static FinallyClausePattern FinallyClause(BlockPattern block = null, Action<FinallyClauseSyntax> action = null)
        {
            return new FinallyClausePattern(block, action);
        }
        public static CompilationUnitPattern CompilationUnit(IEnumerable<ExternAliasDirectivePattern> externs = null, IEnumerable<UsingDirectivePattern> usings = null, IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<MemberDeclarationPattern> members = null, Action<CompilationUnitSyntax> action = null)
        {
            return new CompilationUnitPattern(NodeList(externs), NodeList(usings), NodeList(attributeLists), NodeList(members), action);
        }
        public static ExternAliasDirectivePattern ExternAliasDirective(string identifier = null, Action<ExternAliasDirectiveSyntax> action = null)
        {
            return new ExternAliasDirectivePattern(identifier, action);
        }
        public static UsingDirectivePattern UsingDirective(NameEqualsPattern alias = null, NamePattern name = null, Action<UsingDirectiveSyntax> action = null)
        {
            return new UsingDirectivePattern(alias, name, action);
        }
        public static NamespaceDeclarationPattern NamespaceDeclaration(NamePattern name = null, IEnumerable<ExternAliasDirectivePattern> externs = null, IEnumerable<UsingDirectivePattern> usings = null, IEnumerable<MemberDeclarationPattern> members = null, Action<NamespaceDeclarationSyntax> action = null)
        {
            return new NamespaceDeclarationPattern(name, NodeList(externs), NodeList(usings), NodeList(members), action);
        }
        public static AttributeListPattern AttributeList(AttributeTargetSpecifierPattern target = null, IEnumerable<AttributePattern> attributes = null, Action<AttributeListSyntax> action = null)
        {
            return new AttributeListPattern(target, NodeList(attributes), action);
        }

        public static AttributeListPattern AttributeList(params AttributePattern[] attributes)
        {
            return new AttributeListPattern(null, NodeList(attributes), null);
        }
        public static AttributeTargetSpecifierPattern AttributeTargetSpecifier(string identifier = null, Action<AttributeTargetSpecifierSyntax> action = null)
        {
            return new AttributeTargetSpecifierPattern(identifier, action);
        }
        public static AttributePattern Attribute(NamePattern name = null, AttributeArgumentListPattern argumentList = null, Action<AttributeSyntax> action = null)
        {
            return new AttributePattern(name, argumentList, action);
        }
        public static AttributeArgumentListPattern AttributeArgumentList(IEnumerable<AttributeArgumentPattern> arguments = null, Action<AttributeArgumentListSyntax> action = null)
        {
            return new AttributeArgumentListPattern(NodeList(arguments), action);
        }

        public static AttributeArgumentListPattern AttributeArgumentList(params AttributeArgumentPattern[] arguments)
        {
            return new AttributeArgumentListPattern(NodeList(arguments), null);
        }
        public static AttributeArgumentPattern AttributeArgument(NameEqualsPattern nameEquals = null, NameColonPattern nameColon = null, ExpressionPattern expression = null, Action<AttributeArgumentSyntax> action = null)
        {
            return new AttributeArgumentPattern(nameEquals, nameColon, expression, action);
        }
        public static NameEqualsPattern NameEquals(IdentifierNamePattern name = null, Action<NameEqualsSyntax> action = null)
        {
            return new NameEqualsPattern(name, action);
        }
        public static TypeParameterListPattern TypeParameterList(IEnumerable<TypeParameterPattern> parameters = null, Action<TypeParameterListSyntax> action = null)
        {
            return new TypeParameterListPattern(NodeList(parameters), action);
        }

        public static TypeParameterListPattern TypeParameterList(params TypeParameterPattern[] parameters)
        {
            return new TypeParameterListPattern(NodeList(parameters), null);
        }
        public static TypeParameterPattern TypeParameter(IEnumerable<AttributeListPattern> attributeLists = null, string identifier = null, Action<TypeParameterSyntax> action = null)
        {
            return new TypeParameterPattern(NodeList(attributeLists), identifier, action);
        }
        public static ClassDeclarationPattern ClassDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern baseList = null, TypeParameterListPattern typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, IEnumerable<MemberDeclarationPattern> members = null, Action<ClassDeclarationSyntax> action = null)
        {
            return new ClassDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, typeParameterList, NodeList(constraintClauses), NodeList(members), action);
        }

        public static ClassDeclarationPattern ClassDeclaration(params MemberDeclarationPattern[] members)
        {
            return new ClassDeclarationPattern(null, null, null, null, null, null, NodeList(members), null);
        }
        public static StructDeclarationPattern StructDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern baseList = null, TypeParameterListPattern typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, IEnumerable<MemberDeclarationPattern> members = null, Action<StructDeclarationSyntax> action = null)
        {
            return new StructDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, typeParameterList, NodeList(constraintClauses), NodeList(members), action);
        }

        public static StructDeclarationPattern StructDeclaration(params MemberDeclarationPattern[] members)
        {
            return new StructDeclarationPattern(null, null, null, null, null, null, NodeList(members), null);
        }
        public static InterfaceDeclarationPattern InterfaceDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern baseList = null, TypeParameterListPattern typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, IEnumerable<MemberDeclarationPattern> members = null, Action<InterfaceDeclarationSyntax> action = null)
        {
            return new InterfaceDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, typeParameterList, NodeList(constraintClauses), NodeList(members), action);
        }

        public static InterfaceDeclarationPattern InterfaceDeclaration(params MemberDeclarationPattern[] members)
        {
            return new InterfaceDeclarationPattern(null, null, null, null, null, null, NodeList(members), null);
        }
        public static EnumDeclarationPattern EnumDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern baseList = null, IEnumerable<EnumMemberDeclarationPattern> members = null, Action<EnumDeclarationSyntax> action = null)
        {
            return new EnumDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, NodeList(members), action);
        }

        public static EnumDeclarationPattern EnumDeclaration(params EnumMemberDeclarationPattern[] members)
        {
            return new EnumDeclarationPattern(null, null, null, null, NodeList(members), null);
        }
        public static DelegateDeclarationPattern DelegateDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern returnType = null, string identifier = null, TypeParameterListPattern typeParameterList = null, ParameterListPattern parameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, Action<DelegateDeclarationSyntax> action = null)
        {
            return new DelegateDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), returnType, identifier, typeParameterList, parameterList, NodeList(constraintClauses), action);
        }
        public static EnumMemberDeclarationPattern EnumMemberDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, string identifier = null, EqualsValueClausePattern equalsValue = null, Action<EnumMemberDeclarationSyntax> action = null)
        {
            return new EnumMemberDeclarationPattern(NodeList(attributeLists), identifier, equalsValue, action);
        }
        public static BaseListPattern BaseList(IEnumerable<BaseTypePattern> types = null, Action<BaseListSyntax> action = null)
        {
            return new BaseListPattern(NodeList(types), action);
        }

        public static BaseListPattern BaseList(params BaseTypePattern[] types)
        {
            return new BaseListPattern(NodeList(types), null);
        }
        public static SimpleBaseTypePattern SimpleBaseType(TypePattern type = null, Action<SimpleBaseTypeSyntax> action = null)
        {
            return new SimpleBaseTypePattern(type, action);
        }
        public static TypeParameterConstraintClausePattern TypeParameterConstraintClause(IdentifierNamePattern name = null, IEnumerable<TypeParameterConstraintPattern> constraints = null, Action<TypeParameterConstraintClauseSyntax> action = null)
        {
            return new TypeParameterConstraintClausePattern(name, NodeList(constraints), action);
        }
        public static ConstructorConstraintPattern ConstructorConstraint(Action<ConstructorConstraintSyntax> action = null)
        {
            return new ConstructorConstraintPattern(action);
        }
        public static ClassOrStructConstraintPattern ClassOrStructConstraint(SyntaxKind kind = default(SyntaxKind), Action<ClassOrStructConstraintSyntax> action = null)
        {
            return new ClassOrStructConstraintPattern(kind, action);
        }
        public static TypeConstraintPattern TypeConstraint(TypePattern type = null, Action<TypeConstraintSyntax> action = null)
        {
            return new TypeConstraintPattern(type, action);
        }
        public static FieldDeclarationPattern FieldDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, VariableDeclarationPattern declaration = null, Action<FieldDeclarationSyntax> action = null)
        {
            return new FieldDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), declaration, action);
        }
        public static EventFieldDeclarationPattern EventFieldDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, VariableDeclarationPattern declaration = null, Action<EventFieldDeclarationSyntax> action = null)
        {
            return new EventFieldDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), declaration, action);
        }
        public static ExplicitInterfaceSpecifierPattern ExplicitInterfaceSpecifier(NamePattern name = null, Action<ExplicitInterfaceSpecifierSyntax> action = null)
        {
            return new ExplicitInterfaceSpecifierPattern(name, action);
        }
        public static MethodDeclarationPattern MethodDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, BlockPattern body = null, ArrowExpressionClausePattern expressionBody = null, TypePattern returnType = null, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier = null, string identifier = null, TypeParameterListPattern typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, Action<MethodDeclarationSyntax> action = null)
        {
            return new MethodDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), parameterList, body, expressionBody, returnType, explicitInterfaceSpecifier, identifier, typeParameterList, NodeList(constraintClauses), action);
        }
        public static OperatorDeclarationPattern OperatorDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, BlockPattern body = null, ArrowExpressionClausePattern expressionBody = null, TypePattern returnType = null, Action<OperatorDeclarationSyntax> action = null)
        {
            return new OperatorDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), parameterList, body, expressionBody, returnType, action);
        }
        public static ConversionOperatorDeclarationPattern ConversionOperatorDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, BlockPattern body = null, ArrowExpressionClausePattern expressionBody = null, TypePattern type = null, Action<ConversionOperatorDeclarationSyntax> action = null)
        {
            return new ConversionOperatorDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), parameterList, body, expressionBody, type, action);
        }
        public static ConstructorDeclarationPattern ConstructorDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, BlockPattern body = null, ArrowExpressionClausePattern expressionBody = null, string identifier = null, ConstructorInitializerPattern initializer = null, Action<ConstructorDeclarationSyntax> action = null)
        {
            return new ConstructorDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), parameterList, body, expressionBody, identifier, initializer, action);
        }
        public static ConstructorInitializerPattern ConstructorInitializer(SyntaxKind kind = default(SyntaxKind), ArgumentListPattern argumentList = null, Action<ConstructorInitializerSyntax> action = null)
        {
            return new ConstructorInitializerPattern(kind, argumentList, action);
        }
        public static DestructorDeclarationPattern DestructorDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, BlockPattern body = null, ArrowExpressionClausePattern expressionBody = null, string identifier = null, Action<DestructorDeclarationSyntax> action = null)
        {
            return new DestructorDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), parameterList, body, expressionBody, identifier, action);
        }
        public static PropertyDeclarationPattern PropertyDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier = null, AccessorListPattern accessorList = null, string identifier = null, ArrowExpressionClausePattern expressionBody = null, EqualsValueClausePattern initializer = null, Action<PropertyDeclarationSyntax> action = null)
        {
            return new PropertyDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), type, explicitInterfaceSpecifier, accessorList, identifier, expressionBody, initializer, action);
        }
        public static ArrowExpressionClausePattern ArrowExpressionClause(ExpressionPattern expression = null, Action<ArrowExpressionClauseSyntax> action = null)
        {
            return new ArrowExpressionClausePattern(expression, action);
        }
        public static EventDeclarationPattern EventDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier = null, AccessorListPattern accessorList = null, string identifier = null, Action<EventDeclarationSyntax> action = null)
        {
            return new EventDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), type, explicitInterfaceSpecifier, accessorList, identifier, action);
        }
        public static IndexerDeclarationPattern IndexerDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier = null, AccessorListPattern accessorList = null, BracketedParameterListPattern parameterList = null, ArrowExpressionClausePattern expressionBody = null, Action<IndexerDeclarationSyntax> action = null)
        {
            return new IndexerDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), type, explicitInterfaceSpecifier, accessorList, parameterList, expressionBody, action);
        }
        public static AccessorListPattern AccessorList(IEnumerable<AccessorDeclarationPattern> accessors = null, Action<AccessorListSyntax> action = null)
        {
            return new AccessorListPattern(NodeList(accessors), action);
        }

        public static AccessorListPattern AccessorList(params AccessorDeclarationPattern[] accessors)
        {
            return new AccessorListPattern(NodeList(accessors), null);
        }
        public static AccessorDeclarationPattern AccessorDeclaration(SyntaxKind kind = default(SyntaxKind), IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, BlockPattern body = null, ArrowExpressionClausePattern expressionBody = null, Action<AccessorDeclarationSyntax> action = null)
        {
            return new AccessorDeclarationPattern(kind, NodeList(attributeLists), TokenList(modifiers), body, expressionBody, action);
        }
        public static ParameterListPattern ParameterList(IEnumerable<ParameterPattern> parameters = null, Action<ParameterListSyntax> action = null)
        {
            return new ParameterListPattern(NodeList(parameters), action);
        }

        public static ParameterListPattern ParameterList(params ParameterPattern[] parameters)
        {
            return new ParameterListPattern(NodeList(parameters), null);
        }
        public static BracketedParameterListPattern BracketedParameterList(IEnumerable<ParameterPattern> parameters = null, Action<BracketedParameterListSyntax> action = null)
        {
            return new BracketedParameterListPattern(NodeList(parameters), action);
        }

        public static BracketedParameterListPattern BracketedParameterList(params ParameterPattern[] parameters)
        {
            return new BracketedParameterListPattern(NodeList(parameters), null);
        }
        public static ParameterPattern Parameter(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, string identifier = null, EqualsValueClausePattern @default = null, Action<ParameterSyntax> action = null)
        {
            return new ParameterPattern(NodeList(attributeLists), TokenList(modifiers), type, identifier, @default, action);
        }
        public static IncompleteMemberPattern IncompleteMember(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, Action<IncompleteMemberSyntax> action = null)
        {
            return new IncompleteMemberPattern(NodeList(attributeLists), TokenList(modifiers), type, action);
        }
    }
}
