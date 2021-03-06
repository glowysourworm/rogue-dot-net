﻿using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Processing.Action;
using Rogue.NET.Core.Processing.Event.Backend.EventData;

namespace Rogue.NET.Core.Processing.Model.Content
{
    public abstract class BackendProcessor
    {
        public event SimpleEventHandler<AnimationEventData> AnimationEvent;
        public event SimpleEventHandler<ProjectileAnimationEventData> ProjectileAnimationEvent;
        public event SimpleEventHandler<DialogEventData> DialogEvent;
        public event SimpleEventHandler<LevelEventData> LevelEvent;
        public event SimpleEventHandler<TargetRequestEventData> TargetRequestEvent;
        public event SimpleEventHandler<ScenarioEventData> ScenarioEvent;
        public event SimpleEventHandler<LevelProcessingAction> LevelProcessingActionEvent;

        public virtual void ApplyEndOfTurn(bool regenerate)
        {

        }

        protected virtual void OnAnimationEvent(AnimationEventData eventData)
        {
            if (this.AnimationEvent != null)
                this.AnimationEvent(eventData);
        }

        protected virtual void OnProjectileAnimationEvent(ProjectileAnimationEventData eventData)
        {
            if (this.ProjectileAnimationEvent != null)
                this.ProjectileAnimationEvent(eventData);
        }

        protected virtual void OnDialogEvent(DialogEventData eventData)
        {
            if (this.DialogEvent != null)
                this.DialogEvent(eventData);
        }

        protected virtual void OnLevelEvent(LevelEventData eventData)
        {
            if (this.LevelEvent != null)
                this.LevelEvent(eventData);
        }

        protected virtual void OnTargetingRequesetEvent(TargetRequestEventData eventData)
        {
            if (this.TargetRequestEvent != null)
                this.TargetRequestEvent(eventData);
        }

        protected virtual void OnScenarioEvent(ScenarioEventData eventData)
        {
            if (this.ScenarioEvent != null)
                this.ScenarioEvent(eventData);
        }

        protected virtual void OnLevelProcessingEvent(LevelProcessingAction action)
        {
            if (this.LevelProcessingActionEvent != null)
                this.LevelProcessingActionEvent(action);
        }
    }
}
