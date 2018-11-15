using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    public interface IItemProcessor
    {
        bool ClassApplies(EquipmentType type);
    }
}
