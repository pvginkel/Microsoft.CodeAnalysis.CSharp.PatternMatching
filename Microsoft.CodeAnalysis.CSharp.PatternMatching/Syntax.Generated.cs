using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
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


            _action?.Invoke(typed);
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

            _left?.RunCallback(typed.Left, semanticModel);
            _right?.RunCallback(typed.Right, semanticModel);

            _action?.Invoke(typed);
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

            _typeArgumentList?.RunCallback(typed.TypeArgumentList, semanticModel);

            _action?.Invoke(typed);
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

            _arguments?.RunCallback(typed.Arguments, semanticModel);

            _action?.Invoke(typed);
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

            _alias?.RunCallback(typed.Alias, semanticModel);
            _name?.RunCallback(typed.Name, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _elementType?.RunCallback(typed.ElementType, semanticModel);
            _rankSpecifiers?.RunCallback(typed.RankSpecifiers, semanticModel);

            _action?.Invoke(typed);
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

            _sizes?.RunCallback(typed.Sizes, semanticModel);

            _action?.Invoke(typed);
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

            _elementType?.RunCallback(typed.ElementType, semanticModel);

            _action?.Invoke(typed);
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

            _elementType?.RunCallback(typed.ElementType, semanticModel);

            _action?.Invoke(typed);
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

            _elements?.RunCallback(typed.Elements, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _arguments?.RunCallback(typed.Arguments, semanticModel);

            _action?.Invoke(typed);
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

            _operand?.RunCallback(typed.Operand, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _operand?.RunCallback(typed.Operand, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);
            _name?.RunCallback(typed.Name, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);
            _whenNotNull?.RunCallback(typed.WhenNotNull, semanticModel);

            _action?.Invoke(typed);
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

            _name?.RunCallback(typed.Name, semanticModel);

            _action?.Invoke(typed);
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

            _argumentList?.RunCallback(typed.ArgumentList, semanticModel);

            _action?.Invoke(typed);
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

            _argumentList?.RunCallback(typed.ArgumentList, semanticModel);

            _action?.Invoke(typed);
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

            _left?.RunCallback(typed.Left, semanticModel);
            _right?.RunCallback(typed.Right, semanticModel);

            _action?.Invoke(typed);
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

            _left?.RunCallback(typed.Left, semanticModel);
            _right?.RunCallback(typed.Right, semanticModel);

            _action?.Invoke(typed);
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

            _condition?.RunCallback(typed.Condition, semanticModel);
            _whenTrue?.RunCallback(typed.WhenTrue, semanticModel);
            _whenFalse?.RunCallback(typed.WhenFalse, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);
            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);
            _argumentList?.RunCallback(typed.ArgumentList, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);
            _argumentList?.RunCallback(typed.ArgumentList, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (ArgumentListSyntax)node;


            _action?.Invoke(typed);
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
            var typed = (BracketedArgumentListSyntax)node;


            _action?.Invoke(typed);
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

            _nameColon?.RunCallback(typed.NameColon, semanticModel);
            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _name?.RunCallback(typed.Name, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);
            _designation?.RunCallback(typed.Designation, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);
            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (AnonymousMethodExpressionSyntax)node;

            _parameterList?.RunCallback(typed.ParameterList, semanticModel);

            _action?.Invoke(typed);
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

            _parameter?.RunCallback(typed.Parameter, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _parameterList?.RunCallback(typed.ParameterList, semanticModel);

            _action?.Invoke(typed);
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

            _expressions?.RunCallback(typed.Expressions, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);
            _argumentList?.RunCallback(typed.ArgumentList, semanticModel);
            _initializer?.RunCallback(typed.Initializer, semanticModel);

            _action?.Invoke(typed);
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

            _nameEquals?.RunCallback(typed.NameEquals, semanticModel);
            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _initializers?.RunCallback(typed.Initializers, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);
            _initializer?.RunCallback(typed.Initializer, semanticModel);

            _action?.Invoke(typed);
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

            _initializer?.RunCallback(typed.Initializer, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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

            _fromClause?.RunCallback(typed.FromClause, semanticModel);
            _body?.RunCallback(typed.Body, semanticModel);

            _action?.Invoke(typed);
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

            _clauses?.RunCallback(typed.Clauses, semanticModel);
            _selectOrGroup?.RunCallback(typed.SelectOrGroup, semanticModel);
            _continuation?.RunCallback(typed.Continuation, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);
            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);
            _inExpression?.RunCallback(typed.InExpression, semanticModel);
            _leftExpression?.RunCallback(typed.LeftExpression, semanticModel);
            _rightExpression?.RunCallback(typed.RightExpression, semanticModel);
            _into?.RunCallback(typed.Into, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _condition?.RunCallback(typed.Condition, semanticModel);

            _action?.Invoke(typed);
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

            _orderings?.RunCallback(typed.Orderings, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _groupExpression?.RunCallback(typed.GroupExpression, semanticModel);
            _byExpression?.RunCallback(typed.ByExpression, semanticModel);

            _action?.Invoke(typed);
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

            _body?.RunCallback(typed.Body, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _contents?.RunCallback(typed.Contents, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);
            _pattern?.RunCallback(typed.Pattern, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _condition?.RunCallback(typed.Condition, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);
            _designation?.RunCallback(typed.Designation, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);
            _alignmentClause?.RunCallback(typed.AlignmentClause, semanticModel);
            _formatClause?.RunCallback(typed.FormatClause, semanticModel);

            _action?.Invoke(typed);
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

            _value?.RunCallback(typed.Value, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _statement?.RunCallback(typed.Statement, semanticModel);

            _action?.Invoke(typed);
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

            _statements?.RunCallback(typed.Statements, semanticModel);

            _action?.Invoke(typed);
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

            _returnType?.RunCallback(typed.ReturnType, semanticModel);
            _typeParameterList?.RunCallback(typed.TypeParameterList, semanticModel);
            _parameterList?.RunCallback(typed.ParameterList, semanticModel);
            _constraintClauses?.RunCallback(typed.ConstraintClauses, semanticModel);
            _body?.RunCallback(typed.Body, semanticModel);
            _expressionBody?.RunCallback(typed.ExpressionBody, semanticModel);

            _action?.Invoke(typed);
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

            _declaration?.RunCallback(typed.Declaration, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);
            _variables?.RunCallback(typed.Variables, semanticModel);

            _action?.Invoke(typed);
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

            _argumentList?.RunCallback(typed.ArgumentList, semanticModel);
            _initializer?.RunCallback(typed.Initializer, semanticModel);

            _action?.Invoke(typed);
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

            _value?.RunCallback(typed.Value, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _variables?.RunCallback(typed.Variables, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _statement?.RunCallback(typed.Statement, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _condition?.RunCallback(typed.Condition, semanticModel);
            _statement?.RunCallback(typed.Statement, semanticModel);

            _action?.Invoke(typed);
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

            _statement?.RunCallback(typed.Statement, semanticModel);
            _condition?.RunCallback(typed.Condition, semanticModel);

            _action?.Invoke(typed);
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

            _declaration?.RunCallback(typed.Declaration, semanticModel);
            _initializers?.RunCallback(typed.Initializers, semanticModel);
            _condition?.RunCallback(typed.Condition, semanticModel);
            _incrementors?.RunCallback(typed.Incrementors, semanticModel);
            _statement?.RunCallback(typed.Statement, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (ForEachStatementSyntax)node;

            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (ForEachVariableStatementSyntax)node;

            _variable?.RunCallback(typed.Variable, semanticModel);

            _action?.Invoke(typed);
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

            _declaration?.RunCallback(typed.Declaration, semanticModel);
            _expression?.RunCallback(typed.Expression, semanticModel);
            _statement?.RunCallback(typed.Statement, semanticModel);

            _action?.Invoke(typed);
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

            _declaration?.RunCallback(typed.Declaration, semanticModel);
            _statement?.RunCallback(typed.Statement, semanticModel);

            _action?.Invoke(typed);
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

            _block?.RunCallback(typed.Block, semanticModel);

            _action?.Invoke(typed);
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

            _block?.RunCallback(typed.Block, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);
            _statement?.RunCallback(typed.Statement, semanticModel);

            _action?.Invoke(typed);
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

            _condition?.RunCallback(typed.Condition, semanticModel);
            _statement?.RunCallback(typed.Statement, semanticModel);
            _else?.RunCallback(typed.Else, semanticModel);

            _action?.Invoke(typed);
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

            _statement?.RunCallback(typed.Statement, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);
            _sections?.RunCallback(typed.Sections, semanticModel);

            _action?.Invoke(typed);
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

            _labels?.RunCallback(typed.Labels, semanticModel);
            _statements?.RunCallback(typed.Statements, semanticModel);

            _action?.Invoke(typed);
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

            _pattern?.RunCallback(typed.Pattern, semanticModel);
            _whenClause?.RunCallback(typed.WhenClause, semanticModel);

            _action?.Invoke(typed);
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

            _value?.RunCallback(typed.Value, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _block?.RunCallback(typed.Block, semanticModel);
            _catches?.RunCallback(typed.Catches, semanticModel);
            _finally?.RunCallback(typed.Finally, semanticModel);

            _action?.Invoke(typed);
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

            _declaration?.RunCallback(typed.Declaration, semanticModel);
            _filter?.RunCallback(typed.Filter, semanticModel);
            _block?.RunCallback(typed.Block, semanticModel);

            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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

            _filterExpression?.RunCallback(typed.FilterExpression, semanticModel);

            _action?.Invoke(typed);
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

            _block?.RunCallback(typed.Block, semanticModel);

            _action?.Invoke(typed);
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

            _externs?.RunCallback(typed.Externs, semanticModel);
            _usings?.RunCallback(typed.Usings, semanticModel);
            _attributeLists?.RunCallback(typed.AttributeLists, semanticModel);
            _members?.RunCallback(typed.Members, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _alias?.RunCallback(typed.Alias, semanticModel);
            _name?.RunCallback(typed.Name, semanticModel);

            _action?.Invoke(typed);
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

            _name?.RunCallback(typed.Name, semanticModel);
            _externs?.RunCallback(typed.Externs, semanticModel);
            _usings?.RunCallback(typed.Usings, semanticModel);
            _members?.RunCallback(typed.Members, semanticModel);

            _action?.Invoke(typed);
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

            _target?.RunCallback(typed.Target, semanticModel);
            _attributes?.RunCallback(typed.Attributes, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _name?.RunCallback(typed.Name, semanticModel);
            _argumentList?.RunCallback(typed.ArgumentList, semanticModel);

            _action?.Invoke(typed);
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

            _arguments?.RunCallback(typed.Arguments, semanticModel);

            _action?.Invoke(typed);
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

            _nameEquals?.RunCallback(typed.NameEquals, semanticModel);
            _nameColon?.RunCallback(typed.NameColon, semanticModel);
            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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

            _name?.RunCallback(typed.Name, semanticModel);

            _action?.Invoke(typed);
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

            _parameters?.RunCallback(typed.Parameters, semanticModel);

            _action?.Invoke(typed);
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

            _attributeLists?.RunCallback(typed.AttributeLists, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (ClassDeclarationSyntax)node;


            _action?.Invoke(typed);
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
            var typed = (StructDeclarationSyntax)node;


            _action?.Invoke(typed);
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
            var typed = (InterfaceDeclarationSyntax)node;


            _action?.Invoke(typed);
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
            var typed = (EnumDeclarationSyntax)node;

            _members?.RunCallback(typed.Members, semanticModel);

            _action?.Invoke(typed);
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

            _attributeLists?.RunCallback(typed.AttributeLists, semanticModel);
            _returnType?.RunCallback(typed.ReturnType, semanticModel);
            _typeParameterList?.RunCallback(typed.TypeParameterList, semanticModel);
            _parameterList?.RunCallback(typed.ParameterList, semanticModel);
            _constraintClauses?.RunCallback(typed.ConstraintClauses, semanticModel);

            _action?.Invoke(typed);
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

            _attributeLists?.RunCallback(typed.AttributeLists, semanticModel);
            _equalsValue?.RunCallback(typed.EqualsValue, semanticModel);

            _action?.Invoke(typed);
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

            _types?.RunCallback(typed.Types, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (SimpleBaseTypeSyntax)node;


            _action?.Invoke(typed);
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

            _name?.RunCallback(typed.Name, semanticModel);
            _constraints?.RunCallback(typed.Constraints, semanticModel);

            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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


            _action?.Invoke(typed);
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

            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (FieldDeclarationSyntax)node;


            _action?.Invoke(typed);
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
            var typed = (EventFieldDeclarationSyntax)node;


            _action?.Invoke(typed);
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

            _name?.RunCallback(typed.Name, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (MethodDeclarationSyntax)node;

            _returnType?.RunCallback(typed.ReturnType, semanticModel);
            _explicitInterfaceSpecifier?.RunCallback(typed.ExplicitInterfaceSpecifier, semanticModel);
            _typeParameterList?.RunCallback(typed.TypeParameterList, semanticModel);
            _constraintClauses?.RunCallback(typed.ConstraintClauses, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (OperatorDeclarationSyntax)node;

            _returnType?.RunCallback(typed.ReturnType, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (ConversionOperatorDeclarationSyntax)node;

            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (ConstructorDeclarationSyntax)node;

            _initializer?.RunCallback(typed.Initializer, semanticModel);

            _action?.Invoke(typed);
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

            _argumentList?.RunCallback(typed.ArgumentList, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (DestructorDeclarationSyntax)node;


            _action?.Invoke(typed);
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
            var typed = (PropertyDeclarationSyntax)node;

            _expressionBody?.RunCallback(typed.ExpressionBody, semanticModel);
            _initializer?.RunCallback(typed.Initializer, semanticModel);

            _action?.Invoke(typed);
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

            _expression?.RunCallback(typed.Expression, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (EventDeclarationSyntax)node;


            _action?.Invoke(typed);
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
            var typed = (IndexerDeclarationSyntax)node;

            _parameterList?.RunCallback(typed.ParameterList, semanticModel);
            _expressionBody?.RunCallback(typed.ExpressionBody, semanticModel);

            _action?.Invoke(typed);
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

            _accessors?.RunCallback(typed.Accessors, semanticModel);

            _action?.Invoke(typed);
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

            _attributeLists?.RunCallback(typed.AttributeLists, semanticModel);
            _body?.RunCallback(typed.Body, semanticModel);
            _expressionBody?.RunCallback(typed.ExpressionBody, semanticModel);

            _action?.Invoke(typed);
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
            var typed = (ParameterListSyntax)node;


            _action?.Invoke(typed);
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
            var typed = (BracketedParameterListSyntax)node;


            _action?.Invoke(typed);
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

            _attributeLists?.RunCallback(typed.AttributeLists, semanticModel);
            _type?.RunCallback(typed.Type, semanticModel);
            _default?.RunCallback(typed.Default, semanticModel);

            _action?.Invoke(typed);
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

            _attributeLists?.RunCallback(typed.AttributeLists, semanticModel);
            _type?.RunCallback(typed.Type, semanticModel);

            _action?.Invoke(typed);
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
