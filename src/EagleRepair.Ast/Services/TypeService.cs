namespace EagleRepair.Ast.Services
{
    public class TypeService : ITypeService
    {
        public bool InheritsFromIEnumerable(string typeSymbol)
        {
            return typeSymbol.StartsWith("System.Collections.");
        }
    }
}
