using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.CommonControl.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.CommonControl
{
    [Export]
    public partial class AlterationEffectChooser : UserControl
    {
        public static readonly DependencyProperty AlterationInterfaceTypeProperty =
            DependencyProperty.Register("AlterationInterfaceType", 
                                        typeof(Type), 
                                        typeof(AlterationEffectChooser), 
                                        new PropertyMetadata(
                                            new PropertyChangedCallback(OnAlterationInterfaceTypeChanged)));

        /// <summary>
        /// The interface type to the corresponding alteration container
        /// </summary>
        public Type AlterationInterfaceType
        {
            get { return (Type)GetValue(AlterationInterfaceTypeProperty); }
            set { SetValue(AlterationInterfaceTypeProperty, value); }
        }

        /// <summary>
        /// Event that signals a new view type to be loaded for editing (sends Type as parameter)
        /// </summary>
        public event EventHandler<AlterationEffectChosenEventArgs> AlterationEffectChosen;

        protected IDictionary<Type, UITypeAttribute> TypeDescriptions;

        private readonly IList<Type> _supportedInterfaceTypes = new List<Type>()
        {
            typeof(IConsumableAlterationEffectTemplateViewModel),
            typeof(IConsumableProjectileAlterationEffectTemplateViewModel),
            typeof(IDoodadAlterationEffectTemplateViewModel),
            typeof(IEnemyAlterationEffectTemplateViewModel),
            typeof(IEquipmentAttackAlterationEffectTemplateViewModel),
            typeof(IEquipmentCurseAlterationEffectTemplateViewModel),
            typeof(IEquipmentEquipAlterationEffectTemplateViewModel),
            typeof(ISkillAlterationEffectTemplateViewModel)
        };

        public AlterationEffectChooser()
        {
            InitializeComponent();
            Initialize();

            this.DataContextChanged += (sender, e) =>
            {
                if (e.NewValue == null)
                    return;
            };
        }

        private void Initialize()
        {
            // Create dictionary of { Type,  UITypeAttribute } for lookup
            this.TypeDescriptions = typeof(AlterationEffectChooser)
                .Assembly
                .GetTypes()
                .Select(type =>
                {
                    var customAttributes = type.GetCustomAttributes(typeof(UITypeAttribute), false);
                    if (customAttributes.Any())
                        return new KeyValuePair<Type, UITypeAttribute>(type, (UITypeAttribute)customAttributes.First());
                    else
                        return new KeyValuePair<Type, UITypeAttribute>();
                })
                .Where(x => x.Value != null)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        protected bool ValidateSupportedType(Type alterationContainerType)
        {
            return _supportedInterfaceTypes.Contains(alterationContainerType);
        }

        private static void OnAlterationInterfaceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var effectChooser = d as AlterationEffectChooser;
            if (effectChooser != null &&
                e.NewValue != null)
            {
                // If new interface type is supported
                if (effectChooser.ValidateSupportedType(e.NewValue as Type))
                {
                    // Clear effect combo box
                    effectChooser.EffectTypeCB.Items.Clear();

                    // Load effect combo box with new type descriptions
                    foreach (var item in effectChooser.TypeDescriptions
                                                      .Where(x => x.Key.GetInterface(e.NewValue.ToString()) != null)
                                                      .Select(x => new ListBoxItem()
                                                      {
                                                          Content = x.Value.DisplayName,
                                                          ToolTip = x.Value.Description,

                                                          // Store event args as tag
                                                          Tag = new AlterationEffectChosenEventArgs()
                                                          {
                                                              AlterationEffectType = x.Key,
                                                              AlterationEffectViewType = x.Value.ViewType
                                                          }
                                                      }))
                    {
                        effectChooser.EffectTypeCB.Items.Add(item);
                    }
                }
                else
                    throw new Exception("Unsupported Alteration Interface Type");
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if ((this.EffectTypeCB.SelectedItem as ListBoxItem) != null)
                AlterationEffectChosen(this, (this.EffectTypeCB.SelectedItem as ListBoxItem).Tag as AlterationEffectChosenEventArgs);
        }
    }
}
