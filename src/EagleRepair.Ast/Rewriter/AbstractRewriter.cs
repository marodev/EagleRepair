using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EagleRepair.Ast.Rewriter
{
    public abstract class AbstractRewriter : CSharpSyntaxRewriter
    {
        protected readonly IChangeTracker ChangeTracker;
        protected readonly IRewriteService RewriteService;
        protected readonly ITypeService TypeService;
        protected readonly IDisplayService DisplayService;

        protected AbstractRewriter(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService)
        {
            ChangeTracker = changeTracker;
            TypeService = typeService;
            RewriteService = rewriteService;
            DisplayService = displayService;
        }

        public Workspace Workspace { get; set; }

        public SemanticModel SemanticModel { get; set; }
        
        public string ProjectName { get; set; }
        
        public string FilePath { get; set; }
    }
}
