using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SpellControl
{
    public partial class SpellTargetType : UserControl
    {
        public SpellTargetType()
        {
            InitializeComponent();
        }

        public void Inject()
        {
            // TODO
            //_containerViewModel = containerViewModel;

            //this.DataContext = model;

            //var template = model as SpellTemplate;
            //switch (template.Type)
            //{
            //    case AlterationType.AttackAttribute:
            //        {
            //            // set visibility of all targets
            //            this.AllTargetsRB.Visibility = System.Windows.Visibility.Collapsed;

            //            switch (template.AttackAttributeType)
            //            {
            //                case AlterationAttackAttributeType.Imbue:
            //                case AlterationAttackAttributeType.Passive:
            //                case AlterationAttackAttributeType.TemporaryFriendlySource:
            //                case AlterationAttackAttributeType.TemporaryMalignSource:
            //                    this.SourceRB.IsChecked = true;
            //                    break;
            //                default:
            //                    this.TargetRB.IsChecked = true;
            //                    break;
            //            }
            //        }
            //        break;
            //    case AlterationType.OtherMagicEffect:
            //    case AlterationType.PassiveAura:
            //        this.AllTargetsRB.Visibility = System.Windows.Visibility.Collapsed;
            //        break;
            //    default:
            //        break;
            //    case AlterationType.PassiveSource:
            //        this.SourceRB.IsChecked = true;
            //        this.AllTargetsRB.Visibility = System.Windows.Visibility.Collapsed;
            //        break;
            //    case AlterationType.PermanentSource:
            //    case AlterationType.TeleportSelf:
            //    case AlterationType.TemporarySource:
            //        this.SourceRB.IsChecked = true;
            //        break;
            //    case AlterationType.PermanentAllTargets:
            //    case AlterationType.TeleportAllTargets:
            //    case AlterationType.TemporaryAllTargets:
            //        this.AllTargetsRB.IsChecked = true;
            //        break;
            //    case AlterationType.PermanentTarget:
            //    case AlterationType.TeleportTarget:
            //    case AlterationType.TemporaryTarget:
            //        this.TargetRB.IsChecked = true;
            //        break;
            //}
        }

        private void SourceRB_Checked(object sender, RoutedEventArgs e)
        {
            var template = this.DataContext as SpellTemplate;
            switch (template.Type)
            {
                case AlterationType.AttackAttribute:
                    {
                        switch (template.AttackAttributeType)
                        {
                            default:
                                break;
                            case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                                template.AttackAttributeType = AlterationAttackAttributeType.TemporaryFriendlySource;
                                break;
                            case AlterationAttackAttributeType.TemporaryMalignTarget:
                                template.AttackAttributeType = AlterationAttackAttributeType.TemporaryMalignSource;
                                break;
                        }
                    }
                    break;
                case AlterationType.PermanentAllTargets:
                case AlterationType.PermanentTarget:
                    template.Type = AlterationType.PermanentSource;
                    break;
                case AlterationType.TeleportAllTargets:
                case AlterationType.TeleportTarget:
                    template.Type = AlterationType.TeleportSelf;
                    break;
                case AlterationType.TemporaryAllTargets:
                case AlterationType.TemporaryTarget:
                    template.Type = AlterationType.TemporarySource;
                    break;
                default: break;
            }
        }
        private void TargetRB_Checked(object sender, RoutedEventArgs e)
        {
            var template = this.DataContext as SpellTemplate;
            switch (template.Type)
            {
                case AlterationType.AttackAttribute:
                    {
                        switch (template.AttackAttributeType)
                        {
                            case AlterationAttackAttributeType.TemporaryFriendlySource:
                                template.AttackAttributeType = AlterationAttackAttributeType.TemporaryFriendlyTarget;
                                break;
                            case AlterationAttackAttributeType.TemporaryMalignSource:
                                template.AttackAttributeType = AlterationAttackAttributeType.TemporaryMalignTarget;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case AlterationType.PermanentSource:
                case AlterationType.PermanentAllTargets:
                    template.Type = AlterationType.PermanentTarget;
                    break;
                case AlterationType.TeleportSelf:
                case AlterationType.TeleportAllTargets:
                    template.Type = AlterationType.TeleportTarget;
                    break;
                case AlterationType.TemporarySource:
                case AlterationType.TemporaryAllTargets:
                    template.Type = AlterationType.TemporaryTarget;
                    break;
            }
        }
        private void AllTargetsRB_Checked(object sender, RoutedEventArgs e)
        {
            var template = this.DataContext as SpellTemplate;
            switch (template.Type)
            {
                default:
                    break;
                case AlterationType.PermanentSource:
                case AlterationType.PermanentTarget:
                    template.Type = AlterationType.PermanentSource;
                    break;                
                case AlterationType.TeleportTarget:
                case AlterationType.TeleportSelf:
                    template.Type = AlterationType.TeleportSelf;
                    break;
                case AlterationType.TemporaryTarget:
                case AlterationType.TemporarySource:
                    template.Type = AlterationType.TemporarySource;
                    break;
            }
        }
    }
}
