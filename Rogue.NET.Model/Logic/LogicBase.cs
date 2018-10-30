using Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Common.Collections;
using Rogue.NET.Common.EventArgs;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Model.Events;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Model.Logic
{
    public class LogicBase
    {
        readonly IEventAggregator _eventAggregator;

        protected SerializableObservableCollection<LevelData.DialogMessage> DialogMessages { get; set; }
        protected SerializableDictionary<string, ScenarioMetaData> Encyclopedia { get; set; }
        protected SerializableObservableCollection<Enemy> TargetedEnemies { get; set; }
        protected ScenarioConfiguration ScenarioConfig { get; set; }
        protected Random Random { get; set; }
        protected Player Player { get; set; }
        protected Level Level { get; set; }

        public LogicBase(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Initialize();
        }

        private void Initialize()
        {
            _eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe((e) =>
            {
                this.Encyclopedia = e.Encyclopedia;
                this.ScenarioConfig = e.Config;
                this.Random = new Random(e.Seed);
                this.Player = e.Player;
                this.Level = e.Level;
                this.DialogMessages = e.DialogMessages;
                this.TargetedEnemies = e.TargetedEnemies;

                OnLevelLoaded(e.StartLocation);
            });


            _eventAggregator.GetEvent<UserCommandEvent>().Subscribe((e) =>
            {
                // if level is loaded
                if (this.Level == null)
                    return;

                OnLevelCommand(e);
            });

            _eventAggregator.GetEvent<AnimationCompletedEvent>().Subscribe((e) =>
            {
                OnAnimationCompleted(e);
            });
        }

        protected virtual void PublishInitializedEvent()
        {
            _eventAggregator.GetEvent<LevelInitializedEvent>().Publish(new LevelInitializedEvent()
            {
                Data = _unityContainer.Resolve<LevelData>()
            });
        }

        protected virtual void PublishScenarioMessage(string message)
        {
            this.DialogMessages.Add(new LevelData.DialogMessage()
            {
                Message = message,
                Timestamp = DateTime.Now
            });

            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new ScenarioMessageEvent()
            {
                Message = message
            });
        }

        protected virtual void PublishPlayerDiedEvent(string diedOf)
        {
            _eventAggregator.GetEvent<PlayerDiedEvent>().Publish(new PlayerDiedEvent()
            {
                PlayerName = this.Player.RogueName,
                DiedOf = diedOf
            });
        }

        protected virtual void PublishAnimationEvent(
            AnimationReturnAction returnAction, 
            List<AnimationTemplate> animations, 
            Alteration alteration,
            Character source,
            List<Character> targets)
        {
            _eventAggregator.GetEvent<AnimationStartEvent>().Publish(new AnimationStartEvent()
            {
                Alteration = alteration,
                Animations = animations,
                ReturnAction = returnAction,
                Source = source,
                Targets = targets
            });
        }

        protected virtual void PublishLoadLevelRequest(int number, PlayerStartLocation startLocation)
        {
            _eventAggregator.GetEvent<LoadLevelEvent>().Publish(new LoadLevelEvent()
            {
                LevelNumber = number,
                StartLocation = startLocation
            });
        }

        protected virtual void PublishSaveEvent()
        {
            _eventAggregator.GetEvent<SaveScenarioEvent>().Publish(new SaveScenarioEvent()
            {

            });
        }

        protected virtual void PublishSplashScreenEvent(SplashEventType splashEvent)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
            {
                SplashAction = SplashAction.Show,
                SplashType = splashEvent
            });
        }

        protected virtual void PublishScenarioTickEvent()
        {
            _eventAggregator.GetEvent<ScenarioTickEvent>().Publish(new ScenarioTickEvent() { });
        }

        protected virtual void PublishTargetEvent(Enemy targetedEnemy)
        {
            _eventAggregator.GetEvent<EnemyTargetedEvent>().Publish(new EnemyTargetedEvent()
            {
                TargetedEnemy = targetedEnemy,
                TargetingEnded = targetedEnemy == null
            });
        }

        protected virtual void OnLevelCommand(LevelCommandEventArgs args)
        {

        }

        protected virtual void OnLevelLoaded(PlayerStartLocation location)
        {
        }

        protected virtual void OnAnimationCompleted(AnimationCompletedEventArgs e)
        {
        }
    }
}
