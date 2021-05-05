using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EagleRepair.Ast.Extensions;
using EagleRepair.Ast.Services;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace EagleRepair.Ast.RewriteCommand
{
    public class DisposePatternRewriteCommand : AbstractRewriteCommand
    {
        public DisposePatternRewriteCommand(IChangeTracker changeTracker, ITypeService typeService) : base(changeTracker, typeService)
        {
        }

        public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var allClasses = GetAllClasses(node);
            var classesThatImplementIDisposable = GetClassesWithIDisposable(allClasses);
            var nonSealedClassesThatImplementIDisposable = GetNonSealedClasses(classesThatImplementIDisposable);

            var classesToBeSealed = new List<ClassDeclarationSyntax>();
            var classesToImplementDisposePattern = new List<ClassDeclarationSyntax>();

            foreach (var nonSealedClass in nonSealedClassesThatImplementIDisposable)
            {
                if (CanBeSealed(allClasses, nonSealedClass))
                {
                    classesToBeSealed.Add(nonSealedClass);
                }
                else
                {
                    classesToImplementDisposePattern.Add(nonSealedClass);
                }
            }

            var sealedChanges = AddSealedModifier(classesToBeSealed);
            var disposePatternImpl = FixDisposePattern(classesToImplementDisposePattern);

            var changes = new Dictionary<ClassDeclarationSyntax, ClassDeclarationSyntax>();
            changes = changes.Merge(sealedChanges);
            changes = changes.Merge(disposePatternImpl);
            
            if (!changes.Any())
            {
                return base.VisitCompilationUnit(node);
            }

            var newCompilation = node.ReplaceNodes(changes.Keys.AsEnumerable(),
            (n1, n2) => changes[n1]);
            
            // reformat code
            newCompilation =  (CompilationUnitSyntax) Formatter.Format(newCompilation, Workspace);
            return newCompilation;
        }

        private static IDictionary<ClassDeclarationSyntax, ClassDeclarationSyntax> AddSealedModifier(
            IEnumerable<ClassDeclarationSyntax> nonSealedClasses)
        {
            var sealedClassPairs = new Dictionary<ClassDeclarationSyntax, ClassDeclarationSyntax>();
            foreach (var nonSealedClass in nonSealedClasses)
            {
                var sealedClass = AddSealedModifier(nonSealedClass);
                sealedClassPairs.Add(nonSealedClass, (ClassDeclarationSyntax) sealedClass);
            }

            return sealedClassPairs;
        }

        private bool CanBeSealed(IEnumerable<ClassDeclarationSyntax> allClasses, 
            ClassDeclarationSyntax iDisposableClass)
        {
            var iDisposableClassName = iDisposableClass.Identifier.ValueText;
            var inheritedClasses = allClasses.Where(
                c => c.BaseList is not null &&
                     c.BaseList.Types.Any(t => t.ToString().Equals(iDisposableClassName))).ToImmutableList();

            if (!inheritedClasses.Any())
            {
                return true;
            }

            var iDisposableNamespaceClassName = SemanticModel.GetDeclaredSymbol(iDisposableClass)?.ToString();
            foreach (var inheritedClass in inheritedClasses)
            {
                var types = inheritedClass.BaseList?.Types;

                if (types is null)
                {
                    continue;
                }
                
                foreach (var type in types)
                {
                    var namespaceType = SemanticModel.GetTypeInfo(type.Type).Type?.ToString();

                    if (namespaceType is null)
                    {
                        continue;
                    }

                    if (namespaceType.Equals(iDisposableNamespaceClassName))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static Dictionary<ClassDeclarationSyntax, ClassDeclarationSyntax> FixDisposePattern(
            IEnumerable<ClassDeclarationSyntax> nonSealedClassesThatImplementIDisposable)
        {
            var nodesToUpdate = new Dictionary<ClassDeclarationSyntax, ClassDeclarationSyntax>();
            foreach (var classDecl in nonSealedClassesThatImplementIDisposable)
            {
                var disposeMethods = classDecl.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Where(m => m.Identifier.ToString().Equals("Dispose")).ToList();

                if (disposeMethods.Count == 2)
                {
                    // probably implements the dispose pattern correctly
                    continue;
                }

                var newNode = InjectUtils.ModifyDisposeAndAddProtectedDispose(classDecl, disposeMethods.FirstOrDefault());
                nodesToUpdate.Add(classDecl, newNode);

            }

            return nodesToUpdate;
        }

        private static ImmutableList<ClassDeclarationSyntax> GetNonSealedClasses(IEnumerable<ClassDeclarationSyntax> classes)
        {
            return classes
                .Where(c => c.Modifiers.Any(m => !m.ToString().Equals("Sealed")))
                .ToImmutableList();
        }

        private static ImmutableList<ClassDeclarationSyntax> GetClassesWithIDisposable(IEnumerable<ClassDeclarationSyntax> classes)
        {
            return classes
                .Where(c => c.BaseList is not null &&
                            c.BaseList.Types.Any(t => t.ToString().Equals("IDisposable")))
                .ToImmutableList();
        }

        private static ImmutableList<ClassDeclarationSyntax> GetAllClasses(SyntaxNode node)
        {
            return node.DescendantNodes().OfType<ClassDeclarationSyntax>().ToImmutableList();
        }

        private static SyntaxNode AddSealedModifier(ClassDeclarationSyntax classDecl)
        {
            var newNode = InjectUtils.AddSealedKeyword(classDecl);
            return newNode;
        }
    }
}
