using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface
{
    public interface IDifficultyAssetViewModel
    {
        string Id { get; }
        string Name { get; set; }
        int RequiredLevel { get; set; }

        /// <summary>
        /// Flag to say whether it's included in the calculation
        /// </summary>
        bool Included { get; set; }
    }
}
