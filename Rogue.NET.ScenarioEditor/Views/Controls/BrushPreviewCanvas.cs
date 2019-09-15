using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    /// <summary>
    /// Canvas component that listens for a BrushTemplateViewModel.BrushChangedEvent to update it's binding
    /// </summary>
    public class BrushPreviewCanvas : Canvas
    {
        public BrushPreviewCanvas() : base()
        {
            this.DataContextChanged += (sender, e) =>
            {
                var oldTemplate = e.OldValue as BrushTemplateViewModel;
                var newTemplate = e.NewValue as BrushTemplateViewModel;

                if (oldTemplate != null)
                    oldTemplate.BrushUpdatedEvent -= OnBrushChanged;

                if (newTemplate != null)
                    newTemplate.BrushUpdatedEvent += OnBrushChanged;

                if (newTemplate != null)
                    OnBrushChanged();
            };
        }
        private void OnBrushChanged()
        {
            // NOTE*** Problem with updating the binding for the brush was that the multibinding 
            //         wasn't supporting the ObservableCollection as one of its bindings. Nothing
            //         seemed to work - and this is because (i think) the notify collection changed
            //         interface / observable collection doesn't support item updtaes. It becomes
            //         a design problem of whether you're working with an item or it's collection; and
            //         causes issues.
            //
            //         The other solution was to try and create a custom Binding MarkupExtension that
            //         listens for an update and sets the binding - basically what we're doing here.
            //
            //         I would've prefered the MarkupExtension; but there were some strange errors
            //         trying to use the BindingOperations; and I didn't want to bother trying to 
            //         figure them out. 
            //

            var expression = this.GetBindingExpression(Canvas.BackgroundProperty);

            if (expression != null)
                expression.UpdateTarget();
        }
    }
}
