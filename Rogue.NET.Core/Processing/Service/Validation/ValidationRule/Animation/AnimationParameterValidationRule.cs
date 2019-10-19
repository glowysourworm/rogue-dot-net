using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Extension;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Animation
{
    public class AnimationParameterValidationRule : IScenarioValidationRule
    {
        public AnimationParameterValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            return configuration.GetAllAlterations()
                                // ...Where Animation
                                .Where(x => x.Alteration.Animation.Animations.Count > 0)
                                // ...Is Trying to Create a Projectile
                                .Where(x => x.Alteration.Animation.Animations.Any(z => z is AnimationChainTemplate ||
                                                                            z is AnimationChainConstantVelocityTemplate ||
                                                                            z is AnimationLightningTemplate ||
                                                                            z is AnimationLightningChainTemplate ||
                                                                            z is AnimationProjectileTemplate ||
                                                                            z is AnimationProjectileConstantVelocityTemplate))
                                // ...That will fire at the source character
                                .Where(x => x.Alteration.Animation.TargetType == AlterationTargetType.AllInRange ||
                                            x.Alteration.Animation.TargetType == AlterationTargetType.Source)
                                .Select(x =>
                new ScenarioValidationResult()
                {
                    Asset = x.Asset,
                    Message = "Improper Animation Parameters: Projectile and Chain Animations must not have the Source as an Affected Character",
                    Passed = false,
                    Severity = ValidationSeverity.Error,
                    InnerMessage = x.Asset.Name + " Animation is improperly set. Try setting Target Type to 'Target'"
                });
        }
    }
}
