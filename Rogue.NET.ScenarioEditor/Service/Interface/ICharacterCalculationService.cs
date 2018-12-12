using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    public interface ICharacterCalculationService
    {
        RangeViewModel<double> CalculateEnemyAttack(EnemyTemplateViewModel enemyTemplate);
        RangeViewModel<double> CalculateEnemyDefense(EnemyTemplateViewModel enemyTemplate);
    }
}
