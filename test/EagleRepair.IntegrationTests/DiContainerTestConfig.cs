using Autofac;
using EagleRepair.Ast;
using EagleRepair.Ast.Parser;
using EagleRepair.Ast.RewriteCommand;
using EagleRepair.Ast.Services;
using EagleRepair.Cli;
using EagleRepair.Cli.Input;
using EagleRepair.Cli.Wrapper;
using EagleRepair.IntegrationTests.Mock;
using EagleRepair.Monitor;

namespace EagleRepair.IntegrationTests
{
    public static class DiContainerTestConfig
    {
        public static ContainerBuilder Builder()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<RewriteService>().As<IRewriteService>();
            builder.RegisterType<TypeService>().As<ITypeService>();
            builder.RegisterType<CmdLineValidator>().As<ICmdLineValidator>();
            builder.RegisterType<CmdLineReader>().As<ICmdLineReader>();
            builder.RegisterType<FileWrapperMock>().As<IFileWrapper>();
            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<RuleParser>().As<IRuleParser>();
            builder.RegisterType<ChangeTracker>().As<IChangeTracker>().SingleInstance();

            // register all rules
            builder.RegisterAssemblyTypes(typeof(AbstractRewriteCommand).Assembly)
                .Where(t => t.IsSubclassOf(typeof(AbstractRewriteCommand)))
                .As<AbstractRewriteCommand>();

            builder.RegisterType<Engine>().As<IEngine>();

            return builder;
        }
    }
}
