# Microsoft.CodeAnalysis.CSharp.PatternMatching

I've been working with Roslyn for the past few weeks to do automated refactoring on a large codebase. This involves a lot of pattern matching on syntax trees. The code you need to write to do this is generally pretty horrible. This project is an attempt to fix this.

## Disclaimer

This project is a proof of concept. I'm not planning on releasing this as a NuGet package or to actively maintain this. That being said, I do welcome improvements and PR's if they add value to the project.

## Tutorial

This project is a way to do pattern matching on syntax trees. The idea is that you build up a pattern and run that against a `SyntaxNode` or a tree of nodes.

Let's use the following simple application as an example:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello world!");
    }
}
```

Let's say we want to get the string out of all `Console.WriteLine`'s out of this project. Using just Roslyn, the code would look something like this:

```csharp
var syntaxTree = GetSimpleSyntaxTree();
var methodDeclaration = syntaxTree.GetRoot()
    .DescendantNodes()
    .OfType<MethodDeclarationSyntax>()
    .Single();

if (
    methodDeclaration.Body is BlockSyntax block &&
    block.Statements[0] is ExpressionStatementSyntax expressionStatement &&
    expressionStatement.Expression is InvocationExpressionSyntax invocation &&
    invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
    memberAccess.Name is IdentifierNameSyntax name &&
    name.Identifier.Text == "WriteLine" &&
    memberAccess.Expression is IdentifierNameSyntax expression &&
    expression.Identifier.Text == "Console" &&
    invocation.ArgumentList.Arguments.Count == 1 &&
    invocation.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax literalExpression
)
    Assert.AreEqual("Hello world!", literalExpression.Token.ValueText);
```

What this does is in a single `if` statement, match the body of the method declaration for exactly the criteria we set.

This library attempts to accomplish the same goal, but instead of having to write horrible code like this, write it using pattern matching.

The core of the library is the `Pattern` factory class. This is similar to the `SyntaxFactory` class of Roslyn and has most of the same factory methods as that that class has. However, instead of building up `SyntaxNode`'s, it builds up `PatternNode`'s.

A `PatternNode` is a node to match a `SyntaxNode`. The factory methods have different methods for different types of `SyntaxNode`'s. The parameters to these methods mirror the properties of the `SyntaxNode`. These properties are (almost) always optional. This means you can provide patterns for what you want to filter on, and can ignore the rest.

The following example shows doing the same thing as above using pattern matching instead:

```csharp
var syntaxTree = GetSimpleSyntaxTree();
var methodDeclaration = syntaxTree.GetRoot()
    .DescendantNodes()
    .OfType<MethodDeclarationSyntax>()
    .Single();

string expression = null;

if (
    P.SingleStatement(
        P.ExpressionStatement(
            P.InvocationExpression(
                P.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    P.IdentifierName("WriteLine"),
                    P.IdentifierName("Console")
                ),
                P.ArgumentList(
                    P.Argument(
                        expression: P.LiteralExpression(
                            action: p => expression = p.Token.ValueText
                        )
                    )
                )
            )
        )
    ).IsMatch(methodDeclaration.Body)
)
    Assert.AreEqual("Hello world!", expression);
