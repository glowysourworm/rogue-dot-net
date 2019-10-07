using System.ComponentModel.DataAnnotations;

namespace Rogue.NET.Core.Model.Enums
{
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
        Source = 0,

        [Display(Name = "Affected Character(s)",
                 Description = "Applies the point animation to ALL (one-to-many) affected characters")]
        AffectedCharacters = 1
    }
    public enum AnimationEasingType
    {
        [Display(Name = "None",
                 Description = "Applies no easing function to the animation")]
        None,

        [Display(Name = "Back Ease",
                 Description = "Applies easing function that backs off before firing")]
        BackEase,

        [Display(Name = "Bounce Ease",
                 Description = "Applies easing function that bounces like a ball")]
        BounceEase,

        [Display(Name = "Exponential Ease",
                 Description = "Applies a somewhat dramatic acceleration to the animation")]
        ExponentialEase,

        [Display(Name = "Jolt Ease",
                 Description = "Applies an 'electric' feel to the animation - creating a fading spike at the fractional offset")]
        JoltEase
    }
}
