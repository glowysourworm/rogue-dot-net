using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Common.Extension;

using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Windows.Media;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Processing.Symbol.Interface;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    public partial class SymbolComboBox : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly ISvgCache _svgCache;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(SymbolComboBox));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public SymbolComboBox()
        {
            // TODO: Figure out better mechanism for binding
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
            _svgCache = ServiceLocator.Current.GetInstance<ISvgCache>();

            InitializeComponent();
            Initialize();

            this.TheCB.SelectionChanged += (sender, e) =>
            {
                if (e.AddedItems.Count > 0)
                    this.Value = (e.AddedItems[0] as SvgSymbolViewModel).Symbol;
            };
        }

        private void Initialize()
        {
            this.TheCB.ItemsSource = _svgCache.GetResourceNames(SymbolType.Game).Select(symbol =>
            {
                var source = _scenarioResourceService.GetImageSource(ScenarioImage.CreateGameSymbol(symbol, symbol), 2.0);
                return SvgSymbolViewModel.CreateGameSymbol(source, symbol);
            });
        }
    }
}
