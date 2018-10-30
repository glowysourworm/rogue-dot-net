using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Equipment
{
    public partial class EquipmentAttackAttributes : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public EquipmentAttackAttributes()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(EquipmentMetadata); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var config = containerViewModel.SecondaryPayload as ScenarioConfigurationContainer;
            var equipment = model as EquipmentTemplate;
            if (config != null && equipment != null)
            {
                foreach (var attrib in config.AttackAttributes)
                {
                    if (!equipment.AttackAttributes.Any(z => z.Name == attrib.Name))
                        equipment.AttackAttributes.Add(new AttackAttributeTemplate() { Name = attrib.Name });
                }
                for (int i = equipment.AttackAttributes.Count - 1; i >= 0; i--)
                {
                    var attrib = equipment.AttackAttributes[i];
                    if (!config.AttackAttributes.Any(z => z.Name == attrib.Name))
                        equipment.AttackAttributes.RemoveAt(i);
                }

                this.AttackAttributesLB.ItemsSource = equipment.AttackAttributes;
            }
        }
    }
}
