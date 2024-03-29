﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;

namespace EagleRepair.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                return StartApp(args).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // return non-zero exit code
            return 1;
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

            return !succeeded ? 1 : 0;
        }
    }
}