```

In the above code the pattern factory methods are called on `P`. This example is taken from the unit tests part of the project, which have the `Pattern` factory class imported with the alias `P`. Generally it's not ideal to do a static import on the `Pattern` class since the names are the same as the `SyntaxFactory` factory methods. An easy way to keep the clutter down is to import the `Pattern` class using this alias as follows:

```csharp
using P = Microsoft.CodeAnalysis.CSharp.PatternMatching.Pattern;
```

This code does exactly the same as the original code snippet. The difference is that it does it by first building up a pattern, and then running that pattern against a `SyntaxNode`; the `Body` of the method declaration in this example.

For example, the call to `P.InvocationExpression()` creates a pattern node for a `InvocationExpressionSyntax`, a `InvocationExpressionPattern`. This pattern takes an expression and an argument list, which we provide as new pattern nodes.

Once you've built up the tree, you can call `IsMatch` on the root node to see whether the `SyntaxNode` tree matches the `PatternNode` tree. Only if all patterns you provided match exactly, will it return true.

One other thing that's going on is the lambda expression in the `P.Argument()` call. Every pattern takes an optional `Action<T>` callback, where `T` is the type of `SyntaxNode` being matched. You can use this to, in very specific places, have callbacks run. In the above example, this is used to get the text argument to the `Console.WriteLine` call. Note that callbacks will only be run if the whole tree matches. This is done by implementing `IsMatch` in two phases: first it walks the tree to see whether it's a match and only when the complete tree matches, it's walked again to run any callbacks.

The `IsMatch` method takes a second, optional, `SemanticModel` parameter. This is necessary for patterns that work with symbols. There is a pattern `AnySymbolPattern` that checks whether an expression resolves to a symbol and gives that to the callback. A second pattern is the `SymbolPattern` which takes a symbol and checks whether the expression matches that symbol. If you have any of these patterns in your tree, you need to provide the `SemanticModel` argument to the `IsMatch` method.

### Matching groups of nodes

There are a few specialized patterns that match a group of nodes:

* `AnyExpressionPattern` will match any node that inherits from `ExpressionSyntax`;
* `AnyLambdaExpressionPattern` will match both a `ParenthesizedLambdaExpressionSyntax` and a `SimpleLambdaExpressionSyntax`;
* `AnyStatementPattern` will match any node that inherits from `StatementSyntax`;
* `AnySymbolPattern` will match any expression that resolves into a symbol and passes the symbol into the callback;
* `AnyTypePattern` will match any node that inherits from `TypeSyntax`;
* `NullExpressionPattern` will match expressions that are `null`;
* `NullStatementPattern` will match statements that are `null`;
* `SingleStatementPattern` will match nodes that are either a `StatementSyntax` or a `BlockSyntax` with a single statement;
* `VarTypePattern` will match any `TypeSyntax` that denotes the `var` type.

### Matching multiple nodes

The `SyntaxNode` class has a number of methods that return multiple nodes, e.g. `Ancestors()` and `DescendantNodes()`. These methods also exist for the `PatternNode` class, prefixed with `Match`, e.g. `MatchDescendantNodes()`.

Working with the following sample code:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.Write("Hello ");
        Console.WriteLine("world!");
    }
}
```

The following code finds all invocations with a single string argument.

```csharp
var syntaxTree = GetSimpleSyntaxTree();
var methodDeclaration = syntaxTree.GetRoot()
    .DescendantNodes()
    .OfType<MethodDeclarationSyntax>()
    .Single();

var strings = new List<string>();

int matches =
    P.InvocationExpression(
        P.MemberAccessExpression(),
        P.ArgumentList(
            P.Argument(
                expression: P.LiteralExpression(
                    action: p => strings.Add(p.Token.ValueText)
                )
            )
        )
    )
    .MatchDescendantNodes(methodDeclaration.Body)
    .Count();

Assert.AreEqual(2, matches);
Assert.AreEqual(new List<string> { "Hello ", "world!" }, strings);
```

The result of this is an `IEnumerable<SyntaxNode>` with all nodes being matched. For every matched node, the pattern is run and the callbacks are executed. In the example above we use this to build up a list to get all string arguments out of the invocations.

The `Match` methods eagerly execute the pattern against the list of syntax nodes. Alternative methods exist which start with `Enumerate` that are implemented as a generator and will lazily match against the tree. This can be used to e.g. find the first match in a tree by calling e.g. `.EnumerateAncestors(node).First()`. Internally the `Match` methods are implemented by calling the `Enumerate` methods and consuming them by calling `ToList()` on the enumerable.

### Reusing patterns

Obviously using patterns to match against code is a lot slower than doing it by hand as in the first example. The biggest issue being that the `PatternNode` first needs to be allocated before the match can be done. The example above builds the pattern every time it's used.

Ideally you'd want to store patterns in a static variable and reuse them. The way the patterns are built, this is an option. However, this does make using the callbacks more difficult since this requires a closure and you'd have to use static fields to store data to. I'm not yet sure what the best approach is and I may make changes to this library in the future to aid in reusing pattern trees.

If you're building patterns that are matched against a large number of nodes, this is something to be aware of. In that case, either you should not use the callbacks, or write the callbacks in a way that the patterns can be reused.

## Using the project

This project does not have a NuGet package. Easiest way to use this is to either (fork it and) adding the project as a submodule. Alternatively you can also just download the ZIP and dump it into your tree. It's not that big of a project.

## Development

The core of this project is a set of pattern matching classes automatically generated through a T4 script from Roslyn CSharp [Syntax.xml](https://github.com/dotnet/roslyn/blob/master/src/Compilers/CSharp/Portable/Syntax/Syntax.xml) file. Upgrading to the latest version of Roslyn comes down to overwriting the Syntax.xml copy in the project, re-running the T4 script and fixing any compilation issues.

## License

LGPL 3 License.
