using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EagleRepair.Ast.RewriteCommand
{
    public abstract class AbstractRewriteCommand : CSharpSyntaxRewriter
    {
        protected readonly IChangeTracker _changeTracker;
        protected readonly ITypeService _typeService;
        protected SemanticModel _semanticModel;

        protected AbstractRewriteCommand(IChangeTracker changeTracker, ITypeService typeService)
        {
            _changeTracker = changeTracker;
            _typeService = typeService;
        }

        public void SemanticModel(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }
    }
}
