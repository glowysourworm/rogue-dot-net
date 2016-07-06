using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor
{
    public class ScenarioEditorModule : IModule
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;
        readonly IUnityContainer _unityContainer;

        public ScenarioEditorModule(
            IUnityContainer unityContainer,
            IRegionManager regionManager,
            IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEvent()
            {
                Message = "Loading Scenario Editor Module...",
                Progress = 70
            });

            // Controller (singleton)
            _unityContainer.RegisterType<IScenarioEditorController, ScenarioEditorController>(new ContainerControlledLifetimeManager());
            _unityContainer.Resolve(typeof(ScenarioEditorController));

            // View Model Types
            _unityContainer.RegisterType<IEditorViewModel, EditorViewModel>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IScenarioAssetGroupViewModel, ScenarioAssetGroupViewModel>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IScenarioAssetBrowserViewModel, ScenarioAssetBrowserViewModel>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IScenarioConstructionViewModel, ScenarioConstructionViewModel>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IScenarioDifficultyViewModel, ScenarioDifficultyViewModel>(new ContainerControlledLifetimeManager());

            // Views
            _unityContainer.RegisterType<Editor>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<EditorInstructions>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<ScenarioAssetBrowser>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<ScenarioConstruction>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<Output>(new ContainerControlledLifetimeManager());

            _unityContainer.RegisterType<Wizard>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<AnimationBasicType>(new ContainerControlledLifetimeManager());

            // Regions
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(Editor));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EditorInstructions));
            _regionManager.RegisterViewWithRegion("AssetBrowserRegion", typeof(ScenarioAssetBrowser));
            _regionManager.RegisterViewWithRegion("ScenarioConstructionRegion", typeof(ScenarioConstruction));
            _regionManager.RegisterViewWithRegion("OutputRegion", typeof(Output));

            // Design Region - Asset Views
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(AnimationWizard));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ConsumableWizard)); 
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EnemyWizard));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EquipmentWizard));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(SpellWizard));
            _regionManager.RegisterViewWithRegion("AnimationWizardRegion", () =>
            {
                return new Wizard(_unityContainer, new WizardViewModel()
                {
                    FirstPageType = typeof(AnimationBasicType),
                    Title = "Animation Creation Wizard",
                    WizardSteps = new string[] {
                        "Select Basic Type",
                        "Select Focus Type",
                        "Set Animation Parameters",
                        "Preview"
                    }
                });
            });
            _regionManager.RegisterViewWithRegion("ConsumableWizardRegion", () =>
            {
                return new Wizard(_unityContainer, new WizardViewModel()
                {
                    FirstPageType = typeof(ConsumableSubType),
                    Title = "Consumable Wizard",
                    WizardSteps = new string[]{
                        "Select Type",
                        "Select Use Type",
                        "Select Spell Usage",
                        "Set Parameters",
                        "Set Rogue Encyclopedia Data",
                        "Edit Symbol"
                    }
                });
            });
            _regionManager.RegisterViewWithRegion("EnemyWizardRegion", () =>
            {
                return new Wizard(_unityContainer, new WizardViewModel()
                {
                    FirstPageType = typeof(EnemyParameters),
                    Title = "Enemy Wizard",
                    WizardSteps = new string[]{
                        "Set Melee Parameters",
                        "Set Behavior",
                        "Set Attack Attributes",
                        "Set Items",
                        "Set Rogue Encyclopedia Data",
                        "Edit Symbol"
                    }
                });
            });
            _regionManager.RegisterViewWithRegion("EquipmentWizardRegion", () =>
            {
                return new Wizard(_unityContainer, new WizardViewModel()
                {
                    FirstPageType = typeof(EquipmentSpellSelection),
                    Title = "Equipment Wizard",
                    WizardSteps = new string[]{
                        "Select Attached Spells",
                        "Set Parameters",
                        "Set Attack Attributes",
                        "Set Rogue Encyclopedia Data",
                        "Edit Symbol"
                    }
                });
            });
            _regionManager.RegisterViewWithRegion("SpellWizardRegion", () =>
            {
                return new Wizard(_unityContainer, new WizardViewModel()
                {
                    FirstPageType = typeof(SpellType),
                    Title = "Spell Wizard",
                    WizardSteps = new string[]{
                        "Select Base Type",
                        "Select Sub Type",
                        "Set Parameters"
                    }
                });
            });

            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Layout));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Doodad));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(SkillSet));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Brush));

            // Design Region - Construction Views
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(General));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(DungeonObjectPlacement));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EnemyPlacement));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ItemPlacement));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(LayoutDesign));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ObjectiveDesign));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(PlayerDesign));

            // Design Region - Difficulty View
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ScenarioDifficultyChart));
        }
    }
}
