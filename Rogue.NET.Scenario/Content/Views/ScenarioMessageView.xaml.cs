using Prism.Events;
using Rogue.NET.Core.Model.ScenarioMessage.Message;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Content.ViewModel.Message;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Linq;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.Views
{
    [Export]
    public partial class ScenarioMessageView : UserControl
    {
        const int MAX_MESSAGES = 100;

        public ObservableCollection<ScenarioMessageViewModel> ScenarioMessages { get; set; }

        [ImportingConstructor]
        public ScenarioMessageView(IEventAggregator eventAggregator)
        {
            this.ScenarioMessages = new ObservableCollection<ScenarioMessageViewModel>();

            this.DataContext = this;

            InitializeComponent();

            eventAggregator.GetEvent<ScenarioMessageEvent>().Subscribe((message) =>
            {
                if (message is AlterationMessage)
                    AddAlterationMessage(message as AlterationMessage);

                else if (message is EnemyAlterationMessage)
                    AddEnemyAlterationMessage(message as EnemyAlterationMessage);

                else if (message is MeleeMessage)
                    AddMeleeMessage(message as MeleeMessage);

                else if (message is NormalMessage)
                    AddNormalMessage(message as NormalMessage);

                else if (message is PlayerAdvancementMessage)
                    AddPlayerAdvancementMessage(message as PlayerAdvancementMessage);

                else if (message is SkillAdvancementMessage)
                    AddSkillAdvancementMessage(message as SkillAdvancementMessage);
            });
        }

        private void AddAlterationMessage(AlterationMessage message)
        {
            var viewModel = new ScenarioAlterationMessageViewModel()
            {
                AlterationDisplayName = message.AlterationDisplayName,
                Effect = message.Effect,
                EffectedAttributeName = message.EffectedAttributeName,
                IsCausedByAttackAttributes = message.IsCausedByAttackAttributes,
                Priority = message.Priority
            };

            if (message.IsCausedByAttackAttributes)
            {
                viewModel.AttackAttributeHits
                         .AddRange(
                    message.AttackAttributeEffect
                           .Select(x => (AttackAttributeHitViewModel)SetScenarioImageProperties(
                                           new AttackAttributeHitViewModel()
                                           {
                                               AttackAttributeName = x.Key.RogueName,
                                               Hit = x.Value,
                                           }, x.Key)));
            }

            InsertMessage(viewModel);
        }

        private void AddEnemyAlterationMessage(EnemyAlterationMessage message)
        {
            var viewModel = new ScenarioEnemyAlterationMessageViewModel()
            {
                AlterationDisplayName = message.AlterationDisplayName,
                EnemyDisplayName = message.EnemyDisplayName,
                Priority = message.Priority
            };

            InsertMessage(viewModel);
        }

        private void AddMeleeMessage(MeleeMessage message)
        {
            var viewModel = new ScenarioMeleeMessageViewModel()
            {
                ActeeDisplayName = message.ActeeDisplayName,
                ActorDisplayName = message.ActorDisplayName,
                AnyAttackAttributes = message.AnyAttackAttributes,
                BaseHit = message.BaseHit,
                IsCriticalHit = message.IsCriticalHit,
                Priority = message.Priority
            };

            if (message.AnyAttackAttributes)
            {
                viewModel.AttackAttributeHits
                         .AddRange(
                    message.AttackAttributeHit
                           .Select(x => (AttackAttributeHitViewModel)SetScenarioImageProperties(
                                           new AttackAttributeHitViewModel()
                                           {
                                               AttackAttributeName = x.Key.RogueName,
                                               Hit = x.Value,
                                           }, x.Key)));
            }

            InsertMessage(viewModel);
        }

        private void AddNormalMessage(NormalMessage message)
        {
            var viewModel = new ScenarioNormalMessageViewModel()
            {
                Message = message.Message,
                Priority = message.Priority
            };

            InsertMessage(viewModel);
        }

        private void AddPlayerAdvancementMessage(PlayerAdvancementMessage message)
        {
            var viewModel = new ScenarioPlayerAdvancementMessageViewModel()
            {
                Priority = message.Priority,
                PlayerLevel = message.PlayerLevel,
                PlayerName = message.PlayerName
            };
            viewModel.AttributeChanges.AddRange(message.AttributeChanges.Select(x => new AttributeChangeViewModel()
            {
                AttributeName = x.Item1,
                Change = x.Item2,
                Color = x.Item3
            }));

            InsertMessage(viewModel);
        }

        private void AddSkillAdvancementMessage(SkillAdvancementMessage message)
        {
            var viewModel = new ScenarioSkillAdvancementMessageViewModel()
            {
                Priority = message.Priority,
                SkillDisplayName = message.SkillDisplayName,
                SkillLevel = message.SkillLevel                
            };

            InsertMessage(viewModel);
        }

        private void InsertMessage(ScenarioMessageViewModel message)
        {
            this.ScenarioMessages.Insert(0, message);

            if (this.ScenarioMessages.Count > MAX_MESSAGES)
                this.ScenarioMessages.RemoveAt(this.ScenarioMessages.Count - 1);
        }

        private ScenarioImageViewModel SetScenarioImageProperties(ScenarioImageViewModel scenarioImage, ScenarioImage scenarioMetaData)
        {
            scenarioImage.CharacterColor = scenarioMetaData.CharacterColor;
            scenarioImage.CharacterSymbol = scenarioMetaData.CharacterSymbol;
            scenarioImage.Icon = scenarioMetaData.Icon;
            scenarioImage.DisplayIcon = scenarioMetaData.DisplayIcon;
            scenarioImage.SmileyBodyColor = scenarioMetaData.SmileyBodyColor;
            scenarioImage.SmileyLineColor = scenarioMetaData.SmileyLineColor;
            scenarioImage.SmileyAuraColor = scenarioMetaData.SmileyAuraColor;
            scenarioImage.SmileyMood = scenarioMetaData.SmileyMood;
            scenarioImage.SymbolType = scenarioMetaData.SymbolType;

            return scenarioImage;
        }
    }
}
