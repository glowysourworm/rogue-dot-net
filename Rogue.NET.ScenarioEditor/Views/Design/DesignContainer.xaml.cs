using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace Rogue.NET.ScenarioEditor.Views.Design
{
    [Export]
    public partial class DesignContainer : UserControl
    {
        public DesignContainer()
        {
            InitializeComponent();

            this.DataContextChanged += (sender, e) =>
            {
                if (e.NewValue is LevelBranchTemplateViewModel)
                {
                    this.DesignNameTextBlock.Text = (e.NewValue as LevelBranchTemplateViewModel).Name;
                    this.DesignTypeTextRun.Text = "Level Branch";
                }
                else if (e.NewValue is LevelTemplateViewModel)
                {
                    this.DesignNameTextBlock.Text = (e.NewValue as LevelTemplateViewModel).Name;
                    this.DesignTypeTextRun.Text = "Level";
                }
                else
                {
                    this.DesignNameTextBlock.Text = "Unknown";
                    this.DesignTypeTextRun.Text = "Unknown";
                }
            };
        }
    }
}
