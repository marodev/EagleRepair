using System.Runtime.InteropServices;
using Ast;
using Ast.Parser;
using Ast.RewriteCommand;
using Autofac;
using Microsoft.CodeAnalysis.CSharp;
using Monitor;

namespace Cli
{
    public static class DiContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterType<RuleParser>().As<IRuleParser>();
            builder.RegisterType<SolutionParser>().As<ISolutionParser>();
            builder.RegisterType<CmdLineValidator>().As<ICmdLineValidator>();
            builder.RegisterType<CmdLineReader>().As<ICmdLineReader>();
            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<ChangeTracker>().As<IChangeTracker>().SingleInstance();
            // register all rules
            builder.RegisterAssemblyTypes(typeof(AbstractRewriteCommand).Assembly)
                .Where(t => t.IsSubclassOf(typeof(AbstractRewriteCommand)))
                .As<AbstractRewriteCommand>();

            builder.RegisterType<Engine>().As<IEngine>();
            

            return builder.Build();
        }
    }
}