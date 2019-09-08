using Rogue.NET.Scenario.Content.ViewModel.Message;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Linq;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage;
using Rogue.NET.Core.Processing.Event.Scenario;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class ScenarioMessageView : UserControl
    {
        const int MAX_MESSAGES = 100;

        public ObservableCollection<ScenarioMessageViewModel> ScenarioMessages { get; set; }

        [ImportingConstructor]
        public ScenarioMessageView(IRogueEventAggregator eventAggregator)
        {
            this.ScenarioMessages = new ObservableCollection<ScenarioMessageViewModel>();

            this.DataContext = this;

            InitializeComponent();

            eventAggregator.GetEvent<NewScenarioEvent>().Subscribe((e) =>
            {
                Reset();
            });

            eventAggregator.GetEvent<ContinueScenarioEvent>().Subscribe(() =>
            {
                Reset();
            });

            eventAggregator.GetEvent<OpenScenarioEvent>().Subscribe((e) =>
            {
                Reset();
            });

            eventAggregator.GetEvent<ScenarioMessageEvent>().Subscribe((message) =>
            {
                if (message is AlterationMessageData)
                    AddAlterationMessage(message as AlterationMessageData);

                else if (message is EnemyAlterationMessageData)
                    AddEnemyAlterationMessage(message as EnemyAlterationMessageData);

                else if (message is MeleeMessageData)
                    AddMeleeMessage(message as MeleeMessageData);

                else if (message is NormalMessageData)
                    AddNormalMessage(message as NormalMessageData);

                else if (message is PlayerAdvancementMessageData)
                    AddPlayerAdvancementMessage(message as PlayerAdvancementMessageData);

                else if (message is SkillAdvancementMessageData)
                    AddSkillAdvancementMessage(message as SkillAdvancementMessageData);
            });
        }

        private void AddAlterationMessage(AlterationMessageData message)
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

        private void AddEnemyAlterationMessage(EnemyAlterationMessageData message)
        {
            var viewModel = new ScenarioEnemyAlterationMessageViewModel()
            {
                AlterationDisplayName = message.AlterationDisplayName,
                EnemyDisplayName = message.EnemyDisplayName,
                Priority = message.Priority
            };

            InsertMessage(viewModel);
        }

        private void AddMeleeMessage(MeleeMessageData message)
        {
            var viewModel = new ScenarioMeleeMessageViewModel()
            {
                DefenderDisplayName = message.DefenderDisplayName,
                AttackerDisplayName = message.AttackerDisplayName,
                AnySpecializedHits = message.AnySpecializedHits,
                BaseHit = message.BaseHit,
                IsCriticalHit = message.IsCriticalHit,
                Priority = message.Priority
            };

            if (message.AnySpecializedHits)
            {
                viewModel.SpecializedHits
                         .AddRange(
                    message.SpecializedHits
                           .Select(x => (AttackAttributeHitViewModel)SetScenarioImageProperties(
                                           new AttackAttributeHitViewModel()
                                           {
                                               AttackAttributeName = x.Key.RogueName,
                                               Hit = x.Value,
                                           }, x.Key)));
            }

            InsertMessage(viewModel);
        }

        private void AddNormalMessage(NormalMessageData message)
        {
            var viewModel = new ScenarioNormalMessageViewModel()
            {
                Message = message.Message,
                Priority = message.Priority
            };

            InsertMessage(viewModel);
        }

        private void AddPlayerAdvancementMessage(PlayerAdvancementMessageData message)
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

        private void AddSkillAdvancementMessage(SkillAdvancementMessageData message)
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

        private void Reset()
        {
            this.ScenarioMessages.Clear();
        }

        private ScenarioImageViewModel SetScenarioImageProperties(ScenarioImageViewModel scenarioImage, ScenarioImage scenarioMetaData)
        {
            scenarioImage.CharacterColor = scenarioMetaData.CharacterColor;
            scenarioImage.CharacterSymbol = scenarioMetaData.CharacterSymbol;
            scenarioImage.Icon = scenarioMetaData.Icon;
            scenarioImage.DisplayIcon = scenarioMetaData.DisplayIcon;
            scenarioImage.SmileyBodyColor = scenarioMetaData.SmileyBodyColor;
            scenarioImage.SmileyLineColor = scenarioMetaData.SmileyLineColor;
            scenarioImage.SmileyAuraColor = scenarioMetaData.SmileyLightRadiusColor;
            scenarioImage.SmileyExpression = scenarioMetaData.SmileyExpression;
            scenarioImage.SymbolType = scenarioMetaData.SymbolType;

            return scenarioImage;
        }
    }
}
