using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Layout
{
    public class LayoutConnectionGeometryValidationRule : IScenarioValidationRule
    {
        public LayoutConnectionGeometryValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            // TODO:TERRAIN
            return new IScenarioValidationResult[] { };

            //var layouts = configuration.LayoutTemplates
            //                            .Where(template =>
            //                            {
            //                                return template.ConnectionGeometryType == LayoutConnectionGeometryType.Rectilinear &&
            //                                       (template.RoomPlacementType != LayoutRoomPlacementType.RectangularGrid ||
            //                                        template.Type == LayoutType.ConnectedCellularAutomata);
            //                            })
            //                            .Select(template => template.Name)
            //                            .Actualize();

            //return layouts.Select(layoutName =>
            //{
            //    return new ScenarioValidationResult()
            //    {
            //        Passed = false,
            //        Severity = ValidationSeverity.Error,
            //        Message = "Layout Connection Geometry Type of Rectilinear should only be paired with a Room Placement Type of RectangularGrid",
            //        InnerMessage = "Layout Template " + layoutName
            //    };
            //});
        }
    }
}
