
namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IDialogPlayerAdvancementUpdate : IDialogUpdate
    {
        int PlayerPoints { get; set; }
        double Strength { get; set; }
        double Agility { get; set; }
        double Intelligence { get; set; }
        int SkillPoints { get; set; }
    }
}
