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

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    public partial class SymbolComboBox : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public static readonly DependencyProperty SymbolTypeProperty =
            DependencyProperty.Register("SymbolType", typeof(SymbolType), typeof(SymbolComboBox),
                new PropertyMetadata(new PropertyChangedCallback(OnSymbolTypeChanged)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(SymbolComboBox));

        public SymbolType SymbolType
        {
            get { return (SymbolType)GetValue(SymbolTypeProperty); }
            set { SetValue(SymbolTypeProperty, value); }
        }
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public SymbolComboBox()
        {
            // TODO: Figure out better mechanism for binding
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();

            InitializeComponent();
            Initialize(this.SymbolType);

            this.TheCB.SelectionChanged += (sender, e) =>
            {
                if (e.AddedItems.Count > 0)
                    this.Value = (e.AddedItems[0] as SvgSymbolViewModel).Symbol;
            };
        }

        private void Initialize(SymbolType type)
        {
            var assembly = typeof(IRogueEventAggregator).Assembly;

            // For now, just parse the names like the folder structure (figure out more robust way later)
            //
            var prefix = type == SymbolType.Symbol ? "Rogue.NET.Common.Resource.Svg.Scenario.Symbol."
                                                   : "Rogue.NET.Common.Resource.Svg.Game.";

            // Get resources from the character folder -> Parse out category names
            var items = assembly.GetManifestResourceNames()
                                .Where(x => x.Contains(prefix))
                                .Select(x => x.Replace(prefix, ""))
                                .Select(x =>
                                {
                                    var pieces = x.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                                    if (pieces.Length != 2)
                                        throw new Exception("Resource file-name format differs from expected");

                                    // [FileName].svg

                                    // Load Image Source
                                    ImageSource imageSource;

                                    if (type == SymbolType.Symbol)
                                        imageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(pieces[0],
                                                                                                                pieces[0],
                                                                                                                0.0,
                                                                                                                1.0,
                                                                                                                1.0), 2.0);

                                    else
                                        imageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(pieces[0],
                                                                                                                pieces[0]), 2.0);

                                    return new SvgSymbolViewModel(imageSource, pieces[0], 0, 1.0, 1.0);
                                })
                                .Actualize();

            this.TheCB.ItemsSource = items;
        }

        private static void OnSymbolTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SymbolComboBox;
            
            if (control != null &&
                e.NewValue != null)
            {
                var type = (SymbolType)e.NewValue;
                control.Initialize(type);
            }
        }
    }
}
