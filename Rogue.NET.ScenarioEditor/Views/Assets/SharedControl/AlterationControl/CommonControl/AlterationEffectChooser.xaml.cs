using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public AlterationEffectChooser(
                IRogueEventAggregator eventAggregator, 
                Type alterationType,
                Type alterationEffectInterfaceType)
        {
            InitializeComponent();

            // Validate that interface type is supported
            if (!ValidateSupportedType(alterationEffectInterfaceType))
                throw new Exception("Unsupported Alteration Interface Type");

            // Create type descriptions using reflection to get Attribute data
            var typeDescriptions = CreateTypeDescriptions(alterationEffectInterfaceType);

            // Set item source for effect combobox
            this.EffectTypeCB.ItemsSource = typeDescriptions;

            // Load Effect Region with proper view for the effect type
            this.DataContextChanged += (sender, e) =>
            {
                // If there is no effect chosen - then load the default for the region
                if (e.NewValue == null)
                {
                    eventAggregator.GetEvent<LoadDefaultRegionViewEvent>()
                                   .Publish(this.EffectRegion);

                    return;
                }

                // Effect Implementation Type
                var implementationType = e.NewValue.GetType();

                // Find Associated View Type
                var uiDescription = typeDescriptions.FirstOrDefault(x => x.ImplementationType == implementationType);

                // NOTE*** Have to filter out unknown types because data context is set from the container 
                //         once before the AlterationEffectChooser binding takes effect. So, won't be to
                //         validate the type better than that.
                if (uiDescription == null)
                    return;

                // Make sure to set the selected item for the combo box
                this.EffectTypeCB.SelectedItem = uiDescription;

                // If Content Type differs from view type publish a load event
                if (this.EffectRegion.Content == null ||
                    this.EffectRegion.Content.GetType() != uiDescription.ViewType)
                {
                    eventAggregator.GetEvent<LoadAlterationEffectRequestEvent>()
                                    .Publish(this.EffectRegion, new LoadAlterationEffectEventArgs()
                                    {
                                        AlterationType = alterationType,
                                        AlterationEffectViewType = uiDescription.ViewType
                                    });
                }
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
                                                AlterationType = alterationType,
                                                AlterationEffectViewType = effectDescription.ViewType
                                            });

                // New View Type
                else
                {
                    eventAggregator.GetEvent<LoadNewAlterationEffectRequestEvent>()
                                   .Publish(this.EffectRegion,
                                            new LoadNewAlterationEffectEventArgs()
                                            {                      
                                                AlterationType = alterationType,
                                                AlterationEffectType = effectDescription.ImplementationType,
                                                AlterationEffectViewType = effectDescription.ViewType
                                            });
                }
            };
        }

        private IEnumerable<AlterationEffectUIDescription> CreateTypeDescriptions(Type alterationEffectInterfaceType)
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
    }
}
