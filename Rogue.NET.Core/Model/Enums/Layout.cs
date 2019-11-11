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
    public enum TerrainType
    {
        [Display(Name = "Aesthetic",
                 Description = "Creates a terrain layer that has no special effects")]
        Aesthetic = 0,

        [Display(Name = "Special Effect",
                 Description = "Creates a terrain layer with a special effect")]
        Alteration = 1
    }
    public enum TerrainLayoutType
    {
        [Display(Name = "Mutually Exclusive",
                 Description = "Terrain layer cannot be placed on top of - or underneath - any other terrain layer")]
        MutuallyExclusive = 0,

        [Display(Name = "Combined",
                 Description = "Terrain layer can be placed on top of - or underneath - other terrain layers")]
        Combined = 1
    }

    public enum TerrainAmbientLightingType
    {
        [Display(Name = "None",
                 Description = "No lighting generated")]
        None = 0,

        [Display(Name = "Flat Natural Lighting",
                 Description = "Generates fully lit rooms with the specified parameters")]
        Flat = 1,

        [Display(Name = "Lighted Rooms",
                 Description = "Generates fully lit rooms with the specified parameters")]
        LightedRooms = 2,

        [Display(Name = "Large Natural Lighting",
                 Description = "Natural lighting generated using random features similar to your layout")]
        PerlinNoiseLarge = 3,

        [Display(Name = "Small Natural Lighting",
                 Description = "Natural lighting generated using random features that are smaller compared to your layout features")]
        PerlinNoiseSmall = 4,

        [Display(Name = "Speckeled Natural Lighting",
                 Description = "Natural lighting generated using white noise")]
        WhiteNoise = 5,

        [Display(Name = "Wall Lighting",
                 Description = "Point source lights emanating from random wall tiles")]
        WallLighting = 6
    }
}
