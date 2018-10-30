using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Assets;
using Rogue.NET.ScenarioEditor.Views.Assets.Animation;
using Rogue.NET.ScenarioEditor.Views.Assets.Consumable;
using Rogue.NET.ScenarioEditor.Views.Assets.Enemy;
using Rogue.NET.ScenarioEditor.Views.Assets.Equipment;
using Rogue.NET.ScenarioEditor.Views.Assets.Spell;
using Rogue.NET.ScenarioEditor.Views.Construction;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor
{
    [ModuleExport("ScenarioEditor", typeof(ScenarioEditorModule))]
    public class ScenarioEditorModule : IModule
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public ScenarioEditorModule(
            IRegionManager regionManager,
            IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEventArgs()
            {
                Message = "Loading Scenario Editor Module...",
                Progress = 70
            });
        }

        public void Initialize()
        {
        }

        //public void OnInitialized(IContainerProvider containerProvider)
        //{
        //    // Regions
        //    _regionManager.RegisterViewWithRegion("MainRegion", typeof(Editor));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EditorInstructions));
        //    _regionManager.RegisterViewWithRegion("AssetBrowserRegion", typeof(ScenarioAssetBrowser));
        //    _regionManager.RegisterViewWithRegion("ScenarioConstructionRegion", typeof(ScenarioConstruction));
        //    _regionManager.RegisterViewWithRegion("OutputRegion", typeof(Output));

        //    // Design Region - Asset Views
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(AnimationWizard));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ConsumableWizard));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EnemyWizard));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EquipmentWizard));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(SpellWizard));
        //    _regionManager.RegisterViewWithRegion("AnimationWizardRegion", () =>
        //    {
        //        return new Wizard(new WizardViewModel()
        //        {
        //            FirstPageType = typeof(AnimationBasicType),
        //            Title = "Animation Creation Wizard",
        //            WizardSteps = new string[] {
        //                "Select Basic Type",
        //                "Select Focus Type",
        //                "Set Animation Parameters",
        //                "Preview"
        //            }
        //        });
        //    });
        //    _regionManager.RegisterViewWithRegion("ConsumableWizardRegion", () =>
        //    {
        //        return new Wizard(new WizardViewModel()
        //        {
        //            FirstPageType = typeof(ConsumableSubType),
        //            Title = "Consumable Wizard",
        //            WizardSteps = new string[]{
        //                "Select Type",
        //                "Select Use Type",
        //                "Select Spell Usage",
        //                "Set Parameters",
        //                "Set Rogue Encyclopedia Data",
        //                "Edit Symbol"
        //            }
        //        });
        //    });
        //    _regionManager.RegisterViewWithRegion("EnemyWizardRegion", () =>
        //    {
        //        return new Wizard(new WizardViewModel()
        //        {
        //            FirstPageType = typeof(EnemyParameters),
        //            Title = "Enemy Wizard",
        //            WizardSteps = new string[]{
        //                "Set Melee Parameters",
        //                "Set Behavior",
        //                "Set Attack Attributes",
        //                "Set Items",
        //                "Set Rogue Encyclopedia Data",
        //                "Edit Symbol"
        //            }
        //        });
        //    });
        //    _regionManager.RegisterViewWithRegion("EquipmentWizardRegion", () =>
        //    {
        //        return new Wizard(new WizardViewModel()
        //        {
        //            FirstPageType = typeof(EquipmentSpellSelection),
        //            Title = "Equipment Wizard",
        //            WizardSteps = new string[]{
        //                "Select Attached Spells",
        //                "Set Parameters",
        //                "Set Attack Attributes",
        //                "Set Rogue Encyclopedia Data",
        //                "Edit Symbol"
        //            }
        //        });
        //    });
        //    _regionManager.RegisterViewWithRegion("SpellWizardRegion", () =>
        //    {
        //        return new Wizard(new WizardViewModel()
        //        {
        //            FirstPageType = typeof(SpellType),
        //            Title = "Spell Wizard",
        //            WizardSteps = new string[]{
        //                "Select Base Type",
        //                "Select Sub Type",
        //                "Set Parameters"
        //            }
        //        });
        //    });

        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Layout));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Doodad));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(SkillSet));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Brush));

        //    // Design Region - Construction Views
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(General));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(DungeonObjectPlacement));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EnemyPlacement));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ItemPlacement));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(LayoutDesign));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ObjectiveDesign));
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(PlayerDesign));

        //    // Design Region - Difficulty View
        //    _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ScenarioDifficultyChart));
        //}

        //public void RegisterTypes(IContainerRegistry containerRegistry)
        //{

        //}
    }
}
