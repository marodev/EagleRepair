using Autofac;
using EagleRepair.Ast;
using EagleRepair.Ast.Parser;
using EagleRepair.Ast.Rewriter;
using EagleRepair.Ast.Services;
using EagleRepair.Cli.Input;
using EagleRepair.Cli.Wrapper;
using EagleRepair.Monitor;

namespace EagleRepair.Cli
{
    public static class DiContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ProgressBar>().As<IProgressBar>().SingleInstance();
            builder.RegisterType<DisplayService>().As<IDisplayService>();
            builder.RegisterType<RewriteService>().As<IRewriteService>();
            builder.RegisterType<TypeService>().As<ITypeService>();
            builder.RegisterType<FileWrapper>().As<IFileWrapper>();
            builder.RegisterType<RuleParser>().As<IRuleParser>();
            builder.RegisterType<SolutionParser>().As<ISolutionParser>();
            builder.RegisterType<CmdLineValidator>().As<ICmdLineValidator>();
            builder.RegisterType<CmdLineReader>().As<ICmdLineReader>();
            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<ChangeTracker>().As<IChangeTracker>().SingleInstance();

            // register all rules
            builder.RegisterAssemblyTypes(typeof(AbstractRewriter).Assembly)
                .Where(t => t.IsSubclassOf(typeof(AbstractRewriter)))
                .As<AbstractRewriter>();

            builder.RegisterType<Engine>().As<IEngine>();

            return builder.Build();
        }
    }
}
