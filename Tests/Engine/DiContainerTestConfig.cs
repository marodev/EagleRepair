using Ast;
using Ast.Parser;
using Ast.RewriteCommand;
using Autofac;
using Monitor;

namespace Tests.Engine
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

            builder.RegisterType<Ast.Engine>().As<IEngine>();


            return builder;
        }
    }
}