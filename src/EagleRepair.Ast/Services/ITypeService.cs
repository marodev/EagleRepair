namespace EagleRepair.Ast.Services
{
    public interface ITypeService
    {
        bool InheritsFromIEnumerable(string typeSymbol);

        bool IsIEnumerable(string typeSymbol);

        bool IsBuiltInType(string containingNamespace);
    }
}
