using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Converter.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Layout
{
    public class LayoutSizeValidationRule : IScenarioValidationRule
    {
        readonly LayoutHeightConverter _layoutHeightConverter;
        readonly LayoutWidthConverter _layoutWidthConverter;

        public LayoutSizeValidationRule()
        {
            _layoutHeightConverter = new LayoutHeightConverter();
            _layoutWidthConverter = new LayoutWidthConverter();
        }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var layoutSizes = configuration.LayoutTemplates
                                           .Select(template =>
                                           {
                                               return new
                                               {
                                                   Size = _layoutHeightConverter.Convert(template) * _layoutWidthConverter.Convert(template),
                                                   Template = template
                                               };
                                           })
                                           .Where(result => result.Size >= 10000)
                                           .Actualize();

            return layoutSizes.Select(result =>
                new ScenarioValidationResult()
                {
                    Asset = result.Template,
                    Message = "Layout max size must be less than 10,000 cells",
                    Severity = ValidationSeverity.Error,
                    Passed = false,
                    InnerMessage = "Layout Template " + result.Template.Name + " has a size of " + result.Size.ToString()
                });
        }
    }
}
