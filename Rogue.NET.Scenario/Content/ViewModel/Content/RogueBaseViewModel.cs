﻿using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class RogueBaseViewModel : DependencyObject, INotifyPropertyChanged
    {
        string _id;
        string _rogueName;

        public string Id
        {
            get { return _id; }
            private set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string RogueName
        {
            get { return _rogueName; }
            set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
        }
        public RogueBaseViewModel() { }
        public RogueBaseViewModel(string id, string rogueName)
        {
            this.Id = id;
            this.RogueName = rogueName;
        }

        public RogueBaseViewModel(RogueBase rogueBase)
        {
            this.Id = rogueBase.Id;
            this.RogueName = rogueBase.RogueName;
        }

        public RogueBaseViewModel(Template templateBase)
        {
            this.Id = templateBase.Guid;
            this.RogueName = templateBase.Name;
        }

        #region Notify Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        protected void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression != null)
                OnPropertyChanged(memberExpression.Member.Name);

            else
                throw new Exception("Improper expression type RogueBaseViewModel.OnPropertyChanged<T>");
        }

        /// <summary>
        /// Raised INotifyPropertyChanged event if there's a change to the property. Returns true if there was
        /// a change
        /// </summary>
        protected virtual bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string memberName = "")
        {
            var changed = false;
            if (field == null)
                changed = value != null;
            else
                changed = !field.Equals(value);

            if (changed)
            {
                field = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }

            return changed;
        }
        #endregion

    }
}
