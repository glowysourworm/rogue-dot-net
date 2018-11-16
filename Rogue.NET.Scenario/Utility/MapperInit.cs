using ExpressMapper;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Scenario.Content.ViewModel.Content;

namespace Rogue.NET.Scenario.Utility
{
    public static class MapperInit
    {
        static MapperInit()
        {
            Mapper.Register<Equipment, EquipmentViewModel>();
            Mapper.Register<SkillSet, SkillSetViewModel>();

            Mapper.Compile();
        }
    }
}
