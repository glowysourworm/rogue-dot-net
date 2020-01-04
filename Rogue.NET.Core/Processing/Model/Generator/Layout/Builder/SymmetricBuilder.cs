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
        readonly IConnectionBuilder _connectionBuilder;
        readonly IRegionTriangulationCreator _triangulationCreator;

        [ImportingConstructor]
        public SymmetricBuilder(IRegionBuilder regionBuilder, IConnectionBuilder connectionBuilder, IRegionTriangulationCreator triangulationCreator)
        {
            _regionBuilder = regionBuilder;
            _connectionBuilder = connectionBuilder;
            _triangulationCreator = triangulationCreator;
        }

        public LayoutContainer CreateSymmetricLayout(LayoutTemplate template)
        {
            // Procedure
            //
            // 0) Generate the BASE layer with connections
            // 1) Execute the grid folding - copy over Wall and Corridor information
            // 2) Re-identify BASE LAYER regions as (NOT WALL && NOT CORRIDORS) (THESE WILL OVERWRITE THE BASE LAYER)
            // 3) Calculate the CONNECTION layer
            // 4) Set BASE and CONNECTION layer in the container
            //

            // Step 0 - Base + Initial Connection Layer
            var container = _regionBuilder.BuildRegions(template);

            // Step 1 - Grid Folding
            ExecuteLayoutFolding(container, template);

            // Step 2 - Re-Identify BASE layer (These would be the original "Rooms")
            var newBaseRegions = container.Grid.ConstructConnectedRegions(cell => !cell.IsWall && !cell.IsCorridor);

            // Identify connection regions (These may have a new topology; and don't resemble the original "Rooms")
            var connectionRegions = container.Grid.ConstructConnectedRegions(cell => !cell.IsWall);

            // Create connection triangulation (This is used with the connection regions to create corridors or connections later)
            var connectionGraph = _triangulationCreator.CreateTriangulation(connectionRegions, template);

            // SET NEW DATA IN THE CONTAINER
            container.SetBaseLayer(newBaseRegions);
            container.SetConnectionLayer(connectionRegions, connectionGraph);

            return container;
        }

        private void ExecuteLayoutFolding(LayoutContainer container, LayoutTemplate template)
        {
            switch (template.SymmetryType)
            {
                case LayoutSymmetryType.LeftRight:
                    {
                        // E Half
                        for (int i = 0; i < container.Grid.GetLength(0) / 2; i++)
                        {
                            var mirrorColumn = container.Grid.GetLength(0) - i - 1;

                            for (int j = 0; j < container.Grid.GetLength(1); j++)
                            {
                                // E -> W
                                if (container.Grid[i, j] != null)
                                    container.Grid[mirrorColumn, j] = new GridCellInfo(mirrorColumn, j)
                                    {
                                        IsWall = container.Grid[i, j].IsWall,
                                        IsCorridor = container.Grid[i, j].IsCorridor
                                    };

                                // W -> E
                                else if (container.Grid[mirrorColumn, j] != null)
                                    container.Grid[i, j] = new GridCellInfo(i, j)
                                    {
                                        IsWall = container.Grid[mirrorColumn, j].IsWall,
                                        IsCorridor = container.Grid[mirrorColumn, j].IsCorridor
                                    };
                            }
                        }
                    }
                    break;
                case LayoutSymmetryType.Quadrant:
                    {
                        // NE, SE, SW Quadrants
                        for (int i = 0; i < container.Grid.GetLength(0) / 2; i++)
                        {
                            var mirrorColumn = container.Grid.GetLength(0) - i - 1;

                            for (int j = 0; j < container.Grid.GetLength(1) / 2; j++)
                            {
                                var mirrorRow = container.Grid.GetLength(1) - j - 1;

                                GridCellInfo[] cells = new GridCellInfo[4];

                                // Find cell to mirror - start with NW

                                // NW
                                if (container.Grid[i, j] != null)
                                    cells[0] = container.Grid[i, j];

                                // NE
                                else if (container.Grid[mirrorColumn, j] != null)
                                    cells[1] = container.Grid[mirrorColumn, j];

                                // SE
                                else if (container.Grid[mirrorColumn, mirrorRow] != null)
                                    cells[2] = container.Grid[mirrorColumn, mirrorRow];

                                // SW
                                else if (container.Grid[i, mirrorRow] != null)
                                    cells[3] = container.Grid[i, mirrorRow];

                                var cell = cells.FirstOrDefault(x => x != null);

                                // Mirror cell over to other quadrants
                                if (cell != null)
                                {
                                    container.Grid[i, j] = new GridCellInfo(i, j)
                                    {
                                        IsWall = cell.IsWall,
                                        IsCorridor = cell.IsCorridor
                                    };

                                    container.Grid[mirrorColumn, j] = new GridCellInfo(mirrorColumn, j)
                                    {
                                        IsWall = cell.IsWall,
                                        IsCorridor = cell.IsCorridor
                                    };

                                    container.Grid[i, mirrorRow] = new GridCellInfo(i, mirrorRow)
                                    {
                                        IsWall = cell.IsWall,
                                        IsCorridor = cell.IsCorridor
                                    };

                                    container.Grid[mirrorColumn, mirrorRow] = new GridCellInfo(mirrorColumn, mirrorRow)
                                    {
                                        IsWall = cell.IsWall,
                                        IsCorridor = cell.IsCorridor
                                    };
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
