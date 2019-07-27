using Prism.Events;
using Rogue.NET.Core.Event.Scenario.Level.Command;
using Rogue.NET.Core.Event.Scenario.Level.EventArgs;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ViewModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class PlayerAdvancementView : UserControl
    {
        [ImportingConstructor]
        public PlayerAdvancementView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.AcceptButton.Click += async (sender, e) =>
            {
                var viewModel = this.DataContext as PlayerAdvancementViewModel;

                if (viewModel != null)
                {
                    await eventAggregator.GetEvent<UserCommandEvent>()
                                         .Publish(new PlayerAdvancementCommandEventArgs()
                                         {
                                             Type = PlayerActionType.PlayerAdvancement,
                                             Agility = viewModel.NewAgility,
                                             Intelligence = viewModel.NewIntelligence,
                                             Strength = viewModel.NewStrength,
                                             SkillPoints = viewModel.NewSkillPoints
                                         });
                }
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

        private void OnPointsChanged(PlayerAdvancementViewModel viewModel)
        {
            this.AcceptButton.IsEnabled = (viewModel.PlayerPoints == 0);
        }
    }
}
