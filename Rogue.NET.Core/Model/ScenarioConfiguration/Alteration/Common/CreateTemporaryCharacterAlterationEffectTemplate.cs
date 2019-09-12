using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class CreateTemporaryCharacterAlterationEffectTemplate : Template, IConsumableAlterationEffectTemplate,
                                                                              IDoodadAlterationEffectTemplate,
                                                                              IEnemyAlterationEffectTemplate,
                                                                              ISkillAlterationEffectTemplate
    {
        AlterationRandomPlacementType _randomPlacementType;
        TemporaryCharacterTemplate _temporaryCharacter;
        int _range;

        public AlterationRandomPlacementType RandomPlacementType
        {
            get { return _randomPlacementType; }
            set
            {
                if (_randomPlacementType != value)
                {
                    _randomPlacementType = value;
                    OnPropertyChanged("RandomPlacementType");
                }
            }
        }
        public TemporaryCharacterTemplate TemporaryCharacter
        {
            get { return _temporaryCharacter; }
            set
            {
                if (_temporaryCharacter != value)
                {
                    _temporaryCharacter = value;
                    OnPropertyChanged("TemporaryCharacter");
                }
            }
        }
        public int Range
        {
            get { return _range; }
            set
            {
                if (_range != value)
                {
                    _range = value;
                    OnPropertyChanged("Range");
                }
            }
        }

        public CreateTemporaryCharacterAlterationEffectTemplate() { }
    }
}
