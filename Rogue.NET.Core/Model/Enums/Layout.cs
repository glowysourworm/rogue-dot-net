﻿using System.ComponentModel.DataAnnotations;

namespace Rogue.NET.Core.Model.Enums
{
    public enum LayoutType : int
    {
        [Display(Name = "Normal Rooms",
                 Description = "Generates connected rooms - non-overlapping")]
        RectangularRegion = 0,

        [Display(Name = "Random Rooms",
                 Description = "Generates connected rooms - overlapping or non-overlapping")]
        RandomRectangularRegion = 1,

        [Display(Name = "Maze Map",
                 Description = "Generates a single maze for the whole level!")]
        MazeMap = 2,

        [Display(Name = "Open World Map",
                 Description = "Generates an open layout (no defining borders) using a smooth elevation map")]
        ElevationMap = 3,

        [Display(Name = "Cave Map",
                 Description = "Generates a cave-like layout with some mild to moderate obstructions")]
        CellularAutomataMap = 4
    }
    public enum LayoutCellularAutomataType : int
    {
        [Display(Name = "Open",
                 Description = "Generates more mild obstructions")]
        Open = 0,

        [Display(Name = "Filled",
                 Description = "Generates more morderate obstructions")]
        Filled = 1
    }
    public enum LayoutConnectionType : int
    {
        [Display(Name = "Corridor",
                 Description = "Generates a corridor between rooms")]
        Corridor = 0,

        [Display(Name = "Teleporter",
                 Description = "Generates a teleporter pair between rooms")]
        Teleporter = 1,

        [Display(Name = "Maze",
                 Description = "Generates a maze in the empty space between rooms")]
        Maze = 2
    }
    public enum LayoutSymmetryType : int
    {
        [Display(Name = "2 - Way",
                 Description = "Generates a symmetric map using a left / right fold")]
        LeftRight = 0,

        [Display(Name = "4 - Way",
                 Description = "Generates a symmetric map using a 4-way fold")]
        Quadrant = 1
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
