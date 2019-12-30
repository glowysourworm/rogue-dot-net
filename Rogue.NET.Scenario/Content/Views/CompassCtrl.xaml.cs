using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Event.Level;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface;
using Rogue.NET.Scenario.Processing.Event.Content;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class CompassCtrl : UserControl
    {
        double ZOOM_INCREMENT = 0.25;
        double MARKER_SIZE = 6.0D;

        List<Rectangle> _canvasPoints = new List<Rectangle>();

        [ImportingConstructor]
        public CompassCtrl(IRogueEventAggregator eventAggregator, 
                           IScenarioUIGeometryService scenarioUIGeometryService, 
                           IModelService modelService, 
                           PlayerViewModel playerViewModel)
        {
            this.DataContext = playerViewModel;

            InitializeComponent();

            // subscribe to events
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                Update(modelService, scenarioUIGeometryService);
            });

            eventAggregator.GetEvent<LevelEvent>().Subscribe(update =>
            {
                if (update.LevelUpdateType == LevelEventType.PlayerLocation)
                    Update(modelService, scenarioUIGeometryService);
            });

            this.UpButton.Click += (sender, e) =>
            {
                eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Up);
            };
            this.DownButton.Click += (sender, e) =>
            {
                eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Down);
            };
            this.LeftButton.Click += (sender, e) =>
            {
                eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Left);
            };
            this.RightButton.Click += (sender, e) =>
            {
                eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Right);
            };
            this.CenterButton.Click += (sender, e) =>
            {
                eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.CenterOnPlayer);
            };
            this.ZoomInButton.Click += (sender, e) =>
            {
                var oldFactor = modelService.ZoomFactor;
                var newFactor = (modelService.ZoomFactor + ZOOM_INCREMENT).Clip(1.0, 3.0);

                // SET MODEL SERVICE VALUE
                modelService.ZoomFactor = newFactor;

                eventAggregator.GetEvent<ZoomEvent>().Publish(new ZoomEventData()
                {
                    NewZoomFactor = newFactor,
                    OldZoomFactor = oldFactor
                });
            };
            this.ZoomOutButton.Click += (sender, e) =>
            {
                var oldFactor = modelService.ZoomFactor;
                var newFactor = (modelService.ZoomFactor - ZOOM_INCREMENT).Clip(1.0, 3.0);

                // SET MODEL SERVICE VALUE
                modelService.ZoomFactor = newFactor;

                eventAggregator.GetEvent<ZoomEvent>().Publish(new ZoomEventData()
                {
                    NewZoomFactor = newFactor,
                    OldZoomFactor = oldFactor
                });
            };
            this.ObjectivesButton.Click += (sender, e) =>
            {
                eventAggregator.GetEvent<DialogEvent>().Publish(new DialogEventData()
                {
                    Type = DialogEventType.Objective
                });
            };
            this.HelpButton.Click += (sender, e) =>
            {
                eventAggregator.GetEvent<DialogEvent>().Publish(new DialogEventData()
                {
                    Type = DialogEventType.Help
                });
            };

        }

        private void Update(IModelService modelService, IScenarioUIGeometryService scenarioUIGeometryService)
        {
            foreach (var rectangle in _canvasPoints)
                this.MapCanvas.Children.Remove(rectangle);

            var levelUIBounds = scenarioUIGeometryService.Cell2UIRect(modelService.Level.Grid.Bounds);
            var playerLocation = scenarioUIGeometryService.Cell2UI(modelService.PlayerLocation);
            var midpointLocation = new Point(150, 90);

            //// Add visible contents to map
            //foreach (var content in modelService.CharacterContentInformation.GetVisibleContents(modelService.Player))
            //{
            //    var contentMarker = new Rectangle();
            //    contentMarker.StrokeThickness = 0;
            //    contentMarker.Fill = content is Enemy ? Brushes.Red : Brushes.LightBlue;
            //    contentMarker.Fill = content is ItemBase ? Brushes.YellowGreen : contentMarker.Fill;

            //    // Center on player location - calculate offset relative to player
            //    var contentLocation = scenarioUIGeometryService.Cell2UI(modelService.GetLocation(content));
            //    var offset = new Point(contentLocation.X - playerLocation.X, contentLocation.Y - playerLocation.Y);

            //    var mapOffsetX = (offset.X / levelUIBounds.Width) * this.MapCanvas.RenderSize.Width;
            //    var mapOffsetY = (offset.Y / levelUIBounds.Height) * this.MapCanvas.RenderSize.Height;

            //    contentMarker.Width = MARKER_SIZE;
            //    contentMarker.Height = MARKER_SIZE;

            //    Canvas.SetLeft(contentMarker, mapOffsetX + midpointLocation.X - (MARKER_SIZE / 2.0));
            //    Canvas.SetTop(contentMarker, mapOffsetY + midpointLocation.Y - (MARKER_SIZE / 2.0));

            //    _canvasPoints.Add(contentMarker);

            //    this.MapCanvas.Children.Add(contentMarker);
            //}
        }
    }
}
