using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    public interface IUITypeAttributeProvider
    {
        IEnumerable<UITypeAttributeViewModel> GetUITypeDescriptions(UITypeAttributeBaseType baseType);
    }
}
