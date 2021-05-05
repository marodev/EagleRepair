using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;

namespace EagleRepair.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return StartApp(args).Result;
        }

        private static async Task<int> StartApp(IEnumerable<string> commandLineArgs)
        {
            // build dependency injection container
            var container = DiContainerConfig.Configure();
            // create scope, DI-container is going to dispose objects at the end of the function
            await using var scope = container.BeginLifetimeScope();
            var app = scope.Resolve<IApplication>();
            // start our app
            var succeeded = await app.Run(commandLineArgs);

            return succeeded ? 0 : 1;
        }
    }
}
