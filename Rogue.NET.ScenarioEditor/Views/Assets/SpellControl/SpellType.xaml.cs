using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SpellControl
{
    public partial class SpellType : UserControl
    {
        public SpellType()
        {
            InitializeComponent();
        }

        // TODO
        //public Type NextPage
        //{
        //    get 
        //    {
        //        var template = this.DataContext as SpellTemplate;
        //        if (template == null)
        //            return typeof(Filler);

        //        switch (template.Type)
        //        {
        //            case AlterationType.PassiveSource:
        //            case AlterationType.PassiveAura:
        //                return typeof(SpellPassive);
        //            case AlterationType.TemporarySource:
        //            case AlterationType.TemporaryTarget:
        //            case AlterationType.TemporaryAllTargets:
        //            case AlterationType.PermanentSource:
        //            case AlterationType.PermanentTarget:
        //            case AlterationType.PermanentAllTargets:
        //            case AlterationType.TeleportSelf:
        //            case AlterationType.TeleportTarget:
        //            case AlterationType.TeleportAllTargets:
        //                return typeof(SpellTargetType);
        //            case AlterationType.OtherMagicEffect:
        //                {
        //                    switch (template.OtherEffectType)
        //                    {
        //                        case AlterationMagicEffectType.ChangeLevelRandomUp:
        //                        case AlterationMagicEffectType.ChangeLevelRandomDown:
        //                            return typeof(SpellLevelChange);
        //                        case AlterationMagicEffectType.RevealItems:
        //                        case AlterationMagicEffectType.RevealMonsters:
        //                        case AlterationMagicEffectType.RevealSavePoint:
        //                        case AlterationMagicEffectType.RevealFood:
        //                        case AlterationMagicEffectType.RevealLevel:
        //                            return typeof(SpellRevealType);
        //                        default:
        //                            return typeof(Filler);
        //                    }
        //                }
        //            case AlterationType.AttackAttribute:
        //                {
        //                    switch (template.AttackAttributeType)
        //                    {
        //                        case AlterationAttackAttributeType.TemporaryFriendlySource:
        //                        case AlterationAttackAttributeType.TemporaryFriendlyTarget:
        //                        case AlterationAttackAttributeType.TemporaryMalignSource:
        //                        case AlterationAttackAttributeType.TemporaryMalignTarget:
        //                            return typeof(SpellTargetType);
        //                        default:
        //                            return typeof(Filler);
        //                    }
        //                }
        //            default:
        //                return typeof(Filler);
        //        }
        //    }
        //}


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
