using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Enums
{
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
}
