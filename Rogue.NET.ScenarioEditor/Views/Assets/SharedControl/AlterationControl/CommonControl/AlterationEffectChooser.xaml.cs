using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Events.Asset.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.CommonControl
{
    /// <summary>
    /// AlterationEffectChooser must be instantiated by user code. 
    /// </summary>
    public partial class AlterationEffectChooser : UserControl
    {
        #region Nested Class
        public class AlterationEffectUIDescription
        {
            public string DisplayName { get; set; }
            public string ToolTip { get; set; }
            public Type ImplementationType { get; set; }
            public Type ViewType { get; set; }
        }

        #endregion

        public static readonly DependencyProperty AlterationProperty =
            DependencyProperty.Register("Alteration", typeof(AlterationTemplateViewModel), typeof(AlterationEffectChooser));

        public static readonly DependencyProperty AlterationEffectProperty =
            DependencyProperty.Register("AlterationEffect", typeof(IAlterationEffectTemplateViewModel), typeof(AlterationEffectChooser), new PropertyMetadata(new PropertyChangedCallback(OnAlterationEffectChanged)));

        public static readonly DependencyProperty AlterationEffectInterfaceTypeProperty =
            DependencyProperty.Register("AlterationEffectInterfaceType", typeof(Type), typeof(AlterationEffectChooser), new PropertyMetadata(new PropertyChangedCallback(OnAlterationEffectInterfaceTypeChanged)));

        public AlterationTemplateViewModel Alteration
        {
            get { return (AlterationTemplateViewModel)GetValue(AlterationProperty); }
            set { SetValue(AlterationProperty, value); }
        }

        public IAlterationEffectTemplateViewModel AlterationEffect
        {
            get { return (IAlterationEffectTemplateViewModel)GetValue(AlterationEffectProperty); }
            set { SetValue(AlterationEffectProperty, value); }
        }

        public Type AlterationEffectInterfaceType
        {
            get { return (Type)GetValue(AlterationEffectInterfaceTypeProperty); }
            set { SetValue(AlterationEffectInterfaceTypeProperty, value); }
        }

        private static readonly IRogueEventAggregator _eventAggregator;

        private readonly IList<Type> _supportedInterfaceTypes = new List<Type>()
        {
            typeof(IConsumableAlterationEffectTemplateViewModel),
            typeof(IConsumableProjectileAlterationEffectTemplateViewModel),
            typeof(IDoodadAlterationEffectTemplateViewModel),
            typeof(IEnemyAlterationEffectTemplateViewModel),
            typeof(IEquipmentAttackAlterationEffectTemplateViewModel),
            typeof(IEquipmentCurseAlterationEffectTemplateViewModel),
            typeof(IEquipmentEquipAlterationEffectTemplateViewModel),
            typeof(IFriendlyAlterationEffectTemplateViewModel),
            typeof(ITemporaryCharacterAlterationEffectTemplateViewModel),
            typeof(ISkillAlterationEffectTemplateViewModel)
        };

        static AlterationEffectChooser()
        {
            _eventAggregator = ServiceLocator.Current.GetInstance<IRogueEventAggregator>();
        }

        public AlterationEffectChooser()
        {
            InitializeComponent();

            // Load new effect type view when user clicks "Apply"
            this.OkButton.Click += (sender, e) =>
            {
                var effectDescription = this.EffectTypeCB.SelectedItem as AlterationEffectUIDescription;

                // Existing View Type
                if (this.EffectRegion.Content != null &
                    this.EffectRegion.Content.GetType() == effectDescription.ViewType)
                    _eventAggregator.GetEvent<LoadAlterationEffectRequestEvent>()
                                    .Publish(this.EffectRegion,
                                            new LoadAlterationEffectEventArgs()
                                            {                                             
                                                AlterationEffectViewType = effectDescription.ViewType
                                            });

                // New View Type
                else
                {
                    // Alteration was properly set so that the new alteration will have an identitiy
                    // for the ScenarioEditorModule to broadcast.
                    if (this.Alteration != null)
                    {
                        _eventAggregator.GetEvent<LoadNewAlterationEffectRequestEvent>()
                                        .Publish(this.EffectRegion,
                                                new LoadNewAlterationEffectEventArgs()
                                                {
                                                    Alteration = this.Alteration,
                                                    AlterationEffectType = effectDescription.ImplementationType,
                                                    AlterationEffectViewType = effectDescription.ViewType
                                                });
                    }
                    else
                        throw new Exception("Alteration not properly set by the container");
                }
            };
        }

        protected IEnumerable<AlterationEffectUIDescription> CreateTypeDescriptions(Type alterationEffectInterfaceType)
        {
            // Create list of alteration effect data for lookup for the combo box filtered by
            // the alteration effect interface type
            return typeof(AlterationEffectChooser)
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
                .Where(x => x.Key.GetInterface(alterationEffectInterfaceType.Name) != null)
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

        private static void OnAlterationEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AlterationEffectChooser;
            var alterationEffect = e.NewValue as IAlterationEffectTemplateViewModel;

            // If there is no effect chosen - then load the default for the region
            if (alterationEffect == null)
            {
                _eventAggregator.GetEvent<LoadDefaultRegionViewEvent>()
                                .Publish(control.EffectRegion);

                return;
            }

            if (control != null &&
                alterationEffect != null)
            {
                // Effect Implementation Type
                var implementationType = e.NewValue.GetType();

                // Get Type Descriptions from list box
                var typeDescriptions = control.EffectTypeCB.ItemsSource as IEnumerable<AlterationEffectUIDescription>;

                // Find Associated View Type
                var uiDescription = typeDescriptions.FirstOrDefault(x => x.ImplementationType == implementationType);

                // NOTE*** Have to filter out unknown types because data context is set from the container 
                //         once before the AlterationEffectChooser binding takes effect. So, won't be to
                //         validate the type better than that.
                if (uiDescription == null)
                    return;

                // Make sure to set the selected item for the combo box
                control.EffectTypeCB.SelectedItem = uiDescription;

                // If Content Type differs from view type publish a load event
                if (control.EffectRegion.Content == null ||
                    control.EffectRegion.Content.GetType() != uiDescription.ViewType)
                {
                    _eventAggregator.GetEvent<LoadAlterationEffectRequestEvent>()
                                    .Publish(control.EffectRegion, new LoadAlterationEffectEventArgs()
                                    {
                                        AlterationEffectViewType = uiDescription.ViewType
                                    });
                }
            }
        }

        private static void OnAlterationEffectInterfaceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AlterationEffectChooser;
            var interfaceType = e.NewValue as Type;

            if (control != null &&
                interfaceType != null)
            {
                // Validate that interface type is supported
                if (!control.ValidateSupportedType(interfaceType))
                    throw new Exception("Unsupported Alteration Interface Type");

                // Create type descriptions using reflection to get Attribute data
                var typeDescriptions = control.CreateTypeDescriptions(interfaceType);

                // Set item source for effect combobox
                control.EffectTypeCB.ItemsSource = typeDescriptions;
            }
        }
    }
}
