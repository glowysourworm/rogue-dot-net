using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.Validation.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.ViewModel.Validation
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioValidationViewModel))]
    public class ScenarioValidationViewModel : NotifyViewModel, IScenarioValidationViewModel
    {
        bool _validationPassed;
        bool _validationRequired;
        int _validationErrorCount;
        int _validationWarningCount;
        int _validationInfoCount;

        public bool ValidationPassed
        {
            get { return _validationPassed; }
            set { this.RaiseAndSetIfChanged(ref _validationPassed, value); }
        }
        public bool ValidationRequired
        {
            get { return _validationRequired; }
            set { this.RaiseAndSetIfChanged(ref _validationRequired, value); }
        }
        public int ValidationErrorCount
        {
            get { return _validationErrorCount; }
            set { this.RaiseAndSetIfChanged(ref _validationErrorCount, value); }
        }

        public int ValidationWarningCount
        {
            get { return _validationWarningCount; }
            set { this.RaiseAndSetIfChanged(ref _validationWarningCount, value); }
        }

        public int ValidationInfoCount
        {
            get { return _validationInfoCount; }
            set { this.RaiseAndSetIfChanged(ref _validationInfoCount, value); }
        }
        public ObservableCollection<IScenarioValidationResult> ValidationMessages { get; private set; }

        [ImportingConstructor]
        public ScenarioValidationViewModel(IRogueEventAggregator eventAggregator)
        {
            this.ValidationMessages = new ObservableCollection<IScenarioValidationResult>();

            // Set validation required flag when scenario is updated
            eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(scenarioCollectionProvider =>
            {
                this.ValidationRequired = true;
            });
        }

        public void Set(IEnumerable<IScenarioValidationResult> validationMessages)
        {
            this.ValidationMessages.Clear();
            this.ValidationMessages.AddRange(validationMessages);
            this.ValidationRequired = false;
            this.ValidationPassed = !validationMessages.Any(x => !x.Passed &&
                                                                 (x.Severity == ValidationSeverity.Error));
            this.ValidationErrorCount = validationMessages.Count(x => x.Severity == ValidationSeverity.Error);
            this.ValidationWarningCount = validationMessages.Count(x => x.Severity == ValidationSeverity.Warning);
            this.ValidationInfoCount = validationMessages.Count(x => x.Severity == ValidationSeverity.Info);
        }

        public void Clear()
        {
            this.ValidationMessages.Clear();
            this.ValidationPassed = false;
            this.ValidationRequired = false;
            this.ValidationErrorCount = 0;
            this.ValidationWarningCount = 0;
            this.ValidationInfoCount = 0;
        }
    }
}
