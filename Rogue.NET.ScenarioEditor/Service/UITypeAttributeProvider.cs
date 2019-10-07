using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IUITypeAttributeProvider))]
    public class UITypeAttributeProvider : IUITypeAttributeProvider
    {
        static readonly IEnumerable<UITypeAttributeViewModel> _uiTypeCollection;

        static UITypeAttributeProvider()
        {
            // Create list of alteration effect data for lookup
            //
            _uiTypeCollection = 
                typeof(UITypeAttributeProvider)
                    .Assembly
                    .GetTypes()
                    .Select(type =>
                    {
                        var customAttributes = type.GetCustomAttributes(typeof(UITypeAttribute), false);
                        if (customAttributes.Any())
                            return new KeyValuePair<Type, UITypeAttribute>(type, (UITypeAttribute)customAttributes.First());
                        else
                            return new KeyValuePair<Type, UITypeAttribute>();
                    })
                    .Where(x => x.Value != null)
                    .Select(x => new UITypeAttributeViewModel()
                    {
                        DisplayName = x.Value.DisplayName,
                        Description = x.Value.Description,
                        ViewType = x.Value.ViewType,
                        ViewModelType = x.Key,
                        BaseType = x.Value.BaseType
                    })
                    .ToList();
        }
        public IEnumerable<UITypeAttributeViewModel> GetUITypeDescriptions(UITypeAttributeBaseType baseType)
        {
            return _uiTypeCollection.Where(x => x.BaseType == baseType)
                                    .Actualize();
        }
    }
}
