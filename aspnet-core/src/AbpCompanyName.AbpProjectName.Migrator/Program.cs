﻿using System;
using Abp;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Castle.Facilities.Logging;
using Abp.Castle.Logging.Log4Net;

namespace AbpCompanyName.AbpProjectName.Migrator
{
    public class Program
    {
        private static bool _skipConnVerification = false;
        private static bool _isAzure = false;

        public static void Main(string[] args)
        {
            ParseArgs(args);

            using (var bootstrapper = AbpBootstrapper.Create<AbpProjectNameMigratorModule>())
            {
                bootstrapper.IocManager.IocContainer
                    .AddFacility<LoggingFacility>(f => f.UseAbpLog4Net()
                        .WithConfig("log4net.config")
                    );

                bootstrapper.Initialize();

                using (var migrateExecuter = bootstrapper.IocManager.ResolveAsDisposable<MultiTenantMigrateExecuter>())
                {
                    migrateExecuter.Object.Run(_skipConnVerification);
                }

                if (!_isAzure) {
                    Console.WriteLine("Press ENTER to exit...");
                    Console.ReadLine();
                }
            }
        }

        private static void ParseArgs(string[] args)
        {
            if (args.IsNullOrEmpty())
            {
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                switch (arg)
                {
                    case "-s":
                        _skipConnVerification = true;
                        break;

                    case "-azure":
                        _skipConnVerification = true;
                        _isAzure = true;
                        break;
                }
            }
        }
    }
}
