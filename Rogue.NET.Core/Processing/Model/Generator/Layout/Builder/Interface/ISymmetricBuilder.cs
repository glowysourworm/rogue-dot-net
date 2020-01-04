using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface ISymmetricBuilder
    {
        /// <summary>
        /// Creates a BASE and CONNECTION layer from the template
        /// </summary>
        LayoutContainer CreateSymmetricLayout(LayoutTemplate template);
    }
}
