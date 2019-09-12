using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class TemporaryCharacterTemplate : NonPlayerCharacterTemplate
    {
        Range<int> _lifetimeCounter;

        public Range<int> LifetimeCounter
        {
            get { return _lifetimeCounter; }
            set
            {
                if (_lifetimeCounter != value)
                {
                    _lifetimeCounter = value;
                    OnPropertyChanged("LifetimeCounter");
                }
            }
        }

        public TemporaryCharacterTemplate()
        {
            this.LifetimeCounter = new Range<int>(100, 150);
        }
    }
}
