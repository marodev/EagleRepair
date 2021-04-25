using EagleRepair.Monitor;
using Microsoft.CodeAnalysis.CSharp;

namespace EagleRepair.Ast.RewriteCommand
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