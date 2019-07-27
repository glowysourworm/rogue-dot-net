using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IPlayerAdvancementCommandAction : IPlayerCommandAction
    {
        double Strength { get; set; }
        double Agility { get; set; }
        double Intelligence { get; set; }
        int SkillPoints { get; set; }
    }
}
