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

            // switch (type)
            // {
            //     case "bool":
            //     case "byte":
            //     case "sbyte":
            //     case "char":
            //     case "decimal":
            //     case "double":
            //     case "float":
            //     case "int":
            //     case "uint":
            //     case "nint":
            //     case "long":
            //     case "ulong":
            //     case "short":
            //     case "ushort":
            //         
            //         return true;
            // }
            //
            
            throw new System.NotImplementedException();
        }
    }
}
