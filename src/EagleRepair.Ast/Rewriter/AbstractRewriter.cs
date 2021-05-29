using EagleRepair.Ast.Priority;
using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EagleRepair.Ast.Rewriter
{
    public abstract class AbstractRewriter : CSharpSyntaxRewriter
    {
        protected readonly IChangeTracker ChangeTracker;
        protected readonly IDisplayService DisplayService;
        protected readonly IRewriteService RewriteService;
        protected readonly ITypeService TypeService;

        protected AbstractRewriter(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService)
        {
            ChangeTracker = changeTracker;
            TypeService = typeService;
            RewriteService = rewriteService;
            DisplayService = displayService;
            Priority = RewriterPriority.DEFAULT;
        }

        public Workspace Workspace { get; set; }

        public SemanticModel SemanticModel { get; set; }

        public string ProjectName { get; set; }

        public string FilePath { get; set; }

        public RewriterPriority Priority { get; set; }
    }
}
