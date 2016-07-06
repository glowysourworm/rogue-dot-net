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

namespace Rogue.NET.ScenarioEditor.Views.Assets.Enemy
{
    public partial class EnemyAttackAttributes : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public EnemyAttackAttributes()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(EnemyItems); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var config = containerViewModel.SecondaryPayload as ScenarioConfiguration;
            var enemy = model as EnemyTemplate;
            if (config != null && enemy != null)
            {
                foreach (var attrib in config.AttackAttributes)
                {
                    if (!enemy.AttackAttributes.Any(z => z.Name == attrib.Name))
                        enemy.AttackAttributes.Add(new AttackAttributeTemplate() { Name = attrib.Name });
                }
                for (int i = enemy.AttackAttributes.Count - 1; i >= 0; i--)
                {
                    var attrib = enemy.AttackAttributes[i];
                    if (!config.AttackAttributes.Any(z => z.Name == attrib.Name))
                        enemy.AttackAttributes.RemoveAt(i);
                }

                this.AttackAttributesLB.ItemsSource = enemy.AttackAttributes;
            }
        }
    }
}
