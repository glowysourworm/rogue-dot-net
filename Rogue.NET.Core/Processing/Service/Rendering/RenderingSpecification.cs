using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Rendering
{
    public class RenderingSpecification
    {
        public delegate bool VisibilityCallbackHandler(int column, int row);
        public delegate double EffectiveVisionHandler(int column, int row);
        public delegate bool IsRevealedCallback(int column, int row);
        public delegate bool IsExploredCallback(int column, int row);
        public delegate Light[] LightingCallback(int column, int row);

        public double ZoomFactor { get; private set; }

        public VisibilityCallbackHandler IsVisibileCallback { get; private set; }
        public VisibilityCallbackHandler WasVisibileCallback { get; private set; }
        public EffectiveVisionHandler EffectiveVisionCallback { get; private set; }
        public IsRevealedCallback RevealedCallback { get; private set; }
        public IsExploredCallback ExploredCallback { get; private set; }
        public LightingCallback EffectiveLightingCallback { get; private set; }

        public IReadOnlyList<RenderingLayer> Layers { get; private set; }

        public RenderingSpecification(IReadOnlyList<RenderingLayer> layers,
                                      VisibilityCallbackHandler isVisibleCallback,
                                      VisibilityCallbackHandler wasVisibleCallback,
                                      EffectiveVisionHandler effectiveVisionCallback,
                                      IsRevealedCallback isRevealedCallback,
                                      IsExploredCallback isExploredCallback,
                                      LightingCallback lightingCallback,
                                      double zoomFactor)
        {
            this.Layers = layers;
            this.IsVisibileCallback = isVisibleCallback;
            this.WasVisibileCallback = wasVisibleCallback;
            this.EffectiveVisionCallback = effectiveVisionCallback;
            this.RevealedCallback = isRevealedCallback;
            this.ExploredCallback = isExploredCallback;
            this.EffectiveLightingCallback = lightingCallback;
            this.ZoomFactor = zoomFactor;
        }
    }
}
