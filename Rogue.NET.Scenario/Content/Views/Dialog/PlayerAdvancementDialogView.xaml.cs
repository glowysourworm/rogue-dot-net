﻿using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Scenario.Content.ViewModel.Dialog;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views.Dialog
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class PlayerAdvancementDialogView : UserControl, IDialogView
    {
        public event SimpleEventHandler<IDialogView, object> DialogViewFinishedEvent;

        [ImportingConstructor]
        public PlayerAdvancementDialogView(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.AcceptButton.Click += (sender, e) =>
            {
                if (this.DialogViewFinishedEvent != null)
                    this.DialogViewFinishedEvent(this, this.DataContext as PlayerAdvancementViewModel);
            };

            // + / - Buttons
            this.AgilityMinusButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as PlayerAdvancementViewModel;
                if (viewModel != null)
                {
                    // Subtract a point and add it to the player points
                    if (viewModel.NewAgility > viewModel.Agility)
                    {
                        viewModel.NewAgility--;
                        viewModel.PlayerPoints++;

                        OnPointsChanged(viewModel);
                    }
                }
            };
            this.IntelligenceMinusButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as PlayerAdvancementViewModel;
                if (viewModel != null)
                {
                    // Subtract a point and add it to the player points
                    if (viewModel.NewIntelligence > viewModel.Intelligence)
                    {
                        viewModel.NewIntelligence--;
                        viewModel.PlayerPoints++;

                        OnPointsChanged(viewModel);
                    }
                }
            };
            this.StrengthMinusButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as PlayerAdvancementViewModel;
                if (viewModel != null)
                {
                    // Subtract a point and add it to the player points
                    if (viewModel.NewStrength > viewModel.Strength)
                    {
                        viewModel.NewStrength--;
                        viewModel.PlayerPoints++;

                        OnPointsChanged(viewModel);
                    }
                }
            };
            this.SkillPointsMinusButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as PlayerAdvancementViewModel;
                if (viewModel != null)
                {
                    // Subtract a point and add it to the player points
                    if (viewModel.NewSkillPoints > viewModel.SkillPoints)
                    {
                        viewModel.NewSkillPoints -= 3;
                        viewModel.PlayerPoints++;

                        OnPointsChanged(viewModel);
                    }
                }
            };

            this.AgilityPlusButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as PlayerAdvancementViewModel;
                if (viewModel != null &&
                    viewModel.PlayerPoints > 0)
                {
                    viewModel.NewAgility++;
                    viewModel.PlayerPoints--;

                    OnPointsChanged(viewModel);
                }
            };
            this.IntelligencePlusButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as PlayerAdvancementViewModel;
                if (viewModel != null &&
                    viewModel.PlayerPoints > 0)
                {
                    viewModel.NewIntelligence++;
                    viewModel.PlayerPoints--;

                    OnPointsChanged(viewModel);
                }
            };
            this.StrengthPlusButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as PlayerAdvancementViewModel;
                if (viewModel != null &&
                    viewModel.PlayerPoints > 0)
                {
                    viewModel.NewStrength++;
                    viewModel.PlayerPoints--;

                    OnPointsChanged(viewModel);
                }
            };
            this.SkillPointsPlusButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as PlayerAdvancementViewModel;
                if (viewModel != null &&
                    viewModel.PlayerPoints > 0)
                {
                    viewModel.NewSkillPoints += 3;
                    viewModel.PlayerPoints--;

                    OnPointsChanged(viewModel);
                }
            };
        }

        public IEnumerable<string> GetMultipleSelectionModeSelectedItemIds()
        {
            return new List<string>();
        }

        private void OnPointsChanged(PlayerAdvancementViewModel viewModel)
        {
            this.AcceptButton.IsEnabled = (viewModel.PlayerPoints == 0);
        }
    }
}