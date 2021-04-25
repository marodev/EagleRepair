using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Monitor;

namespace Ast.RewriteCommand
{
    public abstract class AbstractRewriteCommand : CSharpSyntaxRewriter
    {
        protected readonly IChangeTracker _changeTracker;
        protected AbstractRewriteCommand(IChangeTracker changeTracker)
        {
            _changeTracker = changeTracker;
        }
    }
}