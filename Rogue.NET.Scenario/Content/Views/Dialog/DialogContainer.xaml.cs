using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Event.Scenario.Level.EventArgs;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Scenario.Content.ViewModel.Dialog;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.Enum;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using Rogue.NET.Scenario.Content.Views.ItemGrid;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

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

        private void OnDialogFinished(IDialogView view)
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
                        var itemIds = view.GetSelectedItemIds();

                        if (itemIds.Count() != 1)
                            throw new Exception("Improper use of identify view (selection mode must be single)");

                        args = new PlayerCommandEventArgs(PlayerActionType.Identify, itemIds.First());
                    }
                    break;
                case DialogEventType.Uncurse:
                    {
                        var itemIds = view.GetSelectedItemIds();

                        if (itemIds.Count() != 1)
                            throw new Exception("Improper use of identify view (selection mode must be single)");

                        args = new PlayerCommandEventArgs(PlayerActionType.Uncurse, itemIds.First());
                    }
                    break;
                case DialogEventType.AlterationEffect:
                    {
                        var itemIds = view.GetSelectedItemIds();

                        var effect = (_dialogUpdate as IDialogAlterationEffectUpdate).Effect;
                        if (effect is EquipmentEnhanceAlterationEffect)
                        {
                            if (itemIds.Count() != 1)
                                throw new Exception("Improper use of identify view (selection mode must be single)");

                            args = new PlayerAlterationEffectCommandEventArgs(effect, PlayerActionType.AlterationEffect, itemIds.First());
                        }
                        else if (effect is TransmuteAlterationEffect)
                        {
                            args = new PlayerAlterationEffectMultiItemCommandEventArgs(effect, PlayerMultiItemActionType.AlterationEffect, itemIds.ToArray());
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
                        var consumableViewModel = GetConsumablesViewModel(ItemGridIntendedAction.Identify, ItemGridSelectionMode.Single);
                        var equipmentViewModel = GetEquipmentViewModel(ItemGridIntendedAction.Identify, ItemGridSelectionMode.Single);

                        // Get injected dual item grid to do a single select
                        return new DualItemGrid(equipmentViewModel, consumableViewModel);
                    }
                case DialogEventType.Uncurse:
                    {
                        var equipmentViewModel = GetEquipmentViewModel(ItemGridIntendedAction.Uncurse, ItemGridSelectionMode.Single);

                        equipmentViewModel.IntendedAction = ItemGridIntendedAction.Uncurse;

                        // Get injected dual item grid to do a single select
                        return new EquipmentItemGrid(equipmentViewModel);
                    }
                case DialogEventType.AlterationEffect:
                    {
                        var effect = (update as IDialogAlterationEffectUpdate).Effect;
                        if (effect is EquipmentEnhanceAlterationEffect)
                        {
                            // Calculate the intended action
                            ItemGridIntendedAction intendedAction;

                            // Set up the intended action
                            switch ((effect as EquipmentEnhanceAlterationEffect).Type)
                            {
                                case AlterationModifyEquipmentType.ArmorClass:
                                    intendedAction = ItemGridIntendedAction.EnchantArmor;
                                    break;
                                case AlterationModifyEquipmentType.ArmorImbue:
                                    intendedAction = ItemGridIntendedAction.ImbueArmor;
                                    break;
                                case AlterationModifyEquipmentType.ArmorQuality:
                                    intendedAction = ItemGridIntendedAction.EnhanceArmor;
                                    break;
                                case AlterationModifyEquipmentType.WeaponClass:
                                    intendedAction = ItemGridIntendedAction.EnchantWeapon;
                                    break;
                                case AlterationModifyEquipmentType.WeaponImbue:
                                    intendedAction = ItemGridIntendedAction.ImbueWeapon;
                                    break;
                                case AlterationModifyEquipmentType.WeaponQuality:
                                    intendedAction = ItemGridIntendedAction.EnhanceWeapon;
                                    break;
                                default:
                                    throw new Exception("Unhandled Alteration Equipment Modify Type");
                            }

                            // Get instance of the item grid
                            var viewModel = GetEquipmentViewModel(intendedAction, ItemGridSelectionMode.Single);

                            return new EquipmentItemGrid(viewModel);
                        }
                        else if (effect is TransmuteAlterationEffect)
                        {
                            // Construct view models for transmute - also set selection mode
                            var consumableViewModel = GetConsumablesViewModel(ItemGridIntendedAction.Transmute, ItemGridSelectionMode.Multiple);
                            var equipmentViewModel = GetEquipmentViewModel(ItemGridIntendedAction.Transmute, ItemGridSelectionMode.Multiple);

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

        private ConsumableItemGridViewModel GetConsumablesViewModel(
                    ItemGridIntendedAction intendedAction, 
                    ItemGridSelectionMode selectionMode)
        {
            var viewModel = GetInstance<ConsumableItemGridViewModel>();

            viewModel.IntendedAction = intendedAction;
            viewModel.SelectionMode = selectionMode;
            viewModel.IsDialog = true;

            return viewModel;
        }

        private EquipmentItemGridViewModel GetEquipmentViewModel(
            ItemGridIntendedAction intendedAction,
            ItemGridSelectionMode selectionMode)
        {
            var viewModel = GetInstance<EquipmentItemGridViewModel>();

            viewModel.IntendedAction = intendedAction;
            viewModel.SelectionMode = selectionMode;
            viewModel.IsDialog = true;

            return viewModel;
        }
        #endregion
    }
}
