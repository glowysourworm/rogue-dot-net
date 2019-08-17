using System.ComponentModel.DataAnnotations;

namespace Rogue.NET.Core.Model.Enums
{
    public enum AnimationBaseType
    {
        [Display(Name = "Projectile",
                 Description = "Animates a particle from source character to the affected character(s)")]
        Projectile = 0,

        [Display(Name = "Projectile Reverse",
                 Description = "Animates a particle from affect character(s) to the source character")]
        ProjectileReverse = 1,

        [Display(Name = "Aura",
                 Description = "Animates a elliptical shape around a character")]
        Aura = 2,

        [Display(Name = "Bubbles",
                 Description = "Animates one-to-many particles around a character in a random flurry")]
        Bubbles = 3,

        [Display(Name = "Barrage",
                 Description = "Animates one-to-many particles toward-or-away-from character focal point")]
        Barrage = 4,

        [Display(Name = "Spiral",
                 Description = "Animates one-to-many particles encircling a character")]
        Spiral = 5,

        [Display(Name = "Chain",
                 Description = "Animates a particle from source character to the affected character(s) in sequence")]
        Chain = 6,

        [Display(Name = "Chain",
                 Description = "Animates a particle from affected character(s) to the source character in sequence")]
        ChainReverse = 7,

        [Display(Name = "Screen Blink",
                 Description = "Animates an overlay for the entire level with the specified parameters")]
        ScreenBlink = 8
    }

    /// <summary>
    /// Specifies source / affected character usage for point animations. Affected characters
    /// are calculated by the Alteration.AnimationGroup using the AlterationTargetType. This
    /// specifies how the alteration calculated its targets. The point animation type further
    /// specifies how individual affected characters may be used during the animation sequence.
    /// </summary>
    public enum AnimationPointTargetType
    {
        [Display(Name = "Source",
                 Description = "Applies the point animation to the source character")]
        Source = 1,

        [Display(Name = "Affected Character(s)",
                 Description = "Applies the point animation to ALL (one-to-many) affected characters")]
        AffectedCharacters = 2
    }
}
