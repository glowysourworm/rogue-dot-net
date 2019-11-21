using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Enums
{
    public enum LayoutType : int
    {
        // (DON'T RENUMBER) Numbers prevent loss of data - so enums can be refactored
        Maze = 1,
        ConnectedRectangularRooms = 7,
        ConnectedCellularAutomata = 8
    }
    public enum LayoutCellularAutomataType : int
    {
        Open = 0,
        Filled = 1
    }
    public enum LayoutConnectionType : int
    {
        Corridor = 0,
        CorridorWithDoors = 1,
        Teleporter = 2,
        TeleporterRandom = 3
    }
    public enum LayoutConnectionGeometryType : int
    {
        /// <summary>
        /// Available for RectangularGrid room placement type
        /// </summary>
        Rectilinear = 0,

        /// <summary>
        /// Uses Minimum Spanning Tree algorithm to generate room connections
        /// </summary>
        MinimumSpanningTree = 1
    }
    public enum LayoutCorridorGeometryType : int
    {
        /// <summary>
        /// Straight line connecting cells from two rooms
        /// </summary>
        Linear = 0,
    }
    public enum LayoutRoomPlacementType : int
    {
        /// <summary>
        /// Rectangular grid of rooms
        /// </summary>
        RectangularGrid = 0,

        /// <summary>
        /// Random placement of rectangular rooms
        /// </summary>
        Random = 1
    }
    public enum LayoutMandatoryLocationType
    {
        /// <summary>
        /// Mandatory location for the stairs up
        /// </summary>
        StairsUp,

        /// <summary>
        /// Mandatory location for the stairs down
        /// </summary>
        StairsDown,

        /// <summary>
        /// Mandatory location for the save point
        /// </summary>
        SavePoint,

        /// <summary>
        /// Mandatory location for a room connecting doodad pair (1)
        /// </summary>
        RoomConnector1,

        /// <summary>
        /// Mandatory location for a room connecting doodad pair (2)
        /// </summary>
        RoomConnector2,

        /// <summary>
        /// Mandatory location for a doodad pair (1) that was created when dividing another region with terrain
        /// </summary>
        TerrainConnector1,

        /// <summary>
        /// Mandatory location for a doodad pair (2) that was created when dividing another region with terrain
        /// </summary>
        TerrainConnector2
    }
    public enum TerrainType
    {
        [Display(Name = "Aesthetic",
                 Description = "Creates a terrain layer that has no special effects")]
        Aesthetic = 0,

        [Display(Name = "Special Effect",
                 Description = "Creates a terrain layer with a special effect")]
        Alteration = 1
    }
    public enum TerrainLayer
    {
        [Display(Name = "Below Ground",
                 Description = "Terrain layer that is placed underneath above layers")]
        BelowGround = 0,

        [Display(Name = "Ground",
                 Description = "Terrain layer that is placed at ground level")]
        Ground = 1,

        [Display(Name = "Above Ground",
                 Description = "Terrain layer that is placed above ground level")]
        AboveGround = 2
    }
    public enum TerrainLayoutType
    {
        [Display(Name = "Layer Exclusive",
                 Description = "Terrain layer cannot be placed with any other terrain layer at the specified level")]
        LayerExclusive = 0,

        [Display(Name = "Completely Exclusive",
                 Description = "Terrain layer cannot be placed with any other terrain layers")]
        CompletelyExclusive = 1,

        [Display(Name = "Combined",
                 Description = "Terrain layer can be placed with other terrain layers - not marked exclusive at the specified level")]
        Combined = 2
    }
    public enum TerrainGenerationType
    {
        [Display(Name = "Smooth Features",
                 Description = "Creates a terrain layer that has smoothly changing features")]
        PerlinNoise = 0
    }

    public enum TerrainAmbientLightingType
    {
        [Display(Name = "None",
                 Description = "No lighting generated")]
        None = 0,

        [Display(Name = "Lighted Rooms",
                 Description = "Generates fully lit rooms with the specified parameters")]
        LightedRooms = 1,

        [Display(Name = "Large Natural Lighting",
                 Description = "Natural lighting generated using random features similar to your layout")]
        PerlinNoiseLarge = 2,

        [Display(Name = "Small Natural Lighting",
                 Description = "Natural lighting generated using random features that are smaller compared to your layout features")]
        PerlinNoiseSmall = 3,

        [Display(Name = "Speckeled Natural Lighting",
                 Description = "Natural lighting generated using white noise")]
        WhiteNoise = 4,

        [Display(Name = "Wall Lighting",
                 Description = "Point source lights emanating from random wall tiles")]
        WallLighting = 5
    }
}
