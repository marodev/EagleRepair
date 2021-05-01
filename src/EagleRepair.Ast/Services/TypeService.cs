namespace EagleRepair.Ast.Services
{
    public class TypeService : ITypeService
    {
        public bool InheritsFromIEnumerable(string typeSymbol)
        {
            return typeSymbol.StartsWith("System.Collections.");
        }

        public bool IsBuiltInType(string containingNamespace)
        {
            return containingNamespace is not null && containingNamespace.StartsWith("System");
        }
    }
}
