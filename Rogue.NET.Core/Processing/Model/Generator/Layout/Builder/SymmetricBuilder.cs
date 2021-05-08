using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISymmetricBuilder))]
    public class SymmetricBuilder : ISymmetricBuilder
    {
        readonly IRegionBuilder _regionBuilder;

        [ImportingConstructor]
        public SymmetricBuilder(IRegionBuilder regionBuilder)
        {
            _regionBuilder = regionBuilder;
        }

        public LayoutContainer CreateSymmetricLayout(LayoutTemplate template)
        {
            // Procedure
            //
            // 0) Generate the BASE region layer
            // 1) Execute the grid folding - copy over cell information
            //

            // Step 0 - Base + Initial Connection Layer
            var container = _regionBuilder.BuildRegions(template);

            // Step 1 - Grid Folding
            ExecuteLayoutFolding(container, template);

            return container;
        }

        private void ExecuteLayoutFolding(LayoutContainer container, LayoutTemplate template)
        {
            switch (template.SymmetryType)
            {
                case LayoutSymmetryType.LeftRight:
                    {
                        // E Half
                        for (int i = 0; i < container.Width / 2; i++)
                        {
                            var mirrorColumn = container.Width - i - 1;

                            for (int j = 0; j < container.Height; j++)
                            {
                                // E -> W
                                if (container.Get(i, j) != null)
                                    container.SetLayout(mirrorColumn, j, new GridCellInfo(mirrorColumn, j)
                                    {
                                        IsWall = container.Get(i, j).IsWall,
                                        IsCorridor = container.Get(i, j).IsCorridor
                                    });

                                // W -> E
                                else if (container.Get(mirrorColumn, j) != null)
                                         container.SetLayout(i, j, new GridCellInfo(i, j)
                                         {
                                             IsWall = container.Get(mirrorColumn, j).IsWall,
                                             IsCorridor = container.Get(mirrorColumn, j).IsCorridor
                                         });
                            }
                        }
                    }
                    break;
                case LayoutSymmetryType.Quadrant:
                    {
                        // NE, SE, SW Quadrants
                        for (int i = 0; i < container.Width / 2; i++)
                        {
                            var mirrorColumn = container.Width - i - 1;

                            for (int j = 0; j < container.Height / 2; j++)
                            {
                                var mirrorRow = container.Height - j - 1;

                                GridCellInfo[] cells = new GridCellInfo[4];

                                // Find cell to mirror - start with NW

                                // NW
                                if (container.Get(i, j) != null)
                                    cells[0] = container.Get(i, j);

                                // NE
                                else if (container.Get(mirrorColumn, j) != null)
                                    cells[1] = container.Get(mirrorColumn, j);

                                // SE
                                else if (container.Get(mirrorColumn, mirrorRow) != null)
                                    cells[2] = container.Get(mirrorColumn, mirrorRow);

                                // SW
                                else if (container.Get(i, mirrorRow) != null)
                                    cells[3] = container.Get(i, mirrorRow);

                                var cell = cells.FirstOrDefault(x => x != null);

                                // Mirror cell over to other quadrants
                                if (cell != null)
                                {
                                    container.SetLayout(i, j, new GridCellInfo(i, j)
                                    {
                                        IsWall = cell.IsWall,
                                        IsCorridor = cell.IsCorridor
                                    });

                                    container.SetLayout(mirrorColumn, j, new GridCellInfo(mirrorColumn, j)
                                    {
                                        IsWall = cell.IsWall,
                                        IsCorridor = cell.IsCorridor
                                    });

                                    container.SetLayout(i, mirrorRow, new GridCellInfo(i, mirrorRow)
                                    {
                                        IsWall = cell.IsWall,
                                        IsCorridor = cell.IsCorridor
                                    });

                                    container.SetLayout(mirrorColumn, mirrorRow, new GridCellInfo(mirrorColumn, mirrorRow)
                                    {
                                        IsWall = cell.IsWall,
                                        IsCorridor = cell.IsCorridor
                                    });
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception("Unhandled symmetry type SymmetryBuilder");
            }
        }
    }
}
