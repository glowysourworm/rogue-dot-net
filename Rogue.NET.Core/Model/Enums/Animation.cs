namespace Rogue.NET.Core.Model.Enums
{
    public enum AnimationType
    {
        ProjectileSelfToTarget = 0,
        ProjectileTargetToSelf = 1,
        ProjectileSelfToTargetsInRange = 2,
        ProjectileTargetsInRangeToSelf = 3,
        AuraSelf = 4,
        AuraTarget = 5,
        BubblesSelf = 6,
        BubblesTarget = 7,
        BubblesScreen = 8,
        BarrageSelf = 9,
        BarrageTarget = 10,
        SpiralSelf = 11,
        SpiralTarget = 12,
        ChainSelfToTargetsInRange = 13,
        ScreenBlink = 14
    }

    public enum AnimationBaseType
    {
        Projectile = 0,
        ProjectileReverse = 1,
        Aura = 2,
        Bubbles = 3,
        Barrage = 4,
        Spiral = 5,
        Chain = 6,
        ChainReverse = 7,
        ScreenBlink = 8
    }
}
