using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class MetaDataControl : UserControl
    {
        public static readonly DependencyProperty HasUniqueSettingProperty =
            DependencyProperty.Register("HasUniqueSetting", typeof(bool), typeof(MetaDataControl), new PropertyMetadata(true));

        public static readonly DependencyProperty HasCursedSettingProperty =
            DependencyProperty.Register("HasCursedSetting", typeof(bool), typeof(MetaDataControl), new PropertyMetadata(true));

        public bool HasUniqueSetting
        {
            get { return (bool)GetValue(HasUniqueSettingProperty); }
            set { SetValue(HasUniqueSettingProperty, value); }
        }

        public bool HasCursedSetting
        {
            get { return (bool)GetValue(HasCursedSettingProperty); }
            set { SetValue(HasCursedSettingProperty, value); }
        }

        public MetaDataControl()
        {
            InitializeComponent();
        }
    }
}
