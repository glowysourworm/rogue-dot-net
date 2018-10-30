using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Spell
{
    public partial class SpellType : UserControl, IWizardPage
    {
        public SpellType()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get 
            {
                var template = this.DataContext as SpellTemplate;
                if (template == null)
                    return typeof(Filler);

                switch (template.Type)
                {
                    case AlterationType.PassiveSource:
                    case AlterationType.PassiveAura:
                        return typeof(SpellPassive);
                    case AlterationType.TemporarySource:
                    case AlterationType.TemporaryTarget:
                    case AlterationType.TemporaryAllTargets:
                    case AlterationType.PermanentSource:
                    case AlterationType.PermanentTarget:
                    case AlterationType.PermanentAllTargets:
                    case AlterationType.TeleportSelf:
                    case AlterationType.TeleportTarget:
                    case AlterationType.TeleportAllTargets:
                        return typeof(SpellTargetType);
                    case AlterationType.OtherMagicEffect:
                        {
                            switch (template.OtherEffectType)
                            {
                                case AlterationMagicEffectType.ChangeLevelRandomUp:
                                case AlterationMagicEffectType.ChangeLevelRandomDown:
                                    return typeof(SpellLevelChange);
                                case AlterationMagicEffectType.RevealItems:
                                case AlterationMagicEffectType.RevealMonsters:
                                case AlterationMagicEffectType.RevealSavePoint:
                                case AlterationMagicEffectType.RevealFood:
                                case AlterationMagicEffectType.RevealLevel:
                                    return typeof(SpellRevealType);
                                default:
                                    return typeof(Filler);
                            }
                        }
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.TemporaryFriendlySource:
                                case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                                case AlterationAttackAttributeType.TemporaryMalignSource:
                                case AlterationAttackAttributeType.TemporaryMalignTarget:
                                    return typeof(SpellTargetType);
                                default:
                                    return typeof(Filler);
                            }
                        }
                    default:
                        return typeof(Filler);
                }
            }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var template = model as SpellTemplate;
            if (template == null)
                return;

            foreach (RadioButton button in this.RadioStack.Children)
            {
                if (Enum.GetName(typeof(AlterationType), button.Tag) == template.Type.ToString())
                    button.IsChecked = true;

                if (Enum.GetName(typeof(AlterationMagicEffectType), button.Tag) == template.Type.ToString())
                    button.IsChecked = true;
            }
        }

        IWizardViewModel _containerViewModel;

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var model = this.DataContext as SpellTemplate;
            if (model != null)
            {
                if (radioButton.Tag is AlterationType)
                    model.Type = (AlterationType)radioButton.Tag;

                else
                    model.OtherEffectType = (AlterationMagicEffectType)radioButton.Tag;
            }
        }
    }
}
