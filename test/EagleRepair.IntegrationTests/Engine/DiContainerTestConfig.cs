using Autofac;
using EagleRepair.Ast;
using EagleRepair.Ast.Parser;
using EagleRepair.Ast.RewriteCommand;
using EagleRepair.Monitor;

namespace EagleRepair.IntegrationTests.Engine
{
    public static class DiContainerTestConfig
    {
        public static ContainerBuilder Builder()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<RuleParser>().As<IRuleParser>();
            builder.RegisterType<ChangeTracker>().As<IChangeTracker>().SingleInstance();
            // register all rules
            builder.RegisterAssemblyTypes(typeof(AbstractRewriteCommand).Assembly)
                .Where(t => t.IsSubclassOf(typeof(AbstractRewriteCommand)))
                .As<AbstractRewriteCommand>();

            builder.RegisterType<EagleRepair.Ast.Engine>().As<IEngine>();


            return builder;
        }
    }
}