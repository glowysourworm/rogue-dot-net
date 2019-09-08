using Rogue.NET.Core.Utility;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class LoadingView : UserControl
    {
        [ImportingConstructor]
        public LoadingView()
        {
            var colors = ColorUtility.CreateColors().ToList();

            InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                var random = new Random(DateTime.Now.Millisecond);

                var next = random.Next(0, colors.Count);
                var randomColor = colors[next].Color;

                this.SmileyCtrl.SmileyColor = randomColor;
                this.SmileyCtrl.SmileyLineColor = ColorUtility.Inverse(randomColor);
            };
        }
    }
}
