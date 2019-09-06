using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Event.Scenario.Level.EventArgs;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Scenario.Content.ViewModel.Dialog;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using Rogue.NET.Scenario.Content.Views.ItemGrid;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.DialogMode;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.PrimaryMode;

namespace Rogue.NET.Scenario.Content.Views.Dialog
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class DialogContainer : UserControl, IDialogContainer
    {
        // This is the only saved state for this container - which must be managed.
        IDialogUpdate _dialogUpdate;

        // Use this event to notify listener that dialog has completed
        public event SimpleEventHandler<IDialogContainer, UserCommandEventArgs> DialogFinishedEvent;

        public DialogContainer()
        {
            InitializeComponent();
        }

        public void Initialize(IDialogUpdate update)
        {
            // Set state here to retrieve when finished
            _dialogUpdate = update;

            var view = CreateView(update);

            // Hook dialog event listener
            view.DialogViewFinishedEvent += OnDialogFinished;

            // Set view content
            this.DialogContentBorder.Child = view as FrameworkElement;
        }

        private void OnDialogFinished(IDialogView view, object data)
        {
            // Unhook event to complete cycle
            view.DialogViewFinishedEvent -= OnDialogFinished;
            
            UserCommandEventArgs args = null;

            // Prepare User Command Event (args)
            // Setup the proper dialog UI here
            switch (_dialogUpdate.Type)
            {
                // No User Command to fire
                case DialogEventType.Help:
                case DialogEventType.Commands:
                case DialogEventType.Objective:
                case DialogEventType.Note:
                    break;

                // Create User Command with single item id
                case DialogEventType.Identify:
                    {
                        args = new PlayerCommandEventArgs(PlayerActionType.Identify, (string)data);
                    }
                    break;
                case DialogEventType.Uncurse:
                    {
                        args = new PlayerCommandEventArgs(PlayerActionType.Uncurse, (string)data);
                    }
                    break;
                case DialogEventType.AlterationEffect:
                    {
                        var effect = (_dialogUpdate as IDialogAlterationEffectUpdate).Effect;
                        if (effect is EquipmentEnhanceAlterationEffect)
                        {
                            args = new PlayerAlterationEffectCommandEventArgs(effect, PlayerActionType.AlterationEffect, (string)data);
                        }
                        else if (effect is TransmuteAlterationEffect)
                        {
                            args = new PlayerAlterationEffectMultiItemCommandEventArgs(effect, PlayerMultiItemActionType.AlterationEffect, (string[])data);
                        }
                        else
                            throw new Exception("Unhandled IAlterationEffect Type");
                    }
                    break;
                case DialogEventType.PlayerAdvancement:
                    {
                        var viewModel = (view as FrameworkElement).DataContext as PlayerAdvancementViewModel;

                        args = new PlayerAdvancementCommandEventArgs()
                        {
                            Agility = viewModel.NewAgility,
                            Intelligence = viewModel.NewIntelligence,
                            SkillPoints = viewModel.NewSkillPoints,
                            Strength = viewModel.NewStrength,
                            Type = PlayerActionType.PlayerAdvancement
                        };
                    }
                    break;
                default:
                    throw new Exception("Unknwon Dialog Event Type");
            }
            
            // Fire event to listeners
            if (this.DialogFinishedEvent != null)
                this.DialogFinishedEvent(this, args);
        }

        #region View / View Model Creation
        private IDialogView CreateView(IDialogUpdate update)
        {
            // Setup the proper dialog UI here
            switch (update.Type)
            {
                case DialogEventType.Help:
                    return GetInstance<HelpDialogView>() as IDialogView;
                case DialogEventType.Commands:
                    return GetInstance<CommandsDialogView>() as IDialogView;
                case DialogEventType.Objective:
                    return GetInstance<ObjectiveDialogView>() as IDialogView;
                case DialogEventType.Note:
                    {
                        // TODO: Use Binding Somehow...
                        var view = GetInstance<NoteDialogView>() as IDialogView;
                        (view as NoteDialogView).TitleTB.Text = (update as IDialogNoteUpdate).NoteTitle;
                        (view as NoteDialogView).MessageTB.Text = (update as IDialogNoteUpdate).NoteMessage;

                        return view;
                    }
                case DialogEventType.Identify:
                    {
                        // Get instance of each view model
                        var consumableViewModel = GetConsumablesViewModel(update);
                        var equipmentViewModel = GetEquipmentViewModel(update);

                        // Get injected dual item grid to do a single select
                        return new DualItemGrid(equipmentViewModel, consumableViewModel);
                    }
                case DialogEventType.Uncurse:
                    {
                        // Get manually setup view / view-model
                        var view = new EquipmentItemGrid();
                        var equipmentViewModel = GetEquipmentViewModel(update);

                        view.DataContext = equipmentViewModel;
                        return view;
                    }
                case DialogEventType.AlterationEffect:
                    {
                        var effect = (update as IDialogAlterationEffectUpdate).Effect;

                        if (effect is EquipmentEnhanceAlterationEffect)
                        {
                            // Get manually setup view / view-model
                            var view = new EquipmentItemGrid();
                            var equipmentViewModel = GetEquipmentViewModel(update);

                            view.DataContext = equipmentViewModel;
                            return view;
                        }
                        else if (effect is TransmuteAlterationEffect)
                        {
                            // Construct view models for transmute - also set selection mode
                            var consumableViewModel = GetConsumablesViewModel(update);
                            var equipmentViewModel = GetEquipmentViewModel(update);

                            // Get injected dual item grid to do a single select
                            return new DualItemGrid(equipmentViewModel, consumableViewModel);
                        }
                        else
                            throw new Exception("Unhandled IAlterationEffect Type");

                    }
                case DialogEventType.PlayerAdvancement:
                    {
                        var view = GetInstance<PlayerAdvancementDialogView>() as IDialogView;
                        var playerUpdate = update as IDialogPlayerAdvancementUpdate;

                        (view as UserControl).DataContext = new PlayerAdvancementViewModel()
                        {
                            Agility = playerUpdate.Agility,
                            Intelligence = playerUpdate.Intelligence,
                            Strength = playerUpdate.Strength,
                            SkillPoints = playerUpdate.SkillPoints,

                            // Initialize the new variables
                            NewAgility = playerUpdate.Agility,
                            NewIntelligence = playerUpdate.Intelligence,
                            NewStrength = playerUpdate.Strength,
                            NewSkillPoints = playerUpdate.SkillPoints,

                            // Points to spend
                            PlayerPoints = playerUpdate.PlayerPoints
                        };

                        return view;
                    }
                default:
                    throw new Exception("Unknwon Dialog Event Type");
            }
        }

        private T GetInstance<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        // NOTE:  This pattern was used to deal with mutliple functions for the
        //        consumables / equipment views. The intended action and selection
        //        mode aren't injectible parameters; but the event aggregator and
        //        model service typically are injected.. 
        //
        //        Using a view-model "locater" (injector) pattern, I thought maybe
        //        this could be simplified to an injection pattern.. but, the only
        //        method that makes sense is to pre-register view model instances
        //        using a key.
        //
        //        Example:  EquipmentViewModel [ Transmute, Multiple Selection Model ]
        //
        //                  injected into EquipmentItemGrid using "EquipmentTransmuteMultipleViewModel"
        //
        //                  ...
        //
        //        Then, a "view-model locater" would presribe an injected instance 
        //        seamlessly into the view.
        //
        //        This could probably work; but I'd be worried about dealing with 
        //        the injection context - "What is the situation for injecting 
        //        this particular view-model". Should this be dealt with in one
        //        place? By a single container? Will this get too tricky? What about
        //        managing the events / regions with this context? etc...
        //

        private ConsumableItemGridViewModelBase GetConsumablesViewModel(IDialogUpdate update)
        {
            ConsumableItemGridViewModelBase viewModel = null;

            switch (update.Type)
            {
                case DialogEventType.AlterationEffect:
                    {
                        var effect = (update as IDialogAlterationEffectUpdate).Effect;

                        if (effect is TransmuteAlterationEffect)
                            viewModel = GetInstance<ConsumableTransmuteItemGridViewModel>();
                        else
                            throw new Exception("Unhandled IAlterationEffect Type");
                    }
                    break;
                case DialogEventType.Identify:
                    viewModel = GetInstance<ConsumableIdentifyItemGridViewModel>();
                    break;
                case DialogEventType.Uncurse:
                case DialogEventType.Help:
                case DialogEventType.Commands:
                case DialogEventType.Objective:
                case DialogEventType.Note:
                case DialogEventType.PlayerAdvancement:
                default:
                    throw new Exception("Improper use of Consumable Item Grid View Model (dialog mode)");
            }

            viewModel.IsDialog = true;

            return viewModel;
        }

        private EquipmentItemGridViewModelBase GetEquipmentViewModel(IDialogUpdate update)
        {
            EquipmentItemGridViewModelBase viewModel = null;

            switch (update.Type)
            {
                case DialogEventType.AlterationEffect:
                    {
                        var effect = (update as IDialogAlterationEffectUpdate).Effect;

                        if (effect is EquipmentEnhanceAlterationEffect)
                        {
                            switch ((effect as EquipmentEnhanceAlterationEffect).Type)
                            {
                                case AlterationModifyEquipmentType.ArmorClass:
                                    viewModel = GetInstance<EquipmentEnchantArmorItemGridViewModel>();
                                    break;
                                case AlterationModifyEquipmentType.ArmorImbue:
                                    viewModel = GetInstance<EquipmentImbueArmorItemGridViewModel>();
                                    break;
                                case AlterationModifyEquipmentType.ArmorQuality:
                                    viewModel = GetInstance<EquipmentEnhanceArmorItemGridViewModel>();
                                    break;
                                case AlterationModifyEquipmentType.WeaponClass:
                                    viewModel = GetInstance<EquipmentEnchantWeaponItemGridViewModel>();
                                    break;
                                case AlterationModifyEquipmentType.WeaponImbue:
                                    viewModel = GetInstance<EquipmentImbueWeaponItemGridViewModel>();
                                    break;
                                case AlterationModifyEquipmentType.WeaponQuality:
                                    viewModel = GetInstance<EquipmentEnhanceWeaponItemGridViewModel>();
                                    break;
                                default:
                                    throw new Exception("Unhandled Alteration Equipment Modify Type");
                            }
                        }
                        else if (effect is TransmuteAlterationEffect)
                            viewModel = GetInstance<EquipmentTransmuteItemGridViewModel>();
                        else
                            throw new Exception("Unhandled IAlterationEffect Type");
                    }
                    break;
                case DialogEventType.Identify:
                    viewModel = GetInstance<EquipmentIdentifyItemGridViewModel>();
                    break;
                case DialogEventType.Uncurse:
                    viewModel = GetInstance<EquipmentUncurseItemGridViewModel>();
                    break;
                case DialogEventType.Help:
                case DialogEventType.Commands:
                case DialogEventType.Objective:
                case DialogEventType.Note:
                case DialogEventType.PlayerAdvancement:
                default:
                    throw new Exception("Improper use of Consumable Item Grid View Model (dialog mode)");
            }

            viewModel.IsDialog = true;

            return viewModel;
        }
        #endregion
    }
}
