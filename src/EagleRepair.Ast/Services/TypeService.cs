namespace EagleRepair.Ast.Services
{
    public class TypeService : ITypeService
    {
        public bool InheritsFromIEnumerable(string typeSymbol)
        {
            return typeSymbol is not null && typeSymbol.StartsWith("System.Collections.");
        }

        public bool IsIEnumerable(string typeSymbol)
        {
            return typeSymbol is not null && typeSymbol.Contains("System.Collections.Generic.IEnumerable");
        }

        public bool IsBuiltInType(string containingNamespace)
        {
            return containingNamespace is not null && containingNamespace.StartsWith("System");
        }
    }
}
