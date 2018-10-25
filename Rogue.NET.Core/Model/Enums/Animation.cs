using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rogue.NET.Core.Model.Enums
{
    public enum AnimationType
    {
        ProjectileSelfToTarget,
        ProjectileTargetToSelf,
        ProjectileSelfToTargetsInRange,
        ProjectileTargetsInRangeToSelf,
        AuraSelf,
        AuraTarget,
        BubblesSelf,
        BubblesTarget,
        BubblesScreen,
        BarrageSelf,
        BarrageTarget,
        SpiralSelf,
        SpiralTarget,
        ChainSelfToTargetsInRange,
        ScreenBlink
    }
}
