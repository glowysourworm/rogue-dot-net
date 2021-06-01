using Prism.Mef;
using Prism.Modularity;

using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Rogue.NET.UnitTest.Extension
{
    public class UnitTestBootstrapper : MefBootstrapper
    {
        public UnitTestBootstrapper()
        {
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
        }
        protected override AggregateCatalog CreateAggregateCatalog()
        {
            var catalogs = new AssemblyCatalog[]
            {
                new AssemblyCatalog(Assembly.GetAssembly(typeof(CoreModule))),
                new AssemblyCatalog(Assembly.GetAssembly(typeof(NotifyViewModel))) // Common
            };
            return new AggregateCatalog(catalogs);
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ModuleCatalog(new List<ModuleInfo>()
            {
                new ModuleInfo("CoreModule", "CoreModule")
                {
                    InitializationMode = InitializationMode.WhenAvailable,
                    Ref = new Uri(typeof(CoreModule).Assembly.Location, UriKind.RelativeOrAbsolute).AbsoluteUri
                }
            });
        }
    }
}
