using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class AlteredCharacterStateTemplateViewModel : DungeonObjectTemplateViewModel
    {
        CharacterStateType _baseType;

        public CharacterStateType BaseType
        {
            get { return _baseType; }
            set { this.RaiseAndSetIfChanged(ref _baseType, value); }
        }

        public AlteredCharacterStateTemplateViewModel()
        {
            this.BaseType = CharacterStateType.Normal;
        }
    }
}
