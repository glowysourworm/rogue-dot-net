using Rogue.NET.Model;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Spell
{
    public partial class SpellTargetType : UserControl, IWizardPage
    {
        public SpellTargetType()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(SpellParameters); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var template = model as SpellTemplate;
            switch (template.Type)
            {
                case Common.AlterationType.AttackAttribute:
                    {
                        // set visibility of all targets
                        this.AllTargetsRB.Visibility = System.Windows.Visibility.Collapsed;

                        switch (template.AttackAttributeType)
                        {
                            case Common.AlterationAttackAttributeType.Imbue:
                            case Common.AlterationAttackAttributeType.Passive:
                            case Common.AlterationAttackAttributeType.TemporaryFriendlySource:
                            case Common.AlterationAttackAttributeType.TemporaryMalignSource:
                                this.SourceRB.IsChecked = true;
                                break;
                            default:
                                this.TargetRB.IsChecked = true;
                                break;
                        }
                    }
                    break;
                case Common.AlterationType.OtherMagicEffect:
                case Common.AlterationType.PassiveAura:
                    this.AllTargetsRB.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                default:
                    break;
                case Common.AlterationType.PassiveSource:
                    this.SourceRB.IsChecked = true;
                    this.AllTargetsRB.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Common.AlterationType.PermanentSource:
                case Common.AlterationType.TeleportSelf:
                case Common.AlterationType.TemporarySource:
                    this.SourceRB.IsChecked = true;
                    break;
                case Common.AlterationType.PermanentAllTargets:
                case Common.AlterationType.TeleportAllTargets:
                case Common.AlterationType.TemporaryAllTargets:
                    this.AllTargetsRB.IsChecked = true;
                    break;
                case Common.AlterationType.PermanentTarget:
                case Common.AlterationType.TeleportTarget:
                case Common.AlterationType.TemporaryTarget:
                    this.TargetRB.IsChecked = true;
                    break;
            }
        }

        IWizardViewModel _containerViewModel;

        private void SourceRB_Checked(object sender, RoutedEventArgs e)
        {
            var template = this.DataContext as SpellTemplate;
            switch (template.Type)
            {
                case Common.AlterationType.AttackAttribute:
                    {
                        switch (template.AttackAttributeType)
                        {
                            default:
                                break;
                            case Common.AlterationAttackAttributeType.TemporaryFriendlyTarget:
                                template.AttackAttributeType = Common.AlterationAttackAttributeType.TemporaryFriendlySource;
                                break;
                            case Common.AlterationAttackAttributeType.TemporaryMalignTarget:
                                template.AttackAttributeType = Common.AlterationAttackAttributeType.TemporaryMalignSource;
                                break;
                        }
                    }
                    break;
                case Common.AlterationType.PermanentAllTargets:
                case Common.AlterationType.PermanentTarget:
                    template.Type = Common.AlterationType.PermanentSource;
                    break;
                case Common.AlterationType.TeleportAllTargets:
                case Common.AlterationType.TeleportTarget:
                    template.Type = Common.AlterationType.TeleportSelf;
                    break;
                case Common.AlterationType.TemporaryAllTargets:
                case Common.AlterationType.TemporaryTarget:
                    template.Type = Common.AlterationType.TemporarySource;
                    break;
                default: break;
            }
        }
        private void TargetRB_Checked(object sender, RoutedEventArgs e)
        {
            var template = this.DataContext as SpellTemplate;
            switch (template.Type)
            {
                case Common.AlterationType.AttackAttribute:
                    {
                        switch (template.AttackAttributeType)
                        {
                            case Common.AlterationAttackAttributeType.TemporaryFriendlySource:
                                template.AttackAttributeType = Common.AlterationAttackAttributeType.TemporaryFriendlyTarget;
                                break;
                            case Common.AlterationAttackAttributeType.TemporaryMalignSource:
                                template.AttackAttributeType = Common.AlterationAttackAttributeType.TemporaryMalignTarget;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case Common.AlterationType.PermanentSource:
                case Common.AlterationType.PermanentAllTargets:
                    template.Type = Common.AlterationType.PermanentTarget;
                    break;
                case Common.AlterationType.TeleportSelf:
                case Common.AlterationType.TeleportAllTargets:
                    template.Type = Common.AlterationType.TeleportTarget;
                    break;
                case Common.AlterationType.TemporarySource:
                case Common.AlterationType.TemporaryAllTargets:
                    template.Type = Common.AlterationType.TemporaryTarget;
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
                case Common.AlterationType.PermanentSource:
                case Common.AlterationType.PermanentTarget:
                    template.Type = Common.AlterationType.PermanentSource;
                    break;                
                case Common.AlterationType.TeleportTarget:
                case Common.AlterationType.TeleportSelf:
                    template.Type = Common.AlterationType.TeleportSelf;
                    break;
                case Common.AlterationType.TemporaryTarget:
                case Common.AlterationType.TemporarySource:
                    template.Type = Common.AlterationType.TemporarySource;
                    break;
            }
        }
    }
}
