using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Extension;
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
        #region Routed Events
        public static readonly RoutedEvent AlterationEffectChosenEvent =
            EventManager.RegisterRoutedEvent("AlterationEffectChosen", 
                                             RoutingStrategy.Bubble, 
                                             typeof(RoutedEventHandler), 
                                             typeof(AlterationEffectChooser));

        public static void AddAlterationEffectChosenHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            var element = dependencyObject as UIElement;
            if (element != null)
                element.AddHandler(AlterationEffectChosenEvent, handler);
        }

        public static void RemoveAlterationEffectChosenHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            var element = dependencyObject as UIElement;
            if (element != null)
                element.RemoveHandler(AlterationEffectChosenEvent, handler);
        }

        #endregion

        #region Dependency Properties
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
        #endregion

        #region Nested Class
        public class AlterationEffectUIDescription
        {
            public string DisplayName { get; set; }
            public string ToolTip { get; set; }
            public Type ImplementationType { get; set; }
            public Type ViewType { get; set; }
        }

        #endregion

        protected IList<AlterationEffectUIDescription> TypeDescriptions;

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

        [ImportingConstructor]
        public AlterationEffectChooser(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();
            Initialize();

            // Listen for new alteration effect response to alert listeners
            eventAggregator.GetEvent<LoadAlterationEffectResponseEvent>()
                           .Subscribe((container, e) =>
                           {
                               // Alert listeners of the new alteration effect to initiate
                               // Binding update (from the container control)
                               this.RaiseEvent(new AlterationEffectChosenRoutedEventArgs(e.AlterationEffect, AlterationEffectChosenEvent, this));
                           });

            // Load Effect Region with proper view for the effect type
            this.DataContextChanged += (sender, e) =>
            {
                if (e.NewValue == null)
                    return;

                // Effect Implementation Type
                var implementationType = e.NewValue.GetType();

                // Find Associated View Type
                var uiDescription = this.TypeDescriptions.FirstOrDefault(x => x.ImplementationType == implementationType);

                // NOTE*** Have to filter out unknown types because data context is set from the container 
                //         once before the AlterationEffectChooser binding takes effect. So, won't be to
                //         validate the type better than that.
                if (uiDescription == null)
                    return;

                // Check to see if the implementation type is currently loaded
                if (this.EffectRegion.Content != null &&
                    this.EffectRegion.Content.GetType() == uiDescription.ViewType)
                    return;

                // Publish Load Event
                eventAggregator.GetEvent<LoadAlterationEffectRequestEvent>()
                               .Publish(this.EffectRegion, new LoadAlterationEffectEventArgs()
                               {
                                   AlterationEffectViewType = uiDescription.ViewType
                               });
            };

            // Load new effect type view when user clicks "Apply"
            this.OkButton.Click += (sender, e) =>
            {
                var effectDescription = this.EffectTypeCB.SelectedItem as AlterationEffectUIDescription;

                // Existing View Type
                if (this.EffectRegion.Content != null &
                    this.EffectRegion.Content.GetType() == effectDescription.ViewType)
                    eventAggregator.GetEvent<LoadAlterationEffectRequestEvent>()
                                   .Publish(this.EffectRegion,
                                            new LoadAlterationEffectEventArgs()
                                            {
                                                AlterationEffectViewType = effectDescription.ViewType
                                            });

                // New View Type
                else
                {
                    eventAggregator.GetEvent<LoadNewAlterationEffectRequestEvent>()
                                   .Publish(this.EffectRegion,
                                            new LoadNewAlterationEffectEventArgs()
                                            {
                                                AlterationEffectType = effectDescription.ImplementationType,
                                                AlterationEffectViewType = effectDescription.ViewType
                                            });
                }
            };
        }

        private void Initialize()
        {
            // Create list of alteration effect data for lookup for the combo box
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
                .Select(x => new AlterationEffectUIDescription()
                {
                    DisplayName = x.Value.DisplayName,
                    ToolTip = x.Value.Description,
                    ImplementationType = x.Key,
                    ViewType = x.Value.ViewType
                })
                .ToList();
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

                    // Load effect combo box with type descriptions (filtered by new interface type)
                    effectChooser
                        .EffectTypeCB
                        .ItemsSource = effectChooser
                                        .TypeDescriptions
                                        .Where(x =>
                                        {
                                            return x.ImplementationType.GetInterface(e.NewValue.ToString()) != null;
                                        });
                }
                else
                    throw new Exception("Unsupported Alteration Interface Type");
            }
        }
    }
}
