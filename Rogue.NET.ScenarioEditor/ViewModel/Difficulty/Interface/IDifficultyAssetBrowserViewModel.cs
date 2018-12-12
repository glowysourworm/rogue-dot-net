using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface
{
    public interface IDifficultyAssetBrowserViewModel
    {
        IEnumerable<IDifficultyAssetViewModel> Assets { get; }
    }
}
