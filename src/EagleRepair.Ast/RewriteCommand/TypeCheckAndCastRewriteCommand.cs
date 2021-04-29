using EagleRepair.Ast.Services;
using EagleRepair.Monitor;

namespace EagleRepair.Ast.RewriteCommand
{
    public class TypeCheckAndCastRewriteCommand : AbstractRewriteCommand
    {
        public TypeCheckAndCastRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) : base(changeTracker, typeService)
        {
        }
        
        
    }
}
