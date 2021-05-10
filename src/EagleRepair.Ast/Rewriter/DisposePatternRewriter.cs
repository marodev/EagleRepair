using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EagleRepair.Ast.Extensions;
using EagleRepair.Ast.Services;
using EagleRepair.Ast.Url;
using EagleRepair.Monitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;

namespace EagleRepair.Ast.Rewriter
{
    public class DisposePatternRewriter : AbstractRewriter
    {
        public DisposePatternRewriter(IChangeTracker changeTracker, ITypeService typeService,
            IRewriteService rewriteService, IDisplayService displayService) : base(
            changeTracker, typeService, rewriteService, displayService)
        {
        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var allClasses = GetAllClasses(node);
            var classesThatImplementIDisposable = GetClassesWithIDisposable(allClasses);
            var nonSealedClassesThatImplementIDisposable = GetNonSealedClasses(classesThatImplementIDisposable);

            var classesToBeSealed = new List<ClassDeclarationSyntax>();
            var classesToImplementDisposePattern = new List<ClassDeclarationSyntax>();

            foreach (var nonSealedClass in nonSealedClassesThatImplementIDisposable)
            {
                if (CanBeSealed(nonSealedClass).Result)
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

            foreach (var change in changes)
            {
                var nodeToMonitor = change.Key;
                var lineNumber = $"{DisplayService.GetLineNumber(nodeToMonitor)}";
                var message = SonarQube.RuleSpecification3881Message;
                ChangeTracker.Stage(new Message
                {
                    Line = lineNumber, Path = FilePath, Project = ProjectName, Text = message
                });
            }

            // reformat code
            newCompilation = (CompilationUnitSyntax)Formatter.Format(newCompilation, Workspace);
            return newCompilation;
        }

        private IDictionary<ClassDeclarationSyntax, ClassDeclarationSyntax> AddSealedModifier(
            IEnumerable<ClassDeclarationSyntax> nonSealedClasses)
        {
            var sealedClassPairs = new Dictionary<ClassDeclarationSyntax, ClassDeclarationSyntax>();
            foreach (var nonSealedClass in nonSealedClasses)
            {
                var sealedClass = AddSealedModifier(nonSealedClass);
                sealedClassPairs.Add(nonSealedClass, (ClassDeclarationSyntax)sealedClass);
            }

            return sealedClassPairs;
        }

        private async Task<bool> CanBeSealed(BaseTypeDeclarationSyntax iDisposableClass)
        {
            var isAbstractOrPartial = iDisposableClass.Modifiers.Any(m =>
            {
                var modifier = m.ToString();
                return modifier.Equals("abstract") || modifier.Equals("partial");
            });

            if (isAbstractOrPartial)
            {
                // abstract class or partial class can't be sealed.
                return false;
            }

            var hasVirtualMethods = iDisposableClass.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Any(methodDecl => methodDecl.Modifiers.Any(modifier => modifier.ToString().Equals("virtual")));

            if (hasVirtualMethods)
            {
                // classes that have virtual methods can't be sealed.
                return false;
            }

            var symbol = SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)iDisposableClass);
            if (symbol is not INamedTypeSymbol iNamedTypeSymbol)
            {
                return false;
            }

            // find all derived classes. Note: this operation is expensive but Roslyn uses a cache internally
            var derivedClasses = await SymbolFinder
                .FindDerivedClassesAsync(iNamedTypeSymbol, Workspace.CurrentSolution);

            // if there is at least one derived class, this class declaration can not have the 'sealed' modifier 
            return !derivedClasses.Any();
        }

        private static IList<MethodDeclarationSyntax> GetDisposeMethods(SyntaxNode classDecl)
        {
            return classDecl.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(m => m.Identifier.ToString().Equals("Dispose")).ToList();
        }

        private Dictionary<ClassDeclarationSyntax, ClassDeclarationSyntax> FixDisposePattern(
            IEnumerable<ClassDeclarationSyntax> nonSealedClassesThatImplementIDisposable)
        {
            var nodesToUpdate = new Dictionary<ClassDeclarationSyntax, ClassDeclarationSyntax>();
            foreach (var classDecl in nonSealedClassesThatImplementIDisposable)
            {
                var disposeMethods = GetDisposeMethods(classDecl);

                if (disposeMethods.Count == 2)
                {
                    // probably implements the dispose pattern correctly
                    continue;
                }

                var firstDisposeMethod = disposeMethods.FirstOrDefault();

                if (firstDisposeMethod is null ||
                    firstDisposeMethod.Modifiers.Any(m => m.ToString().Equals("abstract")))
                {
                    // dispose() method is abstract
                    continue;
                }

                var newNode =
                    RewriteService.ModifyDisposeAndAddProtectedDispose(classDecl, disposeMethods.FirstOrDefault());
                nodesToUpdate.Add(classDecl, newNode);
            }

            return nodesToUpdate;
        }

        private static ImmutableList<ClassDeclarationSyntax> GetNonSealedClasses(
            IEnumerable<ClassDeclarationSyntax> classes)
        {
            return classes
                .Where(c => c.Modifiers.All(m => !m.ToString().Equals("sealed")))
                .ToImmutableList();
        }

        private static ImmutableList<ClassDeclarationSyntax> GetClassesWithIDisposable(
            IEnumerable<ClassDeclarationSyntax> classes)
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

        private SyntaxNode AddSealedModifier(ClassDeclarationSyntax classDecl)
        {
            var newNode = RewriteService.AddSealedKeyword(classDecl);
            return newNode;
        }
    }
}
