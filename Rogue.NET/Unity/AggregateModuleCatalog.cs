using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Rogue.NET.Unity
{
    public class AggregateModuleCatalog : IModuleCatalog
    {
        IModuleCatalog _catalog = new ModuleCatalog();

        public AggregateModuleCatalog()
        {
        }

        public IEnumerable<ModuleInfo> Modules
        {
            get
            {
                return _catalog.Modules;
            }
        }

        public void Initialize()
        {
            _catalog.Initialize();
        }

        public void AddModule(ModuleInfo moduleInfo)
        {
            _catalog.AddModule(moduleInfo);
        }

        public IEnumerable<ModuleInfo> CompleteListWithDependencies(IEnumerable<ModuleInfo> modules)
        {
            return modules.SelectMany(module => GetDependentModules(module));
        }

        public IEnumerable<ModuleInfo> GetDependentModules(ModuleInfo moduleInfo)
        {
            var modules = _catalog.Modules.Where(module => moduleInfo.DependsOn.Contains(module.ModuleName));
            return modules.SelectMany(module => GetDependentModules(module));
        }
    }
}
