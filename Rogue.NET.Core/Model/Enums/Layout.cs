using System;
using System.ComponentModel.DataAnnotations;

namespace Rogue.NET.Core.Model.Enums
{
    public enum LayoutType : int
    {
        [Display(Name = "Rectangular Layout",
                 Description = "Generates connected rooms - non-overlapping")]
        RectangularRegion = 0,

        [Display(Name = "Random Layout",
                 Description = "Generates connected rooms - overlapping or non-overlapping")]
        RandomRectangularRegion = 1,

        [Display(Name = "Random Layout (Cave-Like)",
                 Description = "Generates connected rooms - overlapping or non-overlapping")]
        RandomSmoothedRegion = 2,

        [Display(Name = "Maze Layout",
                 Description = "Generates a single maze for the whole level!")]
        MazeMap = 3,

        [Display(Name = "Maze Layout (Cave-Like)",
                 Description = "Generates a maze inside of a cave for the whole level!")]
        CellularAutomataMazeMap = 4,

        [Display(Name = "Open Cave",
                 Description = "Generates an open layout (no defining borders) using a smooth elevation map")]
        ElevationMap = 5,

        [Display(Name = "Open Cave Maze Layout",
                 Description = "Generates a maze inside of an open layout (no defining borders) using a smooth elevation map")]
        ElevationMazeMap = 6,

        [Display(Name = "Closed Cave",
                 Description = "Generates a cave-like layout with some mild to moderate obstructions")]
        CellularAutomataMap = 7
    }
    public enum LayoutCellularAutomataType : int
    {
        [Display(Name = "Open",
                 Description = "Generates more mild obstructions")]
        Open = 0,

        [Display(Name = "Filled (Less)",
                 Description = "Generates more morderate obstructions")]
        FilledLess = 1,

        [Display(Name = "Filled (More)",
                 Description = "Generates more obstructions")]
        FilledMore = 2
    }
    public enum LayoutConnectionType : int
    {
        [Display(Name = "Corridor",
                 Description = "Generates a corridor between rooms")]
        Corridor = 0,

        [Display(Name = "Transport Points",
                 Description = "Generates a scenario object pair to transport character between rooms")]
        ConnectionPoints = 1,

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
        [Display(Name = "Overlay",
                 Description = "Terrain layer overlays other terrain layers beneath it")]
        Overlay = 0,

        [Display(Name = "Completely Exclusive",
                 Description = "Terrain layer cannot be placed with any other terrain layers")]
        CompletelyExclusive = 1,

        [Display(Name = "Combined",
                 Description = "Terrain layer can be placed with other terrain layers - not marked exclusive or overlaying this one")]
        Combined = 2
    }
    public enum TerrainConnectionType
    {
        [Display(Name = "Direct",
                 Description = "Regions separated by terrain are connected directly")]
        Direct = 0,

        [Display(Name = "Avoid",
                 Description = "Regions separated by terrain are connected - avoiding the impassible terrain")]
        Avoid = 1
    }

    [Flags]
    public enum TerrainMaskingType
    {
        [Display(Name = "None", 
                 Description = "No tiles are masked off during terrain creation")]
        None = 0,

        [Display(Name = "Rooms",
                 Description = "Room regions are masked off during terrain creation")]
        Regions = 1,

        [Display(Name = "Corridors",
                 Description = "Corridors are masked off during terrain creation")]
        Corridors = 2,

        [Display(Name = "Empty Space",
                 Description = "Empty space is masked off during terrain creation")]
        EmptySpace = 4
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

        [Display(Name = "Smooth Natural Lighting",
                 Description = "Natural lighting generated using random features similar to your layout")]
        PerlinNoiseLarge = 1,

        [Display(Name = "Speckeled Natural Lighting",
                 Description = "Natural lighting generated using white noise")]
        WhiteNoise = 2
    }
}
