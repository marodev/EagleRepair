using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EagleRepair.Ast.RewriteCommand
{
    public abstract class AbstractRewriteCommand : CSharpSyntaxRewriter
    {
        protected readonly IChangeTracker _changeTracker;
        protected readonly IRewriteService _rewriteService;
        protected readonly ITypeService _typeService;

        protected AbstractRewriteCommand(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService)
        {
            _changeTracker = changeTracker;
            _typeService = typeService;
            _rewriteService = rewriteService;
        }

        public Workspace Workspace { get; set; }

        public SemanticModel SemanticModel { get; set; }
    }
}
