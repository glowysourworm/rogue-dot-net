
namespace Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum
{
    public enum ScenarioMessageType
    {
        /// <summary>
        /// A normal scenario message (Player has found a door)
        /// </summary>
        Normal,

        /// <summary>
        /// A message regarding the details of a player's level advancement
        /// </summary>
        PlayerAdvancement,

        /// <summary>
        /// A message regarding the details of a player's skill advancement
        /// </summary>
        SkillAdvancement,

        /// <summary>
        /// A message showing the details of combat
        /// </summary>
        Melee,

        /// <summary>
        /// A message that shows the details of an enemy alteration effect cast towards a player
        /// </summary>
        EnemyAlteration,

        /// <summary>
        /// A message that shows any alteration effects towards a player on scenario tick
        /// </summary>
        Alteration
    }
}
